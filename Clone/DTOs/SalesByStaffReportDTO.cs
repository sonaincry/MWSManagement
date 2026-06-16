public class SalesByStaffReportDTO
{
    public string StaffId { get; set; } = string.Empty;

    public string StaffName { get; set; } = string.Empty;

    public int NoOfTransactions { get; set; }

    public decimal SalesAmount { get; set; }

    public decimal AvgSalesAmount { get; set; }
}