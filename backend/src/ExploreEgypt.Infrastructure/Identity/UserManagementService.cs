using ExploreEgypt.Application.Features.Authentication.DTOs;
using ExploreEgypt.Application.Features.Users.DTOs;
using ExploreEgypt.Application.Features.Users.Interfaces;
using ExploreEgypt.Domain.Entities.Marketplace;
using ExploreEgypt.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExploreEgypt.Infrastructure.Identity;

public class UserManagementService : IUserManagementService
{
    private static readonly string[] AssignableRoles =
    {
        "Traveler",
        "Company"
    };

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ExploreEgyptDbContext _dbContext;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        ExploreEgyptDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<UserProfileResponse?> GetMyProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return MapProfile(user, roles);
    }

    public async Task<UserManagementResult> UpdateMyProfileAsync(
        Guid userId,
        UpdateMyProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Failure("User not found.");
        }

        user.FullName = request.FullName.Trim();

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return IdentityFailure("Profile update failed.", updateResult);
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserManagementResult
        {
            Succeeded = true,
            Message = "Profile updated successfully.",
            Profile = MapProfile(user, roles)
        };
    }

    public async Task<UserManagementResult> ChangeMyPasswordAsync(
        Guid userId,
        ChangeMyPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Failure("User not found.");
        }

        var changeResult = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!changeResult.Succeeded)
        {
            return IdentityFailure("Password change failed.", changeResult);
        }

        var stampResult = await _userManager.UpdateSecurityStampAsync(user);

        if (!stampResult.Succeeded)
        {
            return IdentityFailure(
                "Password changed, but security token invalidation failed.",
                stampResult);
        }

        return new UserManagementResult
        {
            Succeeded = true,
            Message = "Password changed successfully. Please sign in again."
        };
    }

    public async Task<UserManagementPageResponse> GetUsersAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(user =>
                (user.FullName != null && user.FullName.Contains(term)) ||
                (user.Email != null && user.Email.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)pageSize);

        // Avoid integer overflow when a client requests an extremely
        // large page number.
        if (page > totalPages)
        {
            return new UserManagementPageResponse
            {
                Items = Array.Empty<UserManagementResponse>(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        var skip = (int)(((long)page - 1) * pageSize);

        var users = await query
            .OrderByDescending(user => user.CreatedAt)
            .ThenBy(user => user.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = new List<UserManagementResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(MapManagedUser(user, roles));
        }

        return new UserManagementPageResponse
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
    public async Task<UserManagementResponse?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.Id == userId,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return MapManagedUser(user, roles);
    }

    public async Task<UserManagementResult> UpdateUserStatusAsync(
        Guid targetUserId,
        Guid adminUserId,
        bool isActive)
    {
        var user = await _userManager.FindByIdAsync(targetUserId.ToString());

        if (user is null)
        {
            return Failure("User not found.");
        }

        if (targetUserId == adminUserId && !isActive)
        {
            return Failure("You cannot deactivate your own account.");
        }

        if (user.IsActive == isActive)
        {
            return new UserManagementResult
            {
                Succeeded = true,
                Message = "Account status is already set to the requested value.",
                User = await MapManagedUserAsync(user)
            };
        }

        if (!isActive && await _userManager.IsInRoleAsync(user, "Admin"))
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var activeAdmins = admins.Count(item => item.IsActive);

            if (activeAdmins <= 1)
            {
                return Failure("The last active Admin account cannot be deactivated.");
            }
        }

        user.IsActive = isActive;
        user.SecurityStamp = Guid.NewGuid().ToString("N");

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return IdentityFailure("Account status update failed.", updateResult);
        }

        return new UserManagementResult
        {
            Succeeded = true,
            Message = isActive
                ? "User account activated successfully."
                : "User account deactivated successfully. Existing tokens have been revoked.",
            User = await MapManagedUserAsync(user)
        };
    }

    public async Task<UserManagementResult> UpdateUserRolesAsync(
        Guid targetUserId,
        IReadOnlyCollection<string> requestedRoles,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(targetUserId.ToString());

        if (user is null)
        {
            return Failure("User not found.");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase))
        {
            return Failure(
                "Admin role assignments cannot be changed through this endpoint.");
        }

        var normalizedRoles = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var requestedRole in requestedRoles)
        {
            if (string.IsNullOrWhiteSpace(requestedRole))
            {
                continue;
            }

            var canonicalRole = AssignableRoles.FirstOrDefault(role =>
                string.Equals(
                    role,
                    requestedRole.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (canonicalRole is null)
            {
                return Failure(
                    "Only Traveler and Company roles can be managed through this endpoint.");
            }

            normalizedRoles.Add(canonicalRole);
        }

        if (normalizedRoles.Count == 0)
        {
            return Failure("At least one valid role must be assigned.");
        }

        if (normalizedRoles.Contains("Company"))
        {
            var hasApprovedCompanyMembership =
                await _dbContext.CompanyMembers.AnyAsync(
                    member =>
                        member.UserId == targetUserId &&
                        member.Company.VerificationStatus ==
                            CompanyVerificationStatus.Approved,
                    cancellationToken);

            if (!hasApprovedCompanyMembership)
            {
                return Failure(
                    "Company role requires membership in an approved company.");
            }
        }

        var rolesToRemove = currentRoles
            .Where(role => !normalizedRoles.Contains(role))
            .ToArray();

        var rolesToAdd = normalizedRoles
            .Where(role => !currentRoles.Contains(
                role,
                StringComparer.OrdinalIgnoreCase))
            .ToArray();

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        if (rolesToRemove.Length > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(
                user,
                rolesToRemove);

            if (!removeResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);

                return IdentityFailure(
                    "Could not remove existing roles.",
                    removeResult);
            }
        }

        if (rolesToAdd.Length > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(
                user,
                rolesToAdd);

            if (!addResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);

                return IdentityFailure(
                    "Could not assign requested roles.",
                    addResult);
            }
        }

        var stampResult = await _userManager.UpdateSecurityStampAsync(user);

        if (!stampResult.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);

            return IdentityFailure(
                "Role changes could not invalidate existing tokens.",
                stampResult);
        }

        await transaction.CommitAsync(cancellationToken);

        return new UserManagementResult
        {
            Succeeded = true,
            Message = "User roles updated successfully. Existing tokens have been revoked.",
            User = await MapManagedUserAsync(user)
        };
    }

    private async Task<UserManagementResponse> MapManagedUserAsync(
        ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return MapManagedUser(user, roles);
    }

    private static UserManagementResponse MapManagedUser(
        ApplicationUser user,
        IList<string> roles)
    {
        return new UserManagementResponse
        {
            UserId = user.Id,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToArray()
        };
    }

    private static UserProfileResponse MapProfile(
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

    private static UserManagementResult Failure(string message)
    {
        return new UserManagementResult
        {
            Succeeded = false,
            Message = message
        };
    }

    private static UserManagementResult IdentityFailure(
        string message,
        IdentityResult identityResult)
    {
        return new UserManagementResult
        {
            Succeeded = false,
            Message = message,
            Errors = identityResult.Errors
                .Select(error => error.Description)
                .ToList()
        };
    }
}