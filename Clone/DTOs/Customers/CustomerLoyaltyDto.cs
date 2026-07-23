using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;

namespace MWSManagement.DTOs.Customers
{
    public class CustomerLoyaltyDto
    {
        [Display(Name = "Account No.")]
        [GridColumn(Width = 130, IsPrimaryKey = true)]
        public string? AccountNum { get; set; }

        [Display(Name = "Full Name")]
        [GridColumn(Width = 200)]
        public string? CustomerName { get; set; }

        [Display(Name = "ID Number")]
        [GridColumn(Width = 140)]
        public string? IdentificationNumber { get; set; }

        [Display(Name = "Phone")]
        [GridColumn(Width = 130)]
        public string? MobilePhone { get; set; }

        [Display(Name = "Email")]
        [GridColumn(Width = 180)]
        public string? Email { get; set; }

        [Display(Name = "Birth Date")]
        [GridColumn(Width = 110, TextAlign = "Center")]
        public string? BirthDate { get; set; }

        [Display(Name = "Cards")]
        [GridColumn(Width = 80, TextAlign = "Center")]
        public int CardCount { get; set; }

        [Display(Name = "Card Numbers")]
        [GridColumn(Width = 280)]
        public string? Cards { get; set; }

        [Display(Name = "Address")]
        [GridColumn(Width = 250)]
        public string? FullAddress { get; set; }
    }
}