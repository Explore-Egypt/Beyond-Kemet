using ExploreEgypt.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExploreEgypt.Infrastructure.Persistence;

public class ExploreEgyptDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ExploreEgyptDbContext(
        DbContextOptions<ExploreEgyptDbContext> options)
        : base(options)
    {
    }
}
