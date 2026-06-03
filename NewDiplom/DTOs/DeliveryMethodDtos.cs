namespace NewDiplom.DTOs;

public class CreateDeliveryMethodDto
{
    public string Name { get; set; } = string.Empty;

    public decimal AdditionalPrice { get; set; }

    public int? EstimatedDaysMin { get; set; }

    public int? EstimatedDaysMax { get; set; }
}

public class UpdateDeliveryMethodDto
{
    public string Name { get; set; } = string.Empty;

    public decimal AdditionalPrice { get; set; }

    public int? EstimatedDaysMin { get; set; }

    public int? EstimatedDaysMax { get; set; }

    public bool IsActive { get; set; }
}
public class DeliveryMethodDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal AdditionalPrice { get; set; }
    public int? EstimatedDaysMin { get; set; }

    public int? EstimatedDaysMax { get; set; }
    public bool IsActive { get; set; }
}