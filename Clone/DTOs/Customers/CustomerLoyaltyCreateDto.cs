namespace MWSManagement.DTOs.Customers
{
    public class CustomerLoyaltyCreateDto
    {
        public string CardNumber { get; set; } = string.Empty;
        public string? CustomerAccountNum { get; set; }
        public string? CustomerName { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? MobilePhone { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? FullAddress { get; set; }
        public int CardTenderType { get; set; } = 0;
    }
}
