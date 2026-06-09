namespace NewDiplom.DTOs.Shipments;

public class ShipmentDto
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string DeliveryMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string? CurrentOfficeName { get; set; }  
    public string? DestinationOfficeName { get; set; }  
}