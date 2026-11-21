using ExploreEgypt.Application.Features.Authentication.DTOs;

namespace ExploreEgypt.Application.Features.Authentication.Interfaces;

public interface IJwtTokenService
{
    JwtTokenResult GenerateToken(
        Guid userId,
        string email,
        string fullName,
        IList<string> roles);
}
