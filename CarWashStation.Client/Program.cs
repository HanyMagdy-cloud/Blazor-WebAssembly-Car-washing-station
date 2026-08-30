using CarWashStation.Client;
using CarWashStation.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is missing from wwwroot/appsettings.json.");
builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped(sp => new HttpClient(sp.GetRequiredService<CookieHandler>())
{
    BaseAddress = new Uri(apiBaseUrl)
});
await builder.Build().RunAsync();
