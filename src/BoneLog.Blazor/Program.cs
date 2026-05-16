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


SiteConfig? config = await new HttpClient().GetFromJsonAsync<SiteConfig>($"config.json", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
if(config == null)
{
    Console.WriteLine("Failed to load site configuration.");
    return;
}

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton(config);
builder.Services.AddSingleton(new PathSettings(config.PostsPath, config.IndexPath));
builder.Services.AddScoped<IPostReader,PostReader>();

builder.Services.AddSingleton<ThemeService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


await builder.Build().RunAsync();