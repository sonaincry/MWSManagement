using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MWSManagement.Applications.Locations;
using Microsoft.Data.SqlClient;
using Indotalent.AppSettings;
using Indotalent.Applications.TableSyncs;
using MWSManagement.Models.Entities;
using Indotalent.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace Indotalent.Pages.Syncs
{
    public class SyncPortalModel : PageModel
    {
        private readonly LocationService _locService;
        private readonly TableSyncService _tableSyncService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SyncPortalModel(
            LocationService locService,
            TableSyncService tableSyncService,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _locService = locService;
            _tableSyncService = tableSyncService;
            _context = context;
            _configuration = configuration;
        }

        public List<SelectListItem> LocationOptions { get; set; } = new();
        public List<TableSync> ConfiguredTables { get; set; } = new();
        [BindProperty(Name = "SenderLocationId")] public long SenderLocationId { get; set; }
        [BindProperty(Name = "ReceiverLocationId")] public long ReceiverLocationId { get; set; }

        public string? SyncMessage { get; set; }
        public bool IsSuccess { get; set; } = false;

        public async Task OnGetAsync()
        {
            await LoadSyncPortalDataAsync();
        }

        private async Task LoadSyncPortalDataAsync()
        {
            var locs = await _locService.GetAllAsync();
            LocationOptions = locs.Select(l => new SelectListItem
            {
                Value = l.RecId.ToString(),
                Text = l.Name
            }).ToList();

            var allConfigs = await _tableSyncService.GetAllAsync();
            ConfiguredTables = allConfigs.Where(x => x.IsActive).ToList();
        }

        public async Task<IActionResult> OnPostSyncDataAsync(string[] selectedTables)
        {
            var senderKey = Request.Form.Keys.FirstOrDefault(k => k.Equals("SenderLocationId", StringComparison.OrdinalIgnoreCase));
            var receiverKey = Request.Form.Keys.FirstOrDefault(k => k.Equals("ReceiverLocationId", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(senderKey) && long.TryParse(Request.Form[senderKey], out long sId)) { this.SenderLocationId = sId; }
            if (!string.IsNullOrEmpty(receiverKey) && long.TryParse(Request.Form[receiverKey], out long rId)) { this.ReceiverLocationId = rId; }

            await LoadSyncPortalDataAsync();

            if (selectedTables == null || selectedTables.Length == 0)
            {
                SyncMessage = "Please select table to sync!";
                return Page();
            }

            if (SenderLocationId == 0 || ReceiverLocationId == 0)
            {
                SyncMessage = $"Select sender and receiver!";
                return Page();
            }

            if (SenderLocationId == ReceiverLocationId)
            {
                SyncMessage = "Sender && Receiver cant be the same!";
                return Page();
            }

            string finalSenderConn = "";
            string finalReceiverConn = "";
            Location? senderLoc = null;
            Location? receiverLoc = null;

            try
            {
                senderLoc = await _context.Set<Location>().FirstOrDefaultAsync(x => x.RecId == SenderLocationId);
                receiverLoc = await _context.Set<Location>().FirstOrDefaultAsync(x => x.RecId == ReceiverLocationId);

                if (senderLoc == null || receiverLoc == null)
                {
                    SyncMessage = "Cant find location";
                    return Page();
                }

                string BuildConnStringFromLocation(Location loc)
                {
                    var builder = new SqlConnectionStringBuilder();
                    builder.DataSource = loc.Server ?? "";
                    builder.InitialCatalog = loc.DatabaseName ?? "";
                    builder.UserID = loc.Username ?? "";
                    builder.Password = loc.Password ?? "";
                    builder.TrustServerCertificate = true;
                    builder.MultipleActiveResultSets = true;
                    return builder.ConnectionString;
                }

                finalSenderConn = BuildConnStringFromLocation(senderLoc);
                finalReceiverConn = BuildConnStringFromLocation(receiverLoc);

                var setup = new SyncSetup();

                using (var conn = new SqlConnection(finalSenderConn))
                {
                    await conn.OpenAsync();
                    foreach (var tableString in selectedTables)
                    {
                        string schemaName = "dbo";
                        string tableName = tableString;

                        if (tableString.Contains("."))
                        {
                            var parts = tableString.Split('.');
                            schemaName = parts[0];
                            tableName = parts[1];
                        }
                        else
                        {
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

                        string checkQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @SchemaName AND TABLE_NAME = @TableName";
                        using (var cmd = new SqlCommand(checkQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SchemaName", schemaName);
                            cmd.Parameters.AddWithValue("@TableName", tableName);
                            int tableExists = (int)await cmd.ExecuteScalarAsync();
                            if (tableExists == 0)
                            {
                                IsSuccess = false;
                                SyncMessage = $"<b>Sync error!</b><br/>❌ Error: Table <b>[{schemaName}.{tableName}]</b> not found in database <b>[{senderLoc.Name}]</b>.";
                                return Page();
                            }
                        }

                        var setupTable = new SetupTable(tableName, schemaName);
                        var currentConfig = ConfiguredTables.FirstOrDefault(x => x.TableName == tableString || x.TableName == tableName);
                        int actionEnum = currentConfig != null ? currentConfig.SyncAction : 1;

                        if (actionEnum == 1) { setupTable.SyncDirection = SyncDirection.DownloadOnly; }
                        else if (actionEnum == 2) { setupTable.SyncDirection = SyncDirection.Bidirectional; }
                        else if (actionEnum == 3) { setupTable.SyncDirection = SyncDirection.DownloadOnly; }

                        setup.Tables.Add(setupTable);
                    }
                }

                var serverProvider = new SqlSyncProvider(finalSenderConn);
                var clientProvider = new SqlSyncProvider(finalReceiverConn);

                var syncOptions = new SyncOptions();
                var agent = new SyncAgent(clientProvider, serverProvider, syncOptions);

                string scopeName = string.Join("_", selectedTables).Replace(".", "_");
                if (scopeName.Length > 100)
                {
                    scopeName = "Scope_" + Guid.NewGuid().ToString().Substring(0, 8);
                }

                var syncResult = await agent.SynchronizeAsync(scopeName, setup, SyncType.Normal, null).ConfigureAwait(false);

                IsSuccess = true;
                var totalDownloaded = syncResult.ChangesAppliedOnClient != null ? syncResult.ChangesAppliedOnClient.TotalAppliedChanges : 0;
                var duration = syncResult.CompleteTime - syncResult.StartTime;

                SyncMessage = $"<b>Synchronize successfully!</b><br/>" +
                              $"From: <b>[{senderLoc.Name}]</b> To: <b>[{receiverLoc.Name}]</b><br/>" +
                              $"Duration: {duration:hh\\:mm\\:ss}<br/>" +
                              $"Total affected: <span class='badge badge-info'>{totalDownloaded}</span>";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                SyncMessage = $"<b>Sync failed!</b><br/>" +
                              $"❌ <b>Log Error:</b> {ex.Message}<br/><br/>";
            }

            return Page();
        }
    }
}