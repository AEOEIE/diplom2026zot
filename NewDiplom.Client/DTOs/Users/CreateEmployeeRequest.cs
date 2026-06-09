using System.ComponentModel.DataAnnotations;

namespace NewDiplom.Client.DTOs.Users;

public class CreateEmployeeRequest
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
        ErrorMessage = "Только латиница, цифры и _")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "Минимум 8 символов")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$",
        ErrorMessage = "Пароль должен содержать буквы и цифры")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите серию паспорта")]
    public string PassportSeries { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите номер паспорта")]
    public string PassportNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите адрес регистрации")]
    public string RegistrationAddress { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public int? PostOfficeId { get; set; }

    public string PassportIssuedBy { get; set; } = string.Empty;

    public DateTime? PassportIssueDate { get; set; }
}