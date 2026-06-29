namespace MWSManagement.ControlUI.Helper.Grids
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GridColumnAttribute : Attribute
    {
        public int Width { get; set; } = 150;
        public bool IsPrimaryKey { get; set; } = false;
        public bool Visible { get; set; } = true;
        public string? TextAlign { get; set; }
        public string? Format { get; set; }
        public string? Type { get; set; }
        public string? EditType { get; set; }
        public bool AllowEditing { get; set; } = true;
        public bool AllowFiltering { get; set; } = true;
        public bool AllowSorting { get; set; } = true;
    }
}