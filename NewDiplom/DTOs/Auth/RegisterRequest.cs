namespace NewDiplom.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required(ErrorMessage = "Логин обязателен")]
    [StringLength(20, MinimumLength = 4,
        ErrorMessage = "Логин должен быть от 4 до 20 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Логин может содержать только латинские буквы, цифры и _")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "Пароль должен быть минимум 8 символов")]
    [RegularExpression(
        @"^(?=.*[a-zA-Z])(?=.*\d).+$",
        ErrorMessage = "Пароль должен содержать буквы и цифры")]
    public string Password { get; set; }
}