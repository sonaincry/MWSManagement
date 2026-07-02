using Indotalent.Models.Contracts;
using MWSManagement.ControlUI.Helper.Grids;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWSManagement.Models.Entities
{
    [Table("TableSyncConfigs")]
    public class TableSync : IAxEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "RecId")]
        [GridColumn(Width = 120, IsPrimaryKey = true, Visible = false, AllowEditing = false)]
        public long RecId { get; set; }

        [Required]
        [Display(Name = "Code")]
        [GridColumn(Width = 120)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Table Name")]
        [GridColumn(Width = 250)]
        public string TableName { get; set; } = string.Empty;

        [Display(Name = "Sync Action")]
        [GridColumn(Width = 120)]
        public int SyncAction { get; set; }

        [Display(Name = "Active")]
        [GridColumn(Width = 100)]
        public bool IsActive { get; set; } = true;
    }
}
