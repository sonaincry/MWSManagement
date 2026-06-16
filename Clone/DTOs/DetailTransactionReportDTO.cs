
namespace MWSManagement.Models.DTOs
{
    public class DetailTransactionReportDTO
    {
        public string TransactionId { get; set; } = string.Empty;
        public int Type { get; set; }
        public string ReceiptId { get; set; } = string.Empty;
        public string Store { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public string Staff { get; set; } = string.Empty;
        public DateTime TransDate { get; set; }
        public string TransTime_Formatted { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public string LoyaltyCardId { get; set; } = string.Empty;
    }
}