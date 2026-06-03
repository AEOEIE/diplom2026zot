using System.Net.Http.Headers;

namespace NewDiplom.Client.Services;

public class JwtHttpHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public JwtHttpHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}