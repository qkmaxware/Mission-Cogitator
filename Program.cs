using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Kt;
using Kt.Data;
using Kt.Data.IO;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<JsConsole>(); // For logging to the browser console
builder.Services.AddScoped(sp => new RuleDatabase());
builder.Services.AddScoped(sp => new TeamsDatabase());
builder.Services.AddScoped(sp => new PackageManager(sp.GetService<JsConsole>()!, sp.GetService<HttpClient>()!, sp.GetService<RuleDatabase>()!, sp.GetService<TeamsDatabase>()!));

var app = builder.Build();


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
];
var packageManager = app.Services.GetService<PackageManager>()!;
foreach (var pkgName in packages)
{
    await packageManager.AddFromUrl(pkgName);    
}

await app.RunAsync();
