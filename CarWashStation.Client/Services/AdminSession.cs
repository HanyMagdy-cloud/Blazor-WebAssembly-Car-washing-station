using Microsoft.JSInterop;

namespace CarWashStation.Client.Services;

public sealed class AdminSession(IJSRuntime js)
{
    private const string Key = "carwash.admin.token";

    public async Task<string?> GetTokenAsync() =>
        await js.InvokeAsync<string?>("sessionStorage.getItem", Key)
        ?? await js.InvokeAsync<string?>("localStorage.getItem", Key);

    public async Task SaveAsync(string token, bool rememberMe)
    {
        await ClearAsync();
        await js.InvokeVoidAsync(rememberMe ? "localStorage.setItem" : "sessionStorage.setItem", Key, token);
    }

    public async Task ClearAsync()
    {
        await js.InvokeVoidAsync("sessionStorage.removeItem", Key);
        await js.InvokeVoidAsync("localStorage.removeItem", Key);
    }
}
