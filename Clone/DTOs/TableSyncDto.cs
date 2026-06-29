namespace MWSManagement.DTOs
{
    public class TableSyncDto
    {
        public int Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int SyncAction { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
