using BookingEntity = ExploreEgypt.Domain.Entities.Booking.Booking;
using ExploreEgypt.Domain.Entities.Booking;
using ExploreEgypt.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExploreEgypt.Infrastructure.Persistence.Configurations.Booking;

public sealed class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.ToTable("Bookings", table =>
        {
            table.HasCheckConstraint(
                "CK_Bookings_PositiveParticipantCount",
                "[ParticipantCount] > 0");

            table.HasCheckConstraint(
                "CK_Bookings_NonNegativePrices",
                "[UnitPriceSnapshot] >= 0 AND [TotalPriceSnapshot] >= 0");

            table.HasCheckConstraint(
                "CK_Bookings_TotalPriceSnapshot",
                "[TotalPriceSnapshot] = [UnitPriceSnapshot] * [ParticipantCount]");

            table.HasCheckConstraint(
                "CK_Bookings_ValidStatus",
                "[Status] IN ('Pending', 'Confirmed', 'Rejected', 'Cancelled')");
        });

        builder.HasKey(x => x.BookingId);

        builder.Property(x => x.UnitPriceSnapshot)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalPriceSnapshot)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue(BookingStatus.Pending);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(x => x.Departure)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.DepartureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.TravelerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TravelerId, x.CreatedAt });

        builder.HasIndex(x => new { x.DepartureId, x.Status });
    }
}