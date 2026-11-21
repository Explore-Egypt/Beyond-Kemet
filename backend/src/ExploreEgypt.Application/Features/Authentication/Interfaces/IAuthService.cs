using ExploreEgypt.Application.Features.Authentication.DTOs;

namespace ExploreEgypt.Application.Features.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<UserProfileResponse?> GetCurrentUserAsync(Guid userId);
}
