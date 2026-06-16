using System;

namespace Indotalent.Models.DTOs
{
    public class RetailTransactionDTO
    {
        public string TransactionId { get; set; } = string.Empty;
        public int Type { get; set; }
        public string? ReceiptId { get; set; }
        public string Store { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public string? Staff { get; set; }
        public DateTime TransDate { get; set; }
        public string TransDateSearch { get; set; } = "";
        public decimal NetAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public int NumberOfItems { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}