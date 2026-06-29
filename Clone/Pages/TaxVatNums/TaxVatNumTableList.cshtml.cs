using Indotalent.Applications.TaxVatNums;
using Indotalent.Models.Entities.AX;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.ControlUI.Helper.Grids;

namespace Indotalent.Pages.TaxVatNums
{
    public class TaxVatNumTableListModel : PageModel
    {
        private readonly TaxVatNumTableService _service;

        public TaxVatNumTableListModel(TaxVatNumTableService service)
        {
            _service = service;
        }

        public List<TaxVatNumTableAX> TaxVatNumList { get; set; } = new();

        public List<GridColumnDto> GridColumns { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            GridColumns = GridColumnHelper.FromModel<TaxVatNumTableAX>();
            TaxVatNumList = await _service.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] List<TaxVatNumTableAX> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No selected row(s)."
                });
            }

            try
            {
                var deletedCount = await _service.DeleteManyAsync(rows);

                return new JsonResult(new
                {
                    success = true,
                    message = $"Deleted {deletedCount} row(s)."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}