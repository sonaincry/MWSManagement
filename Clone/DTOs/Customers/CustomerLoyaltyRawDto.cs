namespace MWSManagement.DTOs.Customers
{
    public class CustomerLoyaltyRawDto
    {
        public long CardRecId { get; set; }
        public string? CardNumber { get; set; }
        public string? CardTypeLabel { get; set; }
        public string? AccountNum { get; set; }
        public string? CustomerName { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? MobilePhone { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? FullAddress { get; set; }
    }
}
