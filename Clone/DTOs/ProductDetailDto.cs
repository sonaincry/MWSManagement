namespace MWSManagement.DTOs
{
    public class ProductDetailDto
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal SalesPrice { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? Barcode { get; set; }
        public string? CompanyCode { get; set; }
    }
}
