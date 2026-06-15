using System.ComponentModel.DataAnnotations;

namespace NewDiplom.Client.DTOs.Users;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "Введите фамилию")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите имя")]
    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Введите телефон")]
    [Phone(ErrorMessage = "Некорректный телефон")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите серию паспорта")]
    [RegularExpression(@"^\d{4}$",
    ErrorMessage = "Серия паспорта должна содержать 4 цифры")]
    public string PassportSeries { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите номер паспорта")]
    [RegularExpression(@"^\d{6}$",
    ErrorMessage = "Номер паспорта должен содержать 6 цифр")]
    public string PassportNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите кем выдан паспорт")]
    public string PassportIssuedBy { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите дату выдачи паспорта")]
    public DateTime? PassportIssueDate { get; set; }

    [Required(ErrorMessage = "Введите адрес регистрации")]
    public string RegistrationAddress { get; set; } = string.Empty;
}