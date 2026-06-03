using Blazored.LocalStorage;

namespace NewDiplom.Client.Services;

public class TokenService
{
    private readonly ILocalStorageService _localStorage;

    public TokenService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SetTokenAsync(string token)
    {
        await _localStorage.SetItemAsync("authToken", token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    public async Task RemoveTokenAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
    }
}