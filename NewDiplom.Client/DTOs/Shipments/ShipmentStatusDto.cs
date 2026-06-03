namespace NewDiplom.DTOs.Shipments
{
    public class ShipmentStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
    }
}
