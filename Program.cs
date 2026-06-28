using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Kt;
using Kt.Data;
using Kt.Data.IO;
using Microsoft.JSInterop;
using Kt.Data.JsInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<JsConsole>(); // For logging to the browser console
builder.Services.AddScoped<JsPrompt>(); // For prompting
builder.Services.AddScoped(sp => new RuleDatabase());
builder.Services.AddScoped(sp => new TeamsDatabase());
builder.Services.AddScoped(sp => new PackageManager(sp.GetService<JsConsole>()!, sp.GetService<HttpClient>()!, sp.GetService<RuleDatabase>()!, sp.GetService<TeamsDatabase>()!));

var app = builder.Build();
var js = app.Services.GetService<IJSRuntime>()!;

async Task updateProgress(int step, int maxSteps, string? title = null)
{
    var percent = Math.Clamp((int)MathF.Floor(((float)step/(float)maxSteps) * 100), 0 , 100);
    await js.InvokeVoidAsync("updateLoader", percent, title);
}

string[] packages = [
    "assets/packages/lite-rules",
    "assets/packages/universal-equipment",
    "assets/packages/teams/pathfinders",
    "assets/packages/teams/angels-of-death",
    "assets/packages/teams/deathwatch",
    "assets/packages/teams/vespid-stingwings",
    "assets/packages/teams/battleclade",
    "assets/packages/teams/hearthkyn-salvagers",
    "assets/packages/teams/raveners",
    "assets/packages/teams/hierotek-circle",
    "assets/packages/teams/wrecka-krew",
    "assets/packages/teams/xv26-stealth-battlesuits",
    "assets/packages/teams/kommandos",
    "assets/packages/teams/imperial-navy-breachers",
];
var lastPackage = packages.Length - 1;
var packageManager = app.Services.GetService<PackageManager>()!;
await updateProgress(0, lastPackage, "Loading Asset Packs...");
for (int i = 0, next=1; i < packages.Length; i++, next++)
{
    var pkgName = packages[i];
    await packageManager.AddFromUrl(pkgName);  
    await updateProgress(next, lastPackage);  
}

await app.RunAsync();
