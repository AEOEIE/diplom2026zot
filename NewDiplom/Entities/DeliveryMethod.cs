namespace NewDiplom.Entities;

public class DeliveryMethod
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal AdditionalPrice { get; set; }

    public int? EstimatedDaysMin { get; set; }

    public int? EstimatedDaysMax { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}