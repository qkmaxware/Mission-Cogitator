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

var packages = app.Services.GetService<PackageManager>()!;
await packages.AddFromUrl("assets/packages/lite-rules");
await packages.AddFromUrl("assets/packages/universal-equipment");
await packages.AddFromUrl("assets/packages/teams/pathfinders");
await packages.AddFromUrl("assets/packages/teams/angels-of-death");
await packages.AddFromUrl("assets/packages/teams/deathwatch");
await packages.AddFromUrl("assets/packages/teams/vespid-stingwings");
await packages.AddFromUrl("assets/packages/teams/battleclade");
await packages.AddFromUrl("assets/packages/teams/hearthkyn-salvagers");

await app.RunAsync();
