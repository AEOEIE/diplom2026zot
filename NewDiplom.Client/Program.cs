using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NewDiplom.Client;
using NewDiplom.Client.Services;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<JwtHttpHandler>();
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<JwtHttpHandler>();

    handler.InnerHandler = new HttpClientHandler();

    return new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5254/")
    };
});





builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<AuthStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(
    provider => provider.GetRequiredService<AuthStateProvider>());
builder.Services.AddScoped<ShipmentService>();
builder.Services.AddScoped<UserService>();

await builder.Build().RunAsync();
