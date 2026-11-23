using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExploreEgypt.Infrastructure.Identity;

public static class IdentityRoleSeeder
{
    private static readonly string[] RoleNames =
    {
        "Traveler",
        "Company",
        "Admin"
    };

    public static async Task SeedRolesAsync(
        IServiceProvider services,
        IConfiguration configuration,
        bool allowAdminBootstrap)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var roleName in RoleNames)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new ApplicationRole
                {
                    Name = roleName
                });

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(error =>
                        $"{error.Code}: {error.Description}"));

                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}'. {errors}");
            }
        }

        if (allowAdminBootstrap)
        {
            await BootstrapFirstAdminAsync(
                scope.ServiceProvider,
                configuration);
        }
    }

    private static async Task BootstrapFirstAdminAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var password = configuration["BootstrapAdmin:Password"];

        // Bootstrap is a one-time operation.
        // Once the secret password is removed, existing accounts remain unchanged.
        if (string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var email = configuration["BootstrapAdmin:Email"];

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin Email is required when a Bootstrap password is configured.");
        }

        var userManager = services
            .GetRequiredService<UserManager<ApplicationUser>>();

        email = email.Trim();

        var admin = await userManager.FindByEmailAsync(email);
        var createdNow = false;

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = configuration["BootstrapAdmin:FullName"]
                    ?? "System Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(
                admin,
                password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(error =>
                        $"{error.Code}: {error.Description}"));

                throw new InvalidOperationException(
                    $"Bootstrap Admin creation failed: {errors}");
            }

            createdNow = true;
        }
        else if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            if (!admin.IsActive)
            {
                throw new InvalidOperationException(
                    "The configured Bootstrap Admin account is inactive.");
            }

            var passwordMatches = await userManager.CheckPasswordAsync(
                admin,
                password);

            if (!passwordMatches)
            {
                throw new InvalidOperationException(
                    "Bootstrap Admin credentials do not match the existing account.");
            }
        }

        if (!admin.IsActive)
        {
            throw new InvalidOperationException(
                "The configured Bootstrap Admin account is inactive.");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                admin,
                "Admin");

            if (!roleResult.Succeeded)
            {
                if (createdNow)
                {
                    await userManager.DeleteAsync(admin);
                }

                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(error =>
                        $"{error.Code}: {error.Description}"));

                throw new InvalidOperationException(
                    $"Bootstrap Admin role assignment failed: {errors}");
            }

            var stampResult = await userManager.UpdateSecurityStampAsync(admin);

            if (!stampResult.Succeeded)
            {
                await userManager.RemoveFromRoleAsync(admin, "Admin");

                if (createdNow)
                {
                    await userManager.DeleteAsync(admin);
                }

                throw new InvalidOperationException(
                    "Could not update the Bootstrap Admin security stamp.");
            }
        }
    }
}