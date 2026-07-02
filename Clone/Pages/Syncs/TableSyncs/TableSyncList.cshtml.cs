using Indotalent.Applications.TableSyncs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.ControlUI.Helper.Grids;
using MWSManagement.Models.Entities;

namespace MWSManagement.Pages.Syncs.TableSyncs
{
    public class TableSyncListModel : PageModel
    {
        private readonly TableSyncService _service;

        public TableSyncListModel(TableSyncService service)
        {
            _service = service;
        }

        public List<TableSync> ConfigList { get; set; } = new();
        public List<GridColumnDto> GridColumns { get; set; } = new();
        [TempData] public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            GridColumns = GridColumnHelper.FromModel<TableSync>();
            ConfigList = await _service.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] List<TableSync> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return new JsonResult(new { success = false, message = "No selected row(s)." });
            }

            try
            {
                var deletedCount = await _service.DeleteManyAsync(rows);
                return new JsonResult(new { success = true, message = $"Deleted {deletedCount} row(s)." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
