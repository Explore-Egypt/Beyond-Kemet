using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

    public static async Task SeedRolesAsync(IServiceProvider services)
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
    }
}
