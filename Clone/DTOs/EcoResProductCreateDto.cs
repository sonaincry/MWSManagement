using System.ComponentModel.DataAnnotations;

namespace Indotalent.DTOs
{
    public class EcoResProductCreateDto
    {
        public long RecId { get; set; } // 0 create, # 0 edit

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string SearchName { get; set; } = string.Empty;

        public int ProductType { get; set; } = 1;

        public long? CategoryRecId { get; set; }

        public long? CategoryHierarchyRecId { get; set; }
    }
}