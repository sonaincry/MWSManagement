using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Indotalent.DTOs
{
    public class UnitOfMeasureDto
    {
        [Column("RECID")]
        [Display(Name = "RECID")]
        [GridColumn(Width = 100)]
        public long RecId { get; set; }

        [Column("SYMBOL")]
        [Display(Name = "SYMBOL")]
        [GridColumn(Width = 100)]
        public string Symbol { get; set; } = string.Empty;

        [Column("DECIMALPRECISION")]
        [Display(Name = "DECIMALPRECISION")]
        [GridColumn(Width = 100)]
        public int DecimalPrecision { get; set; }

        [Column("UNITOFMEASURECLASS")]
        [Display(Name = "UNITOFMEASURECLASS")]
        [GridColumn(Width = 100)]
        public int UnitOfMeasureClass { get; set; }

        [Column("SYSTEMOFUNITS")]
        [Display(Name = "SYSTEMOFUNITS")]
        [GridColumn(Width = 100)]
        public int SystemOfUnits { get; set; }
    }
}