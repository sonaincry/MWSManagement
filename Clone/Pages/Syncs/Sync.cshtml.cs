using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MWSManagement.Applications.Locations;
using MWSManagement.Applications.TableSyncs;
using MWSManagement.DTOs;
using Microsoft.Data.SqlClient;
using Indotalent.AppSettings;

namespace Indotalent.Pages.Syncs
{
    public class SyncPortalModel : PageModel
    {
        private readonly LocationService _locService;
        private readonly TableSyncService _tableSyncService;
        public SyncPortalModel(LocationService locService, TableSyncService tableSyncService)
        {
            _locService = locService;
            _tableSyncService = tableSyncService;
        }

        private readonly List<string> _availableTables = new()
        {
            "ax.ECORESCATEGORY",
            "ax.INVENTTABLE",
            "ax.RETAILCHANNELTABLE"
        };

        public List<SelectListItem> LocationOptions { get; set; } = new();
        public List<string> AvailableTables => _availableTables;

        [BindProperty] public int SenderLocationId { get; set; }
        [BindProperty] public int ReceiverLocationId { get; set; }

        public string? SyncMessage { get; set; }
        public bool IsSuccess { get; set; } = false;

        public List<TableSyncDto> ConfiguredTables { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadDropdownOptionsAsync();
        }

        private async Task LoadDropdownOptionsAsync()
        {
            var locs = await _locService.GetAllLocationsAsync();
            LocationOptions = locs.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            }).ToList();

            var allConfigs = await _tableSyncService.GetConfiguredTablesAsync();

            // Chỉ lấy các bảngIsActive true
            ConfiguredTables = allConfigs.Where(x => x.IsActive).ToList();
        }

        public async Task<IActionResult> OnPostSyncDataAsync(string[] selectedTables)
        {
            await LoadDropdownOptionsAsync();

            if (selectedTables == null || selectedTables.Length == 0)
            {
                SyncMessage = "Please select at least 1 table";
                return Page();
            }

            if (SenderLocationId == ReceiverLocationId)
            {
                SyncMessage = "Sender & Receiver should be different";
                return Page();
            }

            try
            {
                var setup = new SyncSetup(selectedTables);

                foreach (var table in setup.Tables)
                {
                    var currentConfig = ConfiguredTables.FirstOrDefault(x => x.TableName == table.TableName);
                    int actionEnum = currentConfig != null ? currentConfig.SyncAction : 1;

                    if (actionEnum == 1)
                    {
                        table.SyncDirection = SyncDirection.DownloadOnly;
                    }
                    else if (actionEnum == 2)
                    {
                        table.SyncDirection = SyncDirection.DownloadOnly;
                    }
                    else if (actionEnum == 3)
                    {
                        table.SyncDirection = SyncDirection.DownloadOnly;
                    }
                }

                var senderLoc = await _locService.GetLocationByIdAsync(SenderLocationId);
                var receiverLoc = await _locService.GetLocationByIdAsync(ReceiverLocationId);

                if (senderLoc == null || receiverLoc == null)
                {
                    SyncMessage = "Can't find location.";
                    return Page();
                }

                string decryptedSenderConn = senderLoc.ConnectionString;
                string decryptedReceiverConn = receiverLoc.ConnectionString;

                try
                {
                    if (!string.IsNullOrEmpty(senderLoc.ConnectionString))
                    {
                        decryptedSenderConn = ConnectionHelper.Decrypt(senderLoc.ConnectionString);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                try
                {
                    if (!string.IsNullOrEmpty(receiverLoc.ConnectionString))
                    {
                        decryptedReceiverConn = ConnectionHelper.Decrypt(receiverLoc.ConnectionString);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                var serverProvider = new SqlSyncProvider(decryptedSenderConn);
                var clientProvider = new SqlSyncProvider(decryptedReceiverConn);
                var syncOptions = new SyncOptions();

                var agent = new SyncAgent(clientProvider, serverProvider, syncOptions);
                string scopeName = string.Join("_", selectedTables).Replace(".", "_");

                var syncResult = await agent.SynchronizeAsync(scopeName, setup, SyncType.Normal, null).ConfigureAwait(false);

                IsSuccess = true;
                var totalDownloaded = syncResult.ChangesAppliedOnClient != null ? syncResult.ChangesAppliedOnClient.TotalAppliedChanges : 0;
                var duration = syncResult.CompleteTime - syncResult.StartTime;

                SyncMessage = $"<b>Sync data successfully!</b><br/>" +
                              $"From [{senderLoc.Name}] to [{receiverLoc.Name}]<br/>" +
                              $"Scope name: {scopeName}<br/>" +
                              $"Duration: {duration:hh\\:mm\\:ss}<br/>" +
                              $"Total data affect: {totalDownloaded}";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                SyncMessage = $"Sync data failed!: {ex.Message}";
            }

            return Page();
        }
    }
}