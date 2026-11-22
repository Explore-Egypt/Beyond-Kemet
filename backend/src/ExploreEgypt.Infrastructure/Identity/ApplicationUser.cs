using Microsoft.AspNetCore.Identity;

namespace ExploreEgypt.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }
}
