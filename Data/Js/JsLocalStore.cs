using Microsoft.JSInterop;

namespace Kt.Data.JsInterop;

public class JsLocalStore
{
   private readonly IJSRuntime JsRuntime;
   public JsLocalStore(IJSRuntime jSRuntime)
   {
       this.JsRuntime = jSRuntime;
   }

    public async Task ClearAsync()
    {
        await JsRuntime.InvokeVoidAsync("localStorage.clear");
    }

    public async Task StoreItemAsync<TItem>(string key, TItem item)
    {
        await JsRuntime.InvokeVoidAsync("localStorage.setItem", key, item);
    }

    public async Task<TItem?> RetrieveItemAsync<TItem>(string key)
    {
        return await JsRuntime.InvokeAsync<TItem>("localStorage.getItem", key);
    }

    public async Task RemoveItemAsync<TItem>(string key)
    {
        await JsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }

}