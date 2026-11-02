using EventEase.Models;

namespace EventEase.Services;

public record RegistrationResult(bool Success, string Message, Registration? Registration = null);

public record AttendanceStats(int Registrations, int Seats, int Attended, int Capacity)
{
    public int SeatsLeft => Math.Max(0, Capacity - Seats);
    public double AttendanceRate => Registrations == 0 ? 0 : (double)Attended / Registrations;
}

/// <summary>Registrations and attendance for all events, kept in localStorage.</summary>
public class RegistrationService(BrowserStorage storage, EventService events)
{
    private const string Key = "eventease.registrations";
    private List<Registration>? _items;

    public event Action? Changed;

    private async Task<List<Registration>> LoadAsync()
    {
        if (_items is not null) return _items;
        _items = await storage.GetAsync<List<Registration>>("localStorage", Key) ?? new List<Registration>();
        return _items;
    }

    private async Task SaveAsync()
    {
        await storage.SetAsync("localStorage", Key, _items);
        Changed?.Invoke();
    }

    public async Task<IReadOnlyList<Registration>> ForEventAsync(int eventId) =>
        (await LoadAsync()).Where(r => r.EventId == eventId).OrderBy(r => r.FullName).ToList();

    public async Task<IReadOnlyList<Registration>> ForEmailAsync(string email) =>
        (await LoadAsync())
            .Where(r => string.Equals(r.Email, email, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.RegisteredAt).ToList();

    public async Task<int> SeatsTakenAsync(int eventId) =>
        (await LoadAsync()).Where(r => r.EventId == eventId).Sum(r => r.Seats);

    public async Task<RegistrationResult> RegisterAsync(Registration registration)
    {
        var evt = await events.GetByIdAsync(registration.EventId);
        if (evt is null) return new(false, "This event no longer exists.");
        if (evt.Date < DateTime.Now) return new(false, "Registration is closed: this event has already started.");

        var list = await LoadAsync();
        var email = registration.Email.Trim();
        if (list.Any(r => r.EventId == evt.Id && string.Equals(r.Email, email, StringComparison.OrdinalIgnoreCase)))
            return new(false, "This email is already registered for this event.");

        var left = evt.Capacity - list.Where(r => r.EventId == evt.Id).Sum(r => r.Seats);
        if (registration.Seats > left)
            return new(false, left <= 0 ? "Sorry, this event is full." : $"Only {left} seat(s) are left.");

        var saved = new Registration
        {
            EventId = evt.Id,
            FullName = registration.FullName.Trim(),
            Email = email,
            Phone = string.IsNullOrWhiteSpace(registration.Phone) ? null : registration.Phone.Trim(),
            Seats = registration.Seats,
            AcceptTerms = true,
            RegisteredAt = DateTime.Now,
        };
        list.Add(saved);
        await SaveAsync();
        return new(true, $"You're registered for {evt.Name}.", saved);
    }

    public async Task SetAttendedAsync(Guid registrationId, bool attended)
    {
        var item = (await LoadAsync()).FirstOrDefault(r => r.Id == registrationId);
        if (item is null || item.Attended == attended) return;
        item.Attended = attended;
        await SaveAsync();
    }

    public async Task<AttendanceStats> StatsAsync(Event evt)
    {
        var list = (await LoadAsync()).Where(r => r.EventId == evt.Id).ToList();
        return new AttendanceStats(list.Count, list.Sum(r => r.Seats), list.Count(r => r.Attended), evt.Capacity);
    }
}
