using Microsoft.JSInterop;

namespace Kt.Data.JsInterop;

public class JsEventListener : IAsyncDisposable
{
    private IJSObjectReference jSObjectReference;

    public JsEventListener(IJSObjectReference jSObjectReference)
    {
        this.jSObjectReference = jSObjectReference;
    }

    public async ValueTask DisposeAsync() {
        await this.jSObjectReference.InvokeVoidAsync("dispose");
    }
}

public class JsWindow
{
   private readonly IJSRuntime JsRuntime;
   public JsWindow(IJSRuntime jSRuntime)
   {
       this.JsRuntime = jSRuntime;
   }

    public async Task<JsEventListener> AddEventListener(string eventName, object self, string methodName)
    {
        var dotNetObjectReference = DotNetObjectReference.Create(self);
        var jsObjectReference = await this.JsRuntime.InvokeAsync<IJSObjectReference>("addDotNetEventListener", dotNetObjectReference, eventName, methodName);
        return new JsEventListener(jsObjectReference);
    }


}