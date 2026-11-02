using EventEase.Models;

namespace EventEase.Services;

/// <summary>
/// Keeps track of who is using the app in this browser tab. The session lives in sessionStorage,
/// so it survives page reloads but ends when the tab is closed.
/// </summary>
public class UserSessionService(BrowserStorage storage)
{
    private const string Key = "eventease.session";
    private bool _loaded;

    public UserSession? Current { get; private set; }
    public bool IsSignedIn => Current is not null;

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        if (_loaded) return;
        Current = await storage.GetAsync<UserSession>("sessionStorage", Key);
        _loaded = true;
        Changed?.Invoke();
    }

    public async Task SignInAsync(string name, string email)
    {
        Current = new UserSession { Name = name.Trim(), Email = email.Trim(), SignedInAt = DateTime.Now };
        await storage.SetAsync("sessionStorage", Key, Current);
        Changed?.Invoke();
    }

    public async Task SignOutAsync()
    {
        Current = null;
        await storage.RemoveAsync("sessionStorage", Key);
        Changed?.Invoke();
    }
}
