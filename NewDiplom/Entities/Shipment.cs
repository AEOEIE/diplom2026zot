namespace NewDiplom.Entities;

public class Shipment
{
    public int Id { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public int ClientId { get; set; }

    public int? SenderEmployeeId { get; set; }

    public int CurrentOfficeId { get; set; }

    public int DestinationOfficeId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string DestinationAddress { get; set; } = string.Empty;

    public int ServiceTypeId { get; set; }

    public int DeliveryMethodId { get; set; }

    public int CurrentStatusId { get; set; }

    public decimal WeightKg { get; set; }

    public decimal? DeclaredValue { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpectedDeliveryDate { get; set; }

    public DateTime? ActualDeliveryDate { get; set; }

    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }

    public User? Client { get; set; }

    public User? SenderEmployee { get; set; }

    public PostOffice? CurrentOffice { get; set; }

    public PostOffice? DestinationOffice { get; set; }

    public ServiceType? ServiceType { get; set; }

    public DeliveryMethod? DeliveryMethod { get; set; }

    public ShipmentStatus? CurrentStatus { get; set; }

    public string RecipientPhone { get; set; } = string.Empty;

    public string? ConfirmationCode { get; set; }

    public bool IsConfirmed { get; set; }

    public ICollection<ShipmentTracking> TrackingRecords { get; set; } = new List<ShipmentTracking>();
}