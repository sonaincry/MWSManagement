namespace MWSManagement.DTOs.Customers
{
    public class CustomerLoyaltyCreateResultDto
    {
        public string AccountNum { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public long CardRecId { get; set; }
    }
}
