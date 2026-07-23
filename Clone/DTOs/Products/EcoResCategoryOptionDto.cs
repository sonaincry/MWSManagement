namespace Indotalent.DTOs
{
    public class EcoResCategoryOptionDto
    {
        public long CategoryRecId { get; set; }
        public long CategoryHierarchyRecId { get; set; }
        public string Name { get; set; } = string.Empty;
        public long SortOrder { get; set; }
        public long Level { get; set; }
    }
}