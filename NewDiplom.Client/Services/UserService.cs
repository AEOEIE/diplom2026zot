using System.Net.Http.Json;
using NewDiplom.Client.DTOs.Users;

namespace NewDiplom.Client.Services;

public class UserService
{
    private readonly HttpClient _http;

    public UserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserDto?> GetMeAsync()
    {
        return await _http.GetFromJsonAsync<UserDto>(
            "api/Users/me");
    }

    public async Task<bool> UpdateProfileAsync(
        UpdateProfileRequest request)
    {
        var response = await _http.PutAsJsonAsync(
            "api/Users/me",
            request);

        return response.IsSuccessStatusCode;
    }
}