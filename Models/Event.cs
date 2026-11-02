using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

/// <summary>An event that people can browse, register for and attend.</summary>
public class Event
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "Event name must be 3 to 80 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Event date is required.")]
    public DateTime Date { get; set; } = DateTime.Today.AddDays(7);

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(120, ErrorMessage = "Location must be at most 120 characters.")]
    public string Location { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description must be at most 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000.")]
    public int Capacity { get; set; } = 50;

    public Event Clone() => (Event)MemberwiseClone();
}
