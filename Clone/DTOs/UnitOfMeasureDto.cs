namespace Indotalent.DTOs
{
    public class UnitOfMeasureDto
    {
        public long RecId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public int DecimalPrecision { get; set; }
        public int UnitOfMeasureClass { get; set; }
        public int SystemOfUnits { get; set; }
    }
}