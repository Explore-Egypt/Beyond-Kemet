using ExploreEgypt.Domain.Entities.Planner;
using ExploreEgypt.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExploreEgypt.Infrastructure.Persistence.Configurations.Planner;

public sealed class ItineraryConfiguration : IEntityTypeConfiguration<Itinerary>
{
    public void Configure(EntityTypeBuilder<Itinerary> builder)
    {
        builder.ToTable("Itineraries", table =>
        {
            table.HasCheckConstraint(
                "CK_Itineraries_PositiveDurationDays",
                "[DurationDays] > 0");

            table.HasCheckConstraint(
                "CK_Itineraries_NonNegativeBudget",
                "[Budget] IS NULL OR [Budget] >= 0");
        });

        builder.HasKey(x => x.ItineraryId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AreaPreference)
            .HasMaxLength(200);

        builder.Property(x => x.Budget)
            .HasPrecision(18, 2);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ItineraryItemConfiguration : IEntityTypeConfiguration<ItineraryItem>
{
    public void Configure(EntityTypeBuilder<ItineraryItem> builder)
    {
        builder.ToTable("ItineraryItems", table =>
        {
            table.HasCheckConstraint(
                "CK_ItineraryItems_PositiveDayAndOrder",
                "[DayNumber] > 0 AND [ItemOrder] > 0");
        });

        builder.HasKey(x => x.ItineraryItemId);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Itinerary)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ItineraryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.ItineraryItems)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ItineraryId,
            x.DayNumber,
            x.ItemOrder
        }).IsUnique();
    }
}