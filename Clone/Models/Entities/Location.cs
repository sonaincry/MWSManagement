using Indotalent.Models.Contracts;
using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWSManagement.Models.Entities
{
    [Table("SyncLocations")]
    public class Location : IAxEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "RecId")]
        [GridColumn(Width = 120, IsPrimaryKey = true, Visible = false, AllowEditing = false)]
        public long RecId { get; set; }

        [Required]
        [Display(Name = "Location Name")]
        [GridColumn(Width = 180)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Server Address")]
        [GridColumn(Width = 150)]
        public string Server { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Database")]
        [GridColumn(Width = 150)]
        public string DatabaseName { get; set; } = string.Empty;

        [Required] 
        [Display(Name = "Username")]
        [GridColumn(Width = 120)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Password")]
        [GridColumn(Width = 120, Visible = false)]
        public string Password { get; set; } = string.Empty;
    }
}