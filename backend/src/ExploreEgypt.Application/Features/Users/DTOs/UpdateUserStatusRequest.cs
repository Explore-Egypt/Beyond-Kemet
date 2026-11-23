using System.ComponentModel.DataAnnotations;

namespace ExploreEgypt.Application.Features.Users.DTOs;

public class UpdateUserStatusRequest
{
    [Required]
    public bool? IsActive { get; set; }
}