using System.ComponentModel.DataAnnotations;

namespace NewDiplom.DTOs.Users;

public class UpdateEmployeeRequest
{
    [Required(ErrorMessage = "Введите имя")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите фамилию")]
    public string LastName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Введите телефон")]
    [Phone(ErrorMessage = "Некорректный телефон")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите логин")]
    [StringLength(20, MinimumLength = 4,
        ErrorMessage = "Логин должен быть от 4 до 20 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Логин может содержать только латиницу, цифры и _")]
    public string Login { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public int? PostOfficeId { get; set; }

    public bool IsActive { get; set; }

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

    public string? Password { get; set; }
}