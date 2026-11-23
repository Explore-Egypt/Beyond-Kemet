using ExploreEgypt.Domain.Entities.Catalog;
using ExploreEgypt.Domain.Entities.Engagement;
using ExploreEgypt.Domain.Entities.Marketplace;
using ExploreEgypt.Domain.Entities.Planner;
using ExploreEgypt.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using BookingEntity = ExploreEgypt.Domain.Entities.Booking.Booking;

namespace ExploreEgypt.Infrastructure.Persistence;

public class ExploreEgyptDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ExploreEgyptDbContext(
        DbContextOptions<ExploreEgyptDbContext> options)
        : base(options)
    {
    }

    // Catalog
    public DbSet<Governorate> Governorates => Set<Governorate>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Place> Places => Set<Place>();
    public DbSet<PlaceImage> PlaceImages => Set<PlaceImage>();
    public DbSet<PlaceFee> PlaceFees => Set<PlaceFee>();
    public DbSet<FacilityType> FacilityTypes => Set<FacilityType>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<PlaceFacility> PlaceFacilities => Set<PlaceFacility>();

    // Marketplace
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourStop> TourStops => Set<TourStop>();
    public DbSet<Departure> Departures => Set<Departure>();

    // Booking
    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();

    // Engagement
    public DbSet<PlaceReview> PlaceReviews => Set<PlaceReview>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    // Planner
    public DbSet<Itinerary> Itineraries => Set<Itinerary>();
    public DbSet<ItineraryItem> ItineraryItems => Set<ItineraryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Preserve ASP.NET Identity mappings.
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration classes in Infrastructure.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ExploreEgyptDbContext).Assembly);
    }
}