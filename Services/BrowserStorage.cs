using System.Text.Json;
using Microsoft.JSInterop;

namespace EventEase.Services;

/// <summary>Small wrapper around the browser's localStorage and sessionStorage.</summary>
public class BrowserStorage(IJSRuntime js)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string area, string key)
    {
        try
        {
            var raw = await js.InvokeAsync<string?>($"{area}.getItem", key);
            return string.IsNullOrWhiteSpace(raw) ? default : JsonSerializer.Deserialize<T>(raw, Json);
        }
        catch (Exception ex) when (ex is JSException or JsonException)
        {
            // Storage blocked (private mode) or corrupted value: start fresh instead of crashing.
            return default;
        }
    }

    public async Task SetAsync<T>(string area, string key, T value)
    {
        try { await js.InvokeVoidAsync($"{area}.setItem", key, JsonSerializer.Serialize(value, Json)); }
        catch (JSException) { /* storage not available: keep working in memory */ }
    }

    public async Task RemoveAsync(string area, string key)
    {
        try { await js.InvokeVoidAsync($"{area}.removeItem", key); }
        catch (JSException) { }
    }
}
