using System.ComponentModel.DataAnnotations;

namespace ExploreEgypt.Application.Features.Users.DTOs;

public class UpdateUserRolesRequest
{
    [Required]
    [MinLength(1)]
    public List<string> Roles { get; set; } = new();
}