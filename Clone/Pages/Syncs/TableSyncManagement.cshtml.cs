using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MWSManagement.Applications.TableSyncs;
using MWSManagement.DTOs;

namespace MWSManagement.Pages.Syncs
{
    public class TableSyncManagementModel : PageModel
    {
        private readonly TableSyncService _tableSyncService;

        public TableSyncManagementModel(TableSyncService tableSyncService)
        {
            _tableSyncService = tableSyncService;
        }

        public List<TableSyncDto> ConfiguredTables { get; set; } = new();
        public List<SelectListItem> DatabaseTablesOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            ConfiguredTables = await _tableSyncService.GetConfiguredTablesAsync();
            DatabaseTablesOptions = await _tableSyncService.GetDatabaseTablesOptionsAsync();
        }

        public async Task<IActionResult> OnPostSaveAsync([FromForm] TableSyncDto input)
        {
            if (string.IsNullOrEmpty(input.TableName)) return RedirectToPage();

            await _tableSyncService.SaveTableSyncConfigAsync(input);

            return RedirectToPage();
        }
    }
}