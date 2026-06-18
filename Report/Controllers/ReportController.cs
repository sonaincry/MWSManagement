using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Report;
using Report.Reports;

[Route("reports")]
public class ReportController : Controller
{
    [HttpGet("staff-sales/pdf")]
    public IActionResult StaffPdf(string staffId, string startDate, string endDate)
    {
        var report = new StaffReport();
        report.Parameters["staffId"].Value = staffId;
        report.Parameters["startDate"].Value = DateTime.Parse(startDate);
        report.Parameters["endDate"].Value = DateTime.Parse(endDate);

        var ms = new MemoryStream();
        report.ExportToPdf(ms);
        ms.Position = 0;
        return File(ms, "application/pdf", "StaffSales.pdf");
    }

    [HttpGet("staff-sales/excel")]
    public IActionResult StaffExcel(string staffId, string startDate, string endDate)
    {
        var report = new StaffReport();
        report.Parameters["staffId"].Value = staffId;
        report.Parameters["startDate"].Value = DateTime.Parse(startDate);
        report.Parameters["endDate"].Value = DateTime.Parse(endDate);

        var ms = new MemoryStream();
        report.ExportToXlsx(ms);
        ms.Position = 0;
        return File(ms,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "StaffSales.xlsx");
    }

    [HttpGet("daily-sales/pdf")]
    public IActionResult DailyPdf(string store, string startDate, string endDate)
    {
        var report = new DailyReport();
        report.Parameters["storeId"].Value = store;
        report.Parameters["startDate"].Value = DateTime.Parse(startDate);
        report.Parameters["endDate"].Value = DateTime.Parse(endDate);

        var ms = new MemoryStream();
        report.ExportToPdf(ms);
        ms.Position = 0;
        return File(ms, "application/pdf", "DailySales.pdf");
    }

    [HttpGet("daily-sales/excel")]
    public IActionResult DailyExcel(string store, string startDate, string endDate)
    {
        var report = new DailyReport();
        report.Parameters["storeId"].Value = store;
        report.Parameters["startDate"].Value = DateTime.Parse(startDate);
        report.Parameters["endDate"].Value = DateTime.Parse(endDate);

        var ms = new MemoryStream();
        report.ExportToXlsx(ms);
        ms.Position = 0;
        return File(ms,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "DailySales.xlsx");
    }
}