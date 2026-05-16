using Microsoft.JSInterop;

namespace BoneLog.Blazor.Services;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private const string StorageKey = "isDarkTheme";

    public bool IsDark { get; private set; } = false;

    public event Action? OnChange;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        var stored = await _jsRuntime.InvokeAsync<string>("localStorage.getItem",StorageKey);
        if(bool.TryParse(stored,out var isDarkStored))
        {
            IsDark = isDarkStored;
        }
        await ApplyAsync();
    }

    public async Task ToggleAsync()
    {
        IsDark = !IsDark;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem",StorageKey,IsDark.ToString());
        await ApplyAsync();
        OnChange?.Invoke();
    }

    private async Task ApplyAsync()
    {
        await _jsRuntime.InvokeVoidAsync("themeHelper.applyDarkClass",IsDark);
    }
}
