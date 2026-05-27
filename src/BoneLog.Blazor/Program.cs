using BoneLog.Abstractions;
using BoneLog.Blazor;
using BoneLog.Blazor.Dtos;
using BoneLog.Blazor.Services;
using BoneLog.Models;
using BoneLog.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;
using System.Text.Json;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

var configUrl = new Uri(new Uri(builder.HostEnvironment.BaseAddress), "config.json");
SiteConfig? config = await new HttpClient().GetFromJsonAsync<SiteConfig>(configUrl, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

if (config == null)
{
    Console.WriteLine("Failed to load site configuration.");
    return;
}

builder.Services.AddSingleton<SiteConfig>(config);
builder.Services.AddSingleton<PathSettings>(config.Paths);
builder.Services.AddScoped<IBlogContentProvider, BlogContentProvider>();

builder.Services.AddSingleton<ThemeService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


await builder.Build().RunAsync();