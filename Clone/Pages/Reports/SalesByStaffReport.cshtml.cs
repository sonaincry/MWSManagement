// Pages/Reports/SalesByStaffReport.cshtml.cs
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.DTOs;
using Indotalent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Indotalent.Pages.Reports
{
    [Authorize]
    public class SalesByStaffReportModel : PageModel
    {
        private readonly SalesReportService _reportService;

        public SalesByStaffReportModel(SalesReportService reportService)
        {
            _reportService = reportService;
        }

        [BindProperty(SupportsGet = true)]
        public string StoreId { get; set; } = "ALL";

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public string? StaffKeyword { get; set; }
        public List<SalesByStaffReportDTO> ReportData { get; set; } = new();
        public bool HasSearched { get; set; } = false;

        public decimal GrandTotalSales => ReportData.Sum(x => x.SalesAmount);
        public int GrandTotalTransactions => ReportData.Sum(x => x.NoOfTransactions);

        public List<SelectListItem> StoreOptions => new()
        {
            new SelectListItem("All Stores", "ALL"),
            new SelectListItem("Store 1001", "1001"),
            // thêm store mới vào đây
        };

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.SetupViewDataTitleFromUrl();
            HasSearched = true;

            ReportData = await _reportService.GetSalesByStaffAsync(
                StoreId, StartDate, EndDate, StaffKeyword);

            return Page();
        }
    }
}