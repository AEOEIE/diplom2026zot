using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;

namespace NewDiplom.Client.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenService _tokenService;
    public AuthStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity()));
        }

        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.ReadJwtToken(token);

        var claims = jwt.Claims;

        var identity = new ClaimsIdentity(
            claims,
            "jwt");

        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _tokenService.SetTokenAsync(token);

        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(
            jwt.Claims,
            "jwt");

        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _tokenService.RemoveTokenAsync();

        var anonymous = new ClaimsPrincipal(
            new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(anonymous)));
    }
}