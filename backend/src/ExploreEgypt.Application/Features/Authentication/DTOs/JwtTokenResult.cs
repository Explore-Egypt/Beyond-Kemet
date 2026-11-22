namespace ExploreEgypt.Application.Features.Authentication.DTOs;

public class JwtTokenResult
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}
