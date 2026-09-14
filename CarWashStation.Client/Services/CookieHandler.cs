using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace CarWashStation.Client.Services;

public sealed class CookieHandler : DelegatingHandler
{
    private readonly AdminSession session;
    public CookieHandler(AdminSession session) : base(new HttpClientHandler()) => this.session = session;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        try
        {
            var token = await session.GetTokenAsync();
            if (!string.IsNullOrEmpty(token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        catch (JSException) { /* Public pages can still work when browser storage is disabled. */ }
        return await base.SendAsync(request, cancellationToken);
    }
}
