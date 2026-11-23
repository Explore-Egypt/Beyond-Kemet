using ExploreEgypt.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExploreEgypt.Infrastructure.Persistence.Configurations.Catalog;

public sealed class GovernorateConfiguration : IEntityTypeConfiguration<Governorate>
{
    public void Configure(EntityTypeBuilder<Governorate> builder)
    {
        builder.ToTable("Governorates");

        builder.HasKey(x => x.GovernorateId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", table =>
        {
            table.HasCheckConstraint(
                "CK_Categories_NoSelfParent",
                "[ParentCategoryId] IS NULL OR [ParentCategoryId] <> [CategoryId]");
        });

        builder.HasKey(x => x.CategoryId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasOne(x => x.ParentCategory)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
    public void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.ToTable("Places", table =>
        {
            table.HasCheckConstraint(
                "CK_Places_CoordinateRange",
                "[Latitude] BETWEEN -90 AND 90 AND [Longitude] BETWEEN -180 AND 180");
        });

        builder.HasKey(x => x.PlaceId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6);

        builder.HasOne(x => x.Governorate)
            .WithMany(x => x.Places)
            .HasForeignKey(x => x.GovernorateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Places)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.GovernorateId, x.CategoryId });

        builder.HasIndex(x => x.IsPublished);
    }
}

public sealed class PlaceImageConfiguration : IEntityTypeConfiguration<PlaceImage>
{
    public void Configure(EntityTypeBuilder<PlaceImage> builder)
    {
        builder.ToTable("PlaceImages");

        builder.HasKey(x => x.PlaceImageId);

        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.AltText)
            .HasMaxLength(500);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.PlaceId, x.DisplayOrder });

        builder.HasIndex(x => x.PlaceId)
            .IsUnique()
            .HasFilter("[IsPrimary] = 1");
    }
}

public sealed class PlaceFeeConfiguration : IEntityTypeConfiguration<PlaceFee>
{
    public void Configure(EntityTypeBuilder<PlaceFee> builder)
    {
        builder.ToTable("PlaceFees", table =>
        {
            table.HasCheckConstraint(
                "CK_PlaceFees_NonNegativeAmount",
                "[Amount] >= 0");

            table.HasCheckConstraint(
                "CK_PlaceFees_ValidDateRange",
                "[ValidFrom] IS NULL OR [ValidTo] IS NULL OR [ValidTo] >= [ValidFrom]");
        });

        builder.HasKey(x => x.PlaceFeeId);

        builder.Property(x => x.FeeName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.Fees)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.PlaceId, x.IsActive });
    }
}

public sealed class FacilityTypeConfiguration : IEntityTypeConfiguration<FacilityType>
{
    public void Configure(EntityTypeBuilder<FacilityType> builder)
    {
        builder.ToTable("FacilityTypes");

        builder.HasKey(x => x.FacilityTypeId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}

public sealed class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facilities", table =>
        {
            table.HasCheckConstraint(
                "CK_Facilities_CoordinateRange",
                "([Latitude] IS NULL AND [Longitude] IS NULL) OR " +
                "([Latitude] IS NOT NULL AND [Longitude] IS NOT NULL " +
                "AND [Latitude] BETWEEN -90 AND 90 AND [Longitude] BETWEEN -180 AND 180)");
        });

        builder.HasKey(x => x.FacilityId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6);

        builder.HasOne(x => x.FacilityType)
            .WithMany(x => x.Facilities)
            .HasForeignKey(x => x.FacilityTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PlaceFacilityConfiguration : IEntityTypeConfiguration<PlaceFacility>
{
    public void Configure(EntityTypeBuilder<PlaceFacility> builder)
    {
        builder.ToTable("PlaceFacilities", table =>
        {
            table.HasCheckConstraint(
                "CK_PlaceFacilities_NonNegativeDistance",
                "[DistanceKm] IS NULL OR [DistanceKm] >= 0");
        });

        builder.HasKey(x => x.PlaceFacilityId);

        builder.Property(x => x.DistanceKm)
            .HasPrecision(8, 2);

        builder.HasOne(x => x.Place)
            .WithMany(x => x.PlaceFacilities)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Facility)
            .WithMany(x => x.PlaceFacilities)
            .HasForeignKey(x => x.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.PlaceId, x.FacilityId })
            .IsUnique();
    }
}