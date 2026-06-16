using System.ComponentModel.DataAnnotations.Schema;

namespace Indotalent.Models.Entities
{
    [Table("RETAILTRANSACTIONTABLE")]
    public class RetailTransaction
    {
        [Column("TRANSACTIONID")]
        public string TransactionId { get; set; } = string.Empty;

        [Column("TYPE")]
        public int Type { get; set; }

        [Column("RECEIPTID")]
        public string? ReceiptId { get; set; }

        [Column("STORE")]
        public string Store { get; set; } = string.Empty;

        [Column("TERMINAL")]
        public string Terminal { get; set; } = string.Empty;

        [Column("STAFF")]
        public string? Staff { get; set; }

        [Column("TRANSDATE")]
        public DateTime TransDate { get; set; }

        [Column("NETAMOUNT")]
        public decimal NetAmount { get; set; }

        [Column("GROSSAMOUNT")]
        public decimal GrossAmount { get; set; }

        [Column("PAYMENTAMOUNT")]
        public decimal PaymentAmount { get; set; }

        [Column("NUMBEROFITEMS")]
        public decimal NumberOfItems { get; set; }

        [Column("CREATEDDATE")]
        public DateTime CreatedDate { get; set; }
    }
}