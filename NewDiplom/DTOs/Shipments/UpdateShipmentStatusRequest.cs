namespace NewDiplom.DTOs.Shipments;

public class UpdateShipmentStatusRequest
{
    public int StatusId { get; set; }

    public string? Comment { get; set; }
}