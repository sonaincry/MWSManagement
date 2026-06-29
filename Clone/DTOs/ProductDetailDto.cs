using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWSManagement.DTOs
{
    public class ProductDetailDto
    {
        [Display(Name = "ProductId")]
        [GridColumn(Width = 100)]
        [Column("Product ID")]
        public string? ProductId { get; set; }

        [Display(Name = "ProductName")]
        [GridColumn(Width = 100)]
        [Column("Product Name")]
        public string? ProductName { get; set; }

        [Display(Name = "ProductDescription")]
        [GridColumn(Width = 100)]
        [Column("Product Description")]
        public string? ProductDescription { get; set; }

        [Display(Name = "SalesPrice")]
        [GridColumn(Width = 100)]
        [Column("Sales Price")]
        public decimal SalesPrice { get; set; }

        [Display(Name = "UnitOfMeasure")]
        [GridColumn(Width = 100)]
        [Column("Unit of Measure")]
        public string? UnitOfMeasure { get; set; }

        [Display(Name = "Barcode")]
        [GridColumn(Width = 100)]
        [Column("Barcode")]
        public string? Barcode { get; set; }

        [Display(Name = "CompanyCode")]
        [GridColumn(Width = 100)]
        [Column("Company Code")]
        public string? CompanyCode { get; set; }
    }
}
