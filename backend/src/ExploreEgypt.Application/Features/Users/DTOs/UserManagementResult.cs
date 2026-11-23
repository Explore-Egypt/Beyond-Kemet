using ExploreEgypt.Application.Features.Authentication.DTOs;

namespace ExploreEgypt.Application.Features.Users.DTOs;

public class UserManagementResult
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public IList<string> Errors { get; set; } = new List<string>();

    public UserProfileResponse? Profile { get; set; }

    public UserManagementResponse? User { get; set; }
}