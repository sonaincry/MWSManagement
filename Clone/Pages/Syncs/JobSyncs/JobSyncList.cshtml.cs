using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MWSManagement.ControlUI.Helper.Grids;
using MWSManagement.Models.Entities;
using Indotalent.Applications.JobSyncs;
using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Dotmim.Sync.Enumerations;
using System.Text;
using MWSManagement.Applications.Locations;
using System.Data;

namespace MWSManagement.Pages.Syncs.JobSyncs
{
    public class JobSyncListModel : PageModel
    {
        private readonly JobSyncService _service;
        private readonly LocationService _locationService;

        public JobSyncListModel(JobSyncService service, LocationService locationService)
        {
            _service = service;
            _locationService = locationService;
        }

        public List<object> ConfigList { get; set; } = new();
        public List<GridColumnDto> GridColumns { get; set; } = new();

        public async Task OnGetAsync()
        {
            GridColumns = GridColumnHelper.FromModel<JobSync>();
            var rawJobs = await _service.GetAllAsync();
            var locations = await _locationService.GetAllAsync();

            foreach (var job in rawJobs)
            {
                var senderNames = new List<string>();
                if (!string.IsNullOrEmpty(job.SenderLocationIds))
                {
                    var sIds = job.SenderLocationIds.Split(',').Select(long.Parse).ToList();
                    senderNames = locations.Where(l => sIds.Contains(l.RecId)).Select(l => l.Name).ToList();
                }

                var receiverNames = new List<string>();
                if (!string.IsNullOrEmpty(job.ReceiverLocationIds))
                {
                    var rIds = job.ReceiverLocationIds.Split(',').Select(long.Parse).ToList();
                    receiverNames = locations.Where(l => rIds.Contains(l.RecId)).Select(l => l.Name).ToList();
                }

                ConfigList.Add(new
                {
                    job.RecId,
                    job.Code,
                    job.SenderLocationIds,
                    job.ReceiverLocationIds,
                    job.TableNames,
                    SelectedSenders = string.Join(", ", senderNames),
                    SelectedReceivers = string.Join(", ", receiverNames),
                    SelectedTables = job.TableNames
                });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] List<JobSync> rows)
        {
            if (rows == null || rows.Count == 0) return new JsonResult(new { success = false, message = "No records selected." });
            var count = await _service.DeleteManyAsync(rows);
            return new JsonResult(new { success = true, message = $"Deleted {count} jobs." });
        }

        public async Task<IActionResult> OnPostExecuteJobAsync([FromBody] long selectedRecId)
        {
            var job = await _service.GetByRecIdAsync(selectedRecId);
            if (job == null) return new JsonResult(new { success = false, message = "Job not found!" });

            var locations = await _locationService.GetAllAsync();

            var senderIds = job.SenderLocationIds.Split(',').Select(long.Parse).ToList();
            var receiverIds = job.ReceiverLocationIds.Split(',').Select(long.Parse).ToList();
            var tables = job.TableNames.Split(',').ToList();

            var logBuilder = new StringBuilder();
            logBuilder.Append($"<h5><i class='fas fa-play text-success mr-2'></i><b>Executing Scheduler Job: {job.Code}</b></h5><hr/>");

            int successCount = 0;

            foreach (var senderId in senderIds)
            {
                var senderNode = locations.FirstOrDefault(l => l.RecId == senderId);
                if (senderNode == null) continue;

                foreach (var receiverId in receiverIds)
                {
                    var receiverNode = locations.FirstOrDefault(l => l.RecId == receiverId);
                    if (receiverNode == null || senderNode.RecId == receiverNode.RecId) continue;

                    string senderConn = BuildConnectionString(senderNode);
                    string receiverConn = BuildConnectionString(receiverNode);

                    var serverProvider = new SqlSyncProvider(senderConn);
                    var clientProvider = new SqlSyncProvider(receiverConn);

                    foreach (var table in tables)
                    {
                        try
                        {
                            string schemaName = "dbo"; // Default schema
                            string tableName = table.Trim();

                            // 1. Tách chuỗi nếu cấu hình chứa cả dạng "schema.tablename"
                            if (tableName.Contains("."))
                            {
                                var parts = tableName.Split('.');
                                schemaName = parts[0];
                                tableName = parts[1];
                            }
                            else
                            {
                                // 2. Tự động truy vấn tìm đúng Schema của bảng ở database gốc (Fix lỗi TAXDATA không tồn tại)
                                using (var conn = new SqlConnection(senderConn))
                                {
                                    await conn.OpenAsync();
                                    string findSchemaQuery = @"
                                        SELECT TOP 1 TABLE_SCHEMA 
                                        FROM INFORMATION_SCHEMA.TABLES 
                                        WHERE TABLE_NAME = @TableName 
                                        ORDER BY CASE WHEN TABLE_SCHEMA = 'ax' THEN 1 
                                                      WHEN TABLE_SCHEMA = 'dbo' THEN 2 
                                                      ELSE 3 END ASC";

                                    using (var cmdSchema = new SqlCommand(findSchemaQuery, conn))
                                    {
                                        cmdSchema.Parameters.AddWithValue("@TableName", tableName);
                                        var dbSchema = await cmdSchema.ExecuteScalarAsync();
                                        if (dbSchema != null && dbSchema != DBNull.Value)
                                        {
                                            schemaName = dbSchema.ToString()!;
                                        }
                                    }
                                }
                            }

                            var setup = new SyncSetup();
                            var setupTable = new SetupTable(tableName, schemaName) { SyncDirection = SyncDirection.DownloadOnly };
                            setup.Tables.Add(setupTable);

                            var agent = new SyncAgent(clientProvider, serverProvider);
                            string scopeName = $"Job_{job.Code}_{senderId}_{receiverId}_{schemaName}_{tableName}";

                            // Đảm bảo không vượt quá độ dài ký tự scope quy định của Dotmim
                            if (scopeName.Length > 100)
                            {
                                scopeName = "Scope_" + Guid.NewGuid().ToString().Substring(0, 8);
                            }

                            var result = await agent.SynchronizeAsync(scopeName, setup, SyncType.Normal, null).ConfigureAwait(false);

                            logBuilder.Append($"✅ <b>[{senderNode.Name}]</b> ➔ <b>[{receiverNode.Name}]</b> | Table: <i>{schemaName}.{tableName}</i> | Changes: <span class='badge badge-success'>{result.ChangesAppliedOnClient?.TotalAppliedChanges ?? 0}</span><br/>");
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            logBuilder.Append($"❌ <b>[{senderNode.Name}]</b> ➔ <b>[{receiverNode.Name}]</b> | Table: <i>{table}</i> | Error: <span class='text-danger'>{ex.Message}</span><br/>");
                        }
                    }
                }
            }

            return new JsonResult(new { success = successCount > 0, message = logBuilder.ToString() });
        }

        private string BuildConnectionString(dynamic locationEntity)
        {
            var connBuilder = new SqlConnectionStringBuilder
            {
                DataSource = locationEntity.Server ?? "",
                InitialCatalog = locationEntity.DatabaseName ?? "",
                UserID = locationEntity.Username ?? "",
                Password = locationEntity.Password ?? "",
                TrustServerCertificate = true,
                MultipleActiveResultSets = true
            };
            return connBuilder.ConnectionString;
        }
    }
}