using Indotalent.Models.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWSManagement.Models
{
    [Table("JobLog")]
    public class JobLog : IAxEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long RecId { get; set; }

        [Required]
        [StringLength(50)]
        public string JobCode { get; set; }

        [Required]
        public long JobRecId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime RunDate { get; set; }

        [Required]
        public TimeSpan RunTime { get; set; }
    }
}