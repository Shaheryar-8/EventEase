using EventEase.Models;

namespace EventEase.Services;

/// <summary>Holds the event catalogue. Seeded with sample data; edits are kept in localStorage.</summary>
public class EventService(BrowserStorage storage)
{
    private const string Key = "eventease.events";
    private List<Event>? _events;

    public event Action? Changed;

    private async Task<List<Event>> LoadAsync()
    {
        if (_events is not null) return _events;
        _events = await storage.GetAsync<List<Event>>("localStorage", Key) ?? Seed();
        return _events;
    }

    public async Task<IReadOnlyList<Event>> GetAllAsync() =>
        (await LoadAsync()).OrderBy(e => e.Date).ToList();

    public async Task<Event?> GetByIdAsync(int id) =>
        (await LoadAsync()).FirstOrDefault(e => e.Id == id);

    public async Task UpdateAsync(Event updated)
    {
        var list = await LoadAsync();
        var index = list.FindIndex(e => e.Id == updated.Id);
        if (index < 0) throw new InvalidOperationException($"Event {updated.Id} does not exist.");
        list[index] = updated.Clone();
        await storage.SetAsync("localStorage", Key, list);
        Changed?.Invoke();
    }

    public async Task<Event> AddAsync(Event item)
    {
        var list = await LoadAsync();
        item.Id = list.Count == 0 ? 1 : list.Max(e => e.Id) + 1;
        list.Add(item.Clone());
        await storage.SetAsync("localStorage", Key, list);
        Changed?.Invoke();
        return item;
    }

    private static List<Event> Seed()
    {
        var today = DateTime.Today;
        var names = new[]
        {
            ("Tech Leaders Summit", "Expo Center, Lahore", "Talks and panels on cloud, AI and product leadership.", 300),
            ("Startup Pitch Night", "Arfa Tower, Lahore", "Ten startups pitch to investors, followed by networking.", 120),
            ("Blazor Workshop", "Online", "Hands-on workshop: build a full Blazor WebAssembly app in a day.", 60),
            ("Marketing Masterclass", "Pearl Continental, Karachi", "Practical growth marketing for small businesses.", 150),
            ("Annual Charity Gala", "Serena Hotel, Islamabad", "Dinner and auction in support of local schools.", 250),
            ("Design Sprint Bootcamp", "NIC, Lahore", "Five-day design sprint method compressed into one intensive day.", 40),
        };
        return names.Select((n, i) => new Event
        {
            Id = i + 1,
            Name = n.Item1,
            Location = n.Item2,
            Description = n.Item3,
            Capacity = n.Item4,
            Date = today.AddDays(5 + i * 9).AddHours(10 + i % 3 * 3),
        }).ToList();
    }
}
