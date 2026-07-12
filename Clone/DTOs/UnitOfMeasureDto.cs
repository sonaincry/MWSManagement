using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;

namespace Indotalent.DTOs
{
    public class UnitOfMeasureDto
    {
        [Display(Name = "RecId")]
        [GridColumn(Width = 120, IsPrimaryKey = true, Visible = false, AllowEditing = false)]
        public long RecId { get; set; }

        [Display(Name = "Unit Code")]
        [GridColumn(Width = 120)]
        public string Symbol { get; set; } = string.Empty;

        [Display(Name = "Decimal Precision")]
        [GridColumn(Width = 140)]
        public int DecimalPrecision { get; set; }

        [Display(Name = "Unit Class")]
        [GridColumn(Width = 120)]
        public int UnitOfMeasureClass { get; set; }

        [Display(Name = "System Group")]
        [GridColumn(Width = 120)]
        public int SystemOfUnits { get; set; }
    }
}