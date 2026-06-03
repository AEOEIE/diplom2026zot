namespace NewDiplom.Entities;

public class ServiceType
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}