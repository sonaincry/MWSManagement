// Pages/Reports/SalesByDateReport.cshtml.cs
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
    public class SalesByDateReportModel : PageModel
    {
        private readonly SalesReportService _reportService;

        public SalesByDateReportModel(SalesReportService reportService)
        {
            _reportService = reportService;
        }

        // --- Filter inputs ---
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
        // --- Result ---
        public List<SalesByDateReportDTO> ReportData { get; set; } = new();
        public bool HasSearched { get; set; } = false;



        // Summary
        public decimal GrandTotalSales => ReportData.Sum(x => x.TotalSales);
        public int GrandTotalTransactions => ReportData.Sum(x => x.NoOfTransactions);

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.SetupViewDataTitleFromUrl();
            HasSearched = true;

            // ChannelId map theo store — bạn điều chỉnh sau
            long channelId = 5637145326;

            ReportData = await _reportService.GetSalesByDateAsync(
    StoreId, StartDate, EndDate);

            return Page();
        }
    }
}