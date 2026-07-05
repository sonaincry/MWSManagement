using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;

namespace Indotalent.Models.Entities.AX
{
    public class EcoResProductListAX
    {
        [Display(Name = "RecId")]
        [GridColumn(Width = 120, IsPrimaryKey = true, Visible = false, AllowEditing = false)]
        public long RecId { get; set; }

        [Display(Name = "Product Number")]
        [GridColumn(Width = 140)]
        public string? DisplayProductNumber { get; set; }

        [Display(Name = "Name")]
        [GridColumn(Width = 300)]
        public string? SearchName { get; set; }

        [Display(Name = "Type")]
        [GridColumn(Width = 100)]
        public int ProductType { get; set; }

        [Display(Name = "Category")]
        [GridColumn(Width = 200)]
        public string? CategoryName { get; set; }
    }
}