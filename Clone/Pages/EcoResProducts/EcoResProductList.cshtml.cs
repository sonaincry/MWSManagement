using Indotalent.Applications.EcoResProducts;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.ControlUI.Helper.Grids;

namespace Indotalent.Pages.EcoResProducts
{
    public class EcoResProductListModel : PageModel
    {
        private readonly EcoResProductService _service;

        public EcoResProductListModel(EcoResProductService service)
        {
            _service = service;
        }

        public List<EcoResProductListDto> ProductList { get; set; } = new();
        public List<GridColumnDto> GridColumns { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            GridColumns = GridColumnHelper.FromModel<EcoResProductListDto>();
            ProductList = await _service.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] List<EcoResProductListDto> rows)
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