namespace ExploreEgypt.Application.Features.Authentication.DTOs;

public class AuthResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Token { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public UserProfileResponse? User { get; set; }

    public IList<string> Errors { get; set; } = new List<string>();
}
