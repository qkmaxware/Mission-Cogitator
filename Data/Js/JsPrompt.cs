using Microsoft.JSInterop;

namespace Kt.Data.JsInterop;

public class JsPrompt
{
   private readonly IJSRuntime JsRuntime;
   public JsPrompt(IJSRuntime jSRuntime)
   {
       this.JsRuntime = jSRuntime;
   }

   public async Task<TReturn?> PromptAsync<TReturn>(object? message, TReturn? defaultValue)
   where TReturn:IParsable<TReturn>
    {
        var inputStr = await this.JsRuntime.InvokeAsync<string>("prompt", message?.ToString() ?? string.Empty, defaultValue?.ToString() ?? string.Empty);
        if (TReturn.TryParse(inputStr, null, out TReturn? parsedValue))
        {   
            return parsedValue;
        }
        return defaultValue;
    }

    public async Task<bool> ConfirmAsync(object? message)
    {
        return await this.JsRuntime.InvokeAsync<bool>("confirm", message?.ToString() ?? "Are you sure?");
    }

    public async Task AlertAsync(object? message)
    {
        await this.JsRuntime.InvokeAsync<bool>("alert", message?.ToString());
    }
}