using System.Reflection;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Kt.Data.JsInterop;

public class JsLocalStorage
{
    private string _prefix = Assembly.GetExecutingAssembly().GetName().Name + ".";
    private readonly IJSRuntime JsRuntime;
    public JsLocalStorage(IJSRuntime jSRuntime)
    {
        this.JsRuntime = jSRuntime;
    }

    public async Task ClearAsync()
    {
        await JsRuntime.InvokeVoidAsync("localStorage.clear");
    }

    public async Task StoreItemAsync<TItem>(string key, TItem item, JsonSerializerOptions? options = null)
    {
        var datum = JsonSerializer.Serialize(item, options ?? JsonSerializerOptions.Default);
        await JsRuntime.InvokeVoidAsync("localStorage.setItem", _prefix + key, datum);
    }

    public async Task<TItem?> RetrieveItemAsync<TItem>(string key, JsonSerializerOptions? options = null)
    {
        var str = await JsRuntime.InvokeAsync<string>("localStorage.getItem", _prefix + key);
        if (str is null)
            return default;
        return JsonSerializer.Deserialize<TItem>(str, options ?? JsonSerializerOptions.Default);
    }

    public async Task RemoveItemAsync<TItem>(string key)
    {
        await JsRuntime.InvokeVoidAsync("localStorage.removeItem", _prefix + key);
    }

}