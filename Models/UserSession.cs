using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

/// <summary>The signed-in visitor for the current browser session.</summary>
public class UserSession
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Name must be 2 to 60 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    public DateTime SignedInAt { get; set; } = DateTime.Now;
}
