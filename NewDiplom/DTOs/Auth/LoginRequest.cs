using System.ComponentModel.DataAnnotations;

namespace NewDiplom.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Введите логин")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = string.Empty;
}