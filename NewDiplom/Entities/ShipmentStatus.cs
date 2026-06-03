namespace NewDiplom.Entities;

public class ShipmentStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public ICollection<ShipmentTracking> TrackingRecords { get; set; } = new List<ShipmentTracking>();
}