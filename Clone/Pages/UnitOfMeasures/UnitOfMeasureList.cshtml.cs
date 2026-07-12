using Indotalent.Applications.UnitOfMeasures;
using Indotalent.DTOs;
using MWSManagement.ControlUI.Helper.Grids;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.UnitOfMeasures
{
    public class UnitOfMeasureListModel : PageModel
    {
        private readonly UnitMeasureService _service;

        public UnitOfMeasureListModel(UnitMeasureService service)
        {
            _service = service;
        }

        public List<UnitOfMeasureDto> UnitList { get; set; } = new();
        public List<GridColumnDto> GridColumns { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            GridColumns = GridColumnHelper.FromModel<UnitOfMeasureDto>();
            UnitList = await _service.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] List<UnitOfMeasureDto> rows)
        {
            if (rows == null || rows.Count == 0)
                return new JsonResult(new { success = false, message = "No selected row(s)." });

            try
            {
                var count = await _service.DeleteManyAsync(rows);
                return new JsonResult(new { success = true, message = $"Deleted {count} row(s)." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}