using Microsoft.JSInterop;

namespace Kt.Data.IO;

public class JsConsole
{
   private readonly IJSRuntime JsRuntime;
   public JsConsole(IJSRuntime jSRuntime)
   {
       this.JsRuntime = jSRuntime;
   }

   public async Task LogAsync(object? message)
   {
       await this.JsRuntime.InvokeVoidAsync("console.log", message?.ToString() ?? string.Empty);
   }

   public async Task ErrorAsync(object? message)
   {
       await this.JsRuntime.InvokeVoidAsync("console.error", message?.ToString() ?? string.Empty);
   }

   public async Task WarnAsync(object? message)
   {
       await this.JsRuntime.InvokeVoidAsync("console.warn", message?.ToString() ?? string.Empty);
   }
}