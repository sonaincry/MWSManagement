using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;

namespace MWSManagement.DTOs
{
    /// <summary>
    /// VIEW [crt].[RETAILTRANSACTIONSVIEW]
    /// </summary>

    public class TransDetailDto
    {
        public string? CHANNEL { get; set; }
        public string? STORE { get; set; }
        public DateTime? TRANSDATE { get; set; }
        public string? TRANSACTIONID { get; set; }
        public string? ITEMID { get; set; }
        public string? PRODUCT { get; set; }
        public string? PRODUCTNAME { get; set; }
        public string? CUSTACCOUNT { get; set; }
        public int? TYPE { get; set; }
        public decimal QTY { get; set; }
        public decimal NETAMOUNT { get; set; }
        public decimal TAXAMOUNT { get; set; }
        public decimal PAYMENTAMOUNT { get; set; }
        public Guid? RowGuid { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
        public int? Id { get; set; }

    }
}
