// Models/DTOs/SalesByDateReportDTO.cs
namespace Indotalent.Models.DTOs
{
    public class SalesByDateReportDTO
    {
        public DateTime SalesDate { get; set; }
        public int NoOfTransactions { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AvgSalesPerTransaction { get; set; }
        public int UniqueStaff { get; set; }
    }
}