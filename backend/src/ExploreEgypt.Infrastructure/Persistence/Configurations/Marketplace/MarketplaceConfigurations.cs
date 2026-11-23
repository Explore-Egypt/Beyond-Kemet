using ExploreEgypt.Domain.Entities.Marketplace;
using ExploreEgypt.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExploreEgypt.Infrastructure.Persistence.Configurations.Marketplace;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies", table =>
        {
            table.HasCheckConstraint(
                "CK_Companies_ValidVerificationStatus",
                "[VerificationStatus] IN ('Pending', 'Approved', 'Rejected')");
        });

        builder.HasKey(x => x.CompanyId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.LicenseNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.VerificationStatus)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue(CompanyVerificationStatus.Pending);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
{
    public void Configure(EntityTypeBuilder<CompanyMember> builder)
    {
        builder.ToTable("CompanyMembers");

        builder.HasKey(x => x.CompanyMemberId);

        builder.Property(x => x.JoinedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CompanyId, x.UserId })
            .IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}

public sealed class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tours", table =>
        {
            table.HasCheckConstraint(
                "CK_Tours_NonNegativePrice",
                "[PricePerPerson] >= 0");

            table.HasCheckConstraint(
                "CK_Tours_PositiveDurationDays",
                "[DurationDays] > 0");
        });

        builder.HasKey(x => x.TourId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.PricePerPerson)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Tours)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class TourStopConfiguration : IEntityTypeConfiguration<TourStop>
{
    public void Configure(EntityTypeBuilder<TourStop> builder)
    {
        builder.ToTable("TourStops", table =>
        {
            table.HasCheckConstraint(
                "CK_TourStops_PositiveStopOrder",
                "[StopOrder] > 0");
        });

        builder.HasKey(x => x.TourStopId);

        builder.HasOne(x => x.Tour)
            .WithMany(x => x.Stops)
            .HasForeignKey(x => x.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.TourStops)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TourId, x.StopOrder })
            .IsUnique();
    }
}

public sealed class DepartureConfiguration : IEntityTypeConfiguration<Departure>
{
    public void Configure(EntityTypeBuilder<Departure> builder)
    {
        builder.ToTable("Departures", table =>
        {
            table.HasCheckConstraint(
                "CK_Departures_EndAfterStart",
                "[EndAt] > [StartAt]");

            table.HasCheckConstraint(
                "CK_Departures_PositiveCapacity",
                "[Capacity] > 0");

            table.HasCheckConstraint(
                "CK_Departures_ValidStatus",
                "[Status] IN ('Open', 'Closed', 'Cancelled', 'Completed')");
        });

        builder.HasKey(x => x.DepartureId);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue(DepartureStatus.Open);

        builder.HasOne(x => x.Tour)
            .WithMany(x => x.Departures)
            .HasForeignKey(x => x.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TourId, x.StartAt });
    }
}