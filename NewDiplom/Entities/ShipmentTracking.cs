namespace NewDiplom.Entities;

public class ShipmentTracking
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public int StatusId { get; set; }

    public int OfficeId { get; set; }

    public int? EmployeeId { get; set; }

    public string? Comment { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public Shipment? Shipment { get; set; }

    public ShipmentStatus? Status { get; set; }

    public PostOffice? Office { get; set; }

    public User? Employee { get; set; }
}