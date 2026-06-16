using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MWSManagement.Pages.Reports
{
    [Authorize]
    public class ReportFilterModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string StoreId { get; set; } = "ALL";  // default

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<SelectListItem> StoreOptions => new()
{
    new SelectListItem("All Stores", "ALL"),
    new SelectListItem("Store 1001", "1001"),
};


        public void OnGet()
        {
        }
    }
}
