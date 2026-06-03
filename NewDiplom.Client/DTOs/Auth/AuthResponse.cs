namespace NewDiplom.Client.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}