namespace NewDiplom.Client.DTOs;

public class CreatePostOfficeDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? WorkingHours { get; set; }
}

public class UpdatePostOfficeDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? WorkingHours { get; set; }

    public bool IsActive { get; set; }
}
public class PostOfficeDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string? WorkingHours { get; set; }

    public bool IsActive { get; set; }
}