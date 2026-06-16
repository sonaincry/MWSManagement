using Microsoft.AspNetCore.Mvc.Rendering;

namespace Indotalent.Pages.Reports
{
    public class ReportFilterViewModel
    {
        public string StoreId { get; set; } = "ALL";

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? StaffKeyword { get; set; }

        public List<SelectListItem> StoreOptions { get; set; } = new();
    }
}