// Path: Pages/Reports/DailySalesReport.cshtml.cs
using Indotalent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.Applications.Lookups;
using MWSManagement.Models.DTOs;

namespace MWSManagement.Pages.Reports
{
    [Authorize]
    public class DailySalesReportModel : PageModel
    {
        private readonly LookupService _lookupService;
        private readonly SalesReportService _reportService;

        public DailySalesReportModel(SalesReportService reportService, LookupService lookupService)
        {
            _reportService = reportService;
            _lookupService = lookupService;
        }

        [BindProperty(SupportsGet = true)]
        public string? StoreId { get; set; } = "1001";

        public List<LookupItem> Stores { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? TerminalId { get; set; } = "1001T1";

        [BindProperty(SupportsGet = true)]
        public string? TransId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ReceiptId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public int Type { get; set; } = 2;

        public List<DetailTransactionReportDTO> ReportData { get; set; } = new();
        public bool HasSearched { get; set; } = false;

        public async Task OnGetAsync()
        {
            Stores = await _lookupService.GetStoresAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            HasSearched = true;
            ReportData = await _reportService.GetRetailTransactionsAsync(
                TransId, ReceiptId, StoreId, TerminalId, StartDate, EndDate, Type);

            Stores = await _lookupService.GetStoresAsync();
            return Page();  
        }
    }
}