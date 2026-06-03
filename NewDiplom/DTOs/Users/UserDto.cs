namespace NewDiplom.DTOs.Users;

public class UserDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public int? PostOfficeId { get; set; }

    public string? PassportSeries { get; set; }

    public string? PassportNumber { get; set; }

    public string? PassportIssuedBy { get; set; }

    public DateTime? PassportIssueDate { get; set; }

    public string? RegistrationAddress { get; set; }

    public bool IsActive { get; set; }

    public bool IsProfileCompleted { get; set; }
    public string? PostOfficeName { get; set; }
    public string Password { get; set; } = string.Empty;
}