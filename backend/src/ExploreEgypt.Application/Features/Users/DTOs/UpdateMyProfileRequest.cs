using System.ComponentModel.DataAnnotations;

namespace ExploreEgypt.Application.Features.Users.DTOs;

public class UpdateMyProfileRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;
}