using Indotalent.Models.Contracts;
using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indotalent.Models.Entities.AX
{
    public class TaxVatNumTableAX : IAxEntity
    {
        [Display(Name = "RecId")]
        [GridColumn(Width = 120, IsPrimaryKey = true, Visible = false, AllowEditing = false)]
        public long RecId { get; set; }

        [Display(Name = "Company")]
        [GridColumn(Width = 120)]
        public string? DataAreaId { get; set; }

        [Display(Name = "Country")]
        [GridColumn(Width = 120)]
        public string? CountryRegionId { get; set; }

        [Display(Name = "VAT Number")]
        [GridColumn(Width = 160)]
        public string? VatNum { get; set; }

        [Display(Name = "Name")]
        [GridColumn(Width = 250)]
        public string? Name { get; set; }

        [NotMapped]
        [GridColumn(Visible = false)]
        public byte[]? RowVersion { get; set; }
    }
}