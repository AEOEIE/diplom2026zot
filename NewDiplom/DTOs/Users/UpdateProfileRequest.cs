namespace NewDiplom.DTOs.Users;

public class UpdateProfileRequest
{
    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PassportSeries { get; set; } = string.Empty;

    public string PassportNumber { get; set; } = string.Empty;

    public string PassportIssuedBy { get; set; } = string.Empty;

    public DateTime? PassportIssueDate { get; set; }

    public string RegistrationAddress { get; set; } = string.Empty;
}