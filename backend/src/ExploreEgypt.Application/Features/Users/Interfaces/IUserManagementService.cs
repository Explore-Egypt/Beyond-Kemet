using ExploreEgypt.Application.Features.Authentication.DTOs;
using ExploreEgypt.Application.Features.Users.DTOs;

namespace ExploreEgypt.Application.Features.Users.Interfaces;

public interface IUserManagementService
{
    Task<UserProfileResponse?> GetMyProfileAsync(Guid userId);

    Task<UserManagementResult> UpdateMyProfileAsync(
        Guid userId,
        UpdateMyProfileRequest request);

    Task<UserManagementResult> ChangeMyPasswordAsync(
        Guid userId,
        ChangeMyPasswordRequest request);

    Task<UserManagementPageResponse> GetUsersAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<UserManagementResponse?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserManagementResult> UpdateUserStatusAsync(
        Guid targetUserId,
        Guid adminUserId,
        bool isActive);

    Task<UserManagementResult> UpdateUserRolesAsync(
        Guid targetUserId,
        IReadOnlyCollection<string> requestedRoles,
        CancellationToken cancellationToken = default);
}