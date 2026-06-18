namespace MWSManagement.DTOs
{
    public class CategoryDTO
    {
        public long RecId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ParentName { get; set; }
        public string? HierarchyName { get; set; }
    }
}
