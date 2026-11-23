using System.ComponentModel.DataAnnotations;

namespace ExploreEgypt.Application.Features.Users.DTOs;

public class ChangeMyPasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;
}