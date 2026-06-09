namespace NewDiplom.Client.DTOs.Shipments;

public class CreateShipmentRequest
{
    public int CurrentOfficeId { get; set; }  //откуда отправляем
    public int DestinationOfficeId { get; set; }  // Куда отправляем
    public int? ClientId { get; set; }  
    public string RecipientName { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public int DeliveryMethodId { get; set; }
    public decimal WeightKg { get; set; }
    public decimal? DeclaredValue { get; set; }
    public string? Notes { get; set; }
}