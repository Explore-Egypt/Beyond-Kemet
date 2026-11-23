using ExploreEgypt.Application.Features.Authentication.DTOs;
using ExploreEgypt.Application.Features.Authentication.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ExploreEgypt.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim();

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
        {
            return new AuthResponse
            {
                Succeeded = false,
                Message = "Registration failed.",
                Errors = createResult.Errors
                    .Select(error => error.Description)
                    .ToList()
            };
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            "Traveler");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return new AuthResponse
            {
                Succeeded = false,
                Message = "Could not assign user role.",
                Errors = roleResult.Errors
                    .Select(error => error.Description)
                    .ToList()
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        var tokenResult = _jwtTokenService.GenerateToken(
            user.Id,
            user.Email!,
            user.FullName!,
            roles,
            user.SecurityStamp ?? throw new InvalidOperationException("User security stamp is missing."));

        return new AuthResponse
        {
            Succeeded = true,
            Message = "Registration successful.",
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            User = MapUser(user, roles)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return new AuthResponse
            {
                Succeeded = false,
                Message = "Invalid email or password."
            };
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return new AuthResponse
            {
                Succeeded = false,
                Message = "Account temporarily locked. Try again later."
            };
        }

        if (!signInResult.Succeeded)
        {
            return new AuthResponse
            {
                Succeeded = false,
                Message = "Invalid email or password."
            };
        }

        if (!user.IsActive)
        {
            return new AuthResponse
            {
                Succeeded = false,
                Message = "Account is inactive. Please contact support."
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        var tokenResult = _jwtTokenService.GenerateToken(
            user.Id,
            user.Email!,
            user.FullName ?? string.Empty,
            roles,
            user.SecurityStamp ?? throw new InvalidOperationException("User security stamp is missing."));

        return new AuthResponse
        {
            Succeeded = true,
            Message = "Login successful.",
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            User = MapUser(user, roles)
        };
    }

    public async Task<UserProfileResponse?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return MapUser(user, roles);
    }

    private static UserProfileResponse MapUser(
        ApplicationUser user,
        IList<string> roles)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles
        };
    }
}