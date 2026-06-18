using System.ComponentModel.DataAnnotations.Schema;

namespace Indotalent.DTOs
{
    public class ProductDto
    {
        [Column("Company Code")]
        public string? CompanyCode { get; set; }

        [Column("Product ID")]
        public string? ProductId { get; set; }

        [Column("Product Name")]
        public string? ProductName { get; set; }

        [Column("Category RecID")]
        public long? CategoryRecId { get; set; }

        [Column("Category Code")]
        public string? CategoryCode { get; set; }

        [Column("Category Name")]
        public string? CategoryName { get; set; }

        [Column("Parent Category Name")]
        public string? ParentCategoryName { get; set; }

        [Column("Category Hierarchy Name")]
        public string? CategoryHierarchyName { get; set; }
    }
}