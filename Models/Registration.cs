using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

/// <summary>A person's registration for one event, including whether they attended.</summary>
public class Registration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int EventId { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Full name must be 2 to 60 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number.")]
    public string? Phone { get; set; }

    [Range(1, 5, ErrorMessage = "You can reserve 1 to 5 seats.")]
    public int Seats { get; set; } = 1;

    [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept the event terms.")]
    public bool AcceptTerms { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.Now;

    public bool Attended { get; set; }
}
