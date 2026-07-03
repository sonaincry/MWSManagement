using Indotalent.Models.Contracts;
using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Đảm bảo có thư viện này

namespace MWSManagement.Models.Entities
{
    [Table("JobSync")]
    public class JobSync : IAxEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "RecId")]
        [GridColumn(Width = 120, IsPrimaryKey = true, Visible = false, AllowEditing = false)]
        public long RecId { get; set; }

        [Required]
        [Display(Name = "Job Code")]
        [GridColumn(Width = 150)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [GridColumn(Width = 200)]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Senders")]
        [GridColumn(Visible = false, AllowEditing = false)]
        public string SenderLocationIds { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Receivers")]
        [GridColumn(Visible = false, AllowEditing = false)]
        public string ReceiverLocationIds { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tables")]
        [GridColumn(Visible = false, AllowEditing = false)]
        public string TableNames { get; set; } = string.Empty;

        [NotMapped]
        public List<string> SelectedSenders { get; set; } = new();

        [NotMapped]
        public List<string> SelectedReceivers { get; set; } = new();

        [NotMapped]
        public List<string> SelectedTables { get; set; } = new();
    }
}