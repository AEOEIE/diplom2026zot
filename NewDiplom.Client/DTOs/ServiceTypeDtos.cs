namespace NewDiplom.Client.DTOs;

public class CreateServiceTypeDto
{
    public string Name { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public string? Description { get; set; }
}

public class UpdateServiceTypeDto
{
    public string Name { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
public class ServiceTypeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}