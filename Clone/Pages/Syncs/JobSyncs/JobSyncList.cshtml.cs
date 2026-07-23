using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Indotalent.Applications.JobSyncs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MWSManagement.Applications.JobLogs;
using MWSManagement.Applications.Locations;
using MWSManagement.ControlUI.Helper.Grids;
using MWSManagement.Models;
using MWSManagement.Models.Entities;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace MWSManagement.Pages.Syncs.JobSyncs
{
    public class JobSyncListModel : PageModel
    {
        private readonly JobSyncService _service;
        private readonly LocationService _locationService;
        private readonly JobLogService _jobLogService;

        public JobSyncListModel(JobSyncService service, LocationService locationService, JobLogService jobLogService)
        {
            _service = service;
            _locationService = locationService;
            _jobLogService = jobLogService;
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
            var tables = job.TableNames.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();

            var logBuilder = new StringBuilder();
            var databaseDescriptionBuilder = new StringBuilder();

            logBuilder.Append($"<div class='mb-2 pb-2 border-bottom'><h5 class='text-dark font-weight-bold'><i class='fas fa-terminal mr-2 text-secondary'></i>Executing Scheduler Job: {job.Code}</h5></div>");

            int successCount = 0;
            int totalOperations = 0;
            bool ultimateSuccess = true;

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

                    foreach (var rawTable in tables)
                    {
                        totalOperations++;
                        string schemaName = "dbo";
                        string tableName = rawTable;

                        if (tableName.Contains(":"))
                        {
                            tableName = tableName.Split(':')[0].Trim();
                        }

                        try
                        {
                            if (tableName.Contains("."))
                            {
                                var parts = tableName.Split('.');
                                schemaName = parts[0];
                                tableName = parts[1];
                            }
                            else
                            {
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
                            var decimalFacets = new Dictionary<string, (byte Precision, byte Scale)>(StringComparer.OrdinalIgnoreCase);

                            using (var conn = new SqlConnection(senderConn))
                            {
                                await conn.OpenAsync();
                                var cmd = new SqlCommand(@"
                                    SELECT COLUMN_NAME, NUMERIC_PRECISION, NUMERIC_SCALE
                                    FROM INFORMATION_SCHEMA.COLUMNS
                                    WHERE TABLE_NAME = @t AND DATA_TYPE IN ('decimal','numeric')", conn);
                                cmd.Parameters.AddWithValue("@t", tableName);
                                using var reader = await cmd.ExecuteReaderAsync();
                                while (await reader.ReadAsync())
                                {
                                    byte precision = Convert.ToByte(reader.GetValue(1));
                                    byte scale = Convert.ToByte(reader.GetValue(2));
                                    decimalFacets[reader.GetString(0)] = (precision, scale);
                                }
                            }

                            void FixDecimalParams(System.Data.Common.DbCommand command)
                            {
                                if (command is SqlCommand sqlCmd)
                                {
                                    foreach (SqlParameter p in sqlCmd.Parameters)
                                    {
                                        if (p.SqlDbType == SqlDbType.Decimal)
                                        {
                                            logBuilder.Append($"<div style='font-size:11px;color:#888'>[DEBUG] Cmd param {p.ParameterName} Precision={p.Precision} Scale={p.Scale} Value={p.Value}</div>");

                                            var colName = p.ParameterName.TrimStart('@');
                                            if (decimalFacets.TryGetValue(colName, out var facet))
                                            {
                                                p.Precision = facet.Precision;
                                                p.Scale = facet.Scale;
                                            }
                                            else
                                            {
                                                p.Precision = 38;
                                                p.Scale = 10;
                                            }
                                        }
                                    }
                                }
                            }

                            agent.LocalOrchestrator.OnGetCommand(args => FixDecimalParams(args.Command));
                            agent.RemoteOrchestrator.OnGetCommand(args => FixDecimalParams(args.Command));
                            string scopeName = $"Job_{job.Code}_{senderId}_{receiverId}_{schemaName}_{tableName}";

                            if (scopeName.Length > 100)
                            {
                                scopeName = "Scope_" + Guid.NewGuid().ToString().Substring(0, 8);
                            }

                            var result = await agent.SynchronizeAsync(scopeName, setup, SyncType.Normal, null).ConfigureAwait(false);

                            // UI Version with HTML
                            logBuilder.Append($"<div class='py-1 text-muted' style='font-family: monospace;'><span class='text-success font-weight-bold'>[OK]</span> [{senderNode.Name}] ➔ [{receiverNode.Name}] | Table: {schemaName}.{tableName} | Changes: {result.ChangesAppliedOnClient?.TotalAppliedChanges ?? 0}</div>");

                            // Database Version without HTML (Appends lines separated by a newline break)
                            databaseDescriptionBuilder.AppendLine($"[OK] [{senderNode.Name}] ➔ [{receiverNode.Name}] | Table: {schemaName}.{tableName} | Changes: {result.ChangesAppliedOnClient?.TotalAppliedChanges ?? 0}");

                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            ultimateSuccess = false;

                            logBuilder.Append($"<div class='p-2 my-2 rounded border bg-light text-dark' style='font-family: monospace; font-size: 13px; border-left: 4px solid #6c757d !important;'>");
                            logBuilder.Append($"<b class='text-secondary'>[ERROR]</b> [{senderNode.Name}] ➔ [{receiverNode.Name}] | Table: {tableName}<br/>");
                            logBuilder.Append($"<span class='text-dark d-block mt-1' style='white-space: pre-wrap;'>Message: {ex.Message}</span>");
                            if (ex.InnerException != null)
                            {
                                logBuilder.Append($"<span class='text-muted d-block mt-1' style='font-size:11px;'>Inner Details: {ex.InnerException.Message}</span>");
                            }
                            logBuilder.Append($"</div>");

                            databaseDescriptionBuilder.AppendLine($"[ERROR] [{senderNode.Name}] ➔ [{receiverNode.Name}] | Table: {tableName} | Message: {ex.Message}");
                        }
                    }
                }
            }

            string operationalStatus = "Success";
            if (successCount == 0 && totalOperations > 0) operationalStatus = "Failed";
            else if (!ultimateSuccess) operationalStatus = "Warning";

            try
            {
                var persistentLog = new JobLog
                {
                    JobRecId = job.RecId,
                    JobCode = job.Code,
                    RunDate = DateTime.Today,
                    RunTime = DateTime.Now.TimeOfDay,
                    Status = operationalStatus,
                    Description = databaseDescriptionBuilder.ToString().Trim() // Saves perfectly clean plain text lines!
                };

                await _jobLogService.CreateAsync(persistentLog);
            }
            catch (Exception dbLogEx)
            {
                var errorMsg = dbLogEx.InnerException != null ? dbLogEx.InnerException.Message : dbLogEx.Message;
                logBuilder.Append($"<div class='mt-2 pt-2 border-top text-muted font-weight-bold' style='font-size:12px;'><i class='fas fa-info-circle mr-1'></i> System Note: Run tracked but log string context optimized ({errorMsg})</div>");
            }

            return new JsonResult(new { success = (operationalStatus != "Failed"), message = logBuilder.ToString() });
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