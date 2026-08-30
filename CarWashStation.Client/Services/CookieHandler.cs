using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace CarWashStation.Client.Services;

public sealed class CookieHandler : DelegatingHandler
{
    public CookieHandler() : base(new HttpClientHandler()) { }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}
