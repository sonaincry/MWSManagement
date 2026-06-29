using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indotalent.DTOs
{
    public class ProductDto
    {
        [Column("Company Code")]
        [Display(Name = "Company Code")]
        [GridColumn(Width = 100)]
        public string? CompanyCode { get; set; }

        [Column("Product ID")]
        [Display(Name = "Product ID")]
        [GridColumn(Width = 100)]
        public string? ProductId { get; set; }

        [Column("Product Name")]
        [Display(Name = "Product Name")]
        [GridColumn(Width = 100)]
        public string? ProductName { get; set; }

        [Column("Category RecID")]
        [Display(Name = "Category RecID")]
        [GridColumn(Width = 100)]
        public long? CategoryRecId { get; set; }

        [Column("Category Code")]
        [Display(Name = "Category Code")]
        [GridColumn(Width = 100)]
        public string? CategoryCode { get; set; }

        [Column("Category Name")]
        [Display(Name = "Category Name")]
        [GridColumn(Width = 100)]
        public string? CategoryName { get; set; }

        [Column("Parent Category Name")]
        [Display(Name = "Parent Category Name")]
        [GridColumn(Width = 100)]
        public string? ParentCategoryName { get; set; }

        [Column("Category Hierarchy Name")]
        [Display(Name = "Category Hierarchy Name")]
        [GridColumn(Width = 100)]
        public string? CategoryHierarchyName { get; set; }
    }
}