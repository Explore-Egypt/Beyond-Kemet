using ExploreEgypt.Domain.Entities.Engagement;
using ExploreEgypt.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExploreEgypt.Infrastructure.Persistence.Configurations.Engagement;

public sealed class PlaceReviewConfiguration : IEntityTypeConfiguration<PlaceReview>
{
    public void Configure(EntityTypeBuilder<PlaceReview> builder)
    {
        builder.ToTable("PlaceReviews", table =>
        {
            table.HasCheckConstraint(
                "CK_PlaceReviews_RatingRange",
                "[Rating] BETWEEN 1 AND 5");
        });

        builder.HasKey(x => x.ReviewId);

        builder.Property(x => x.Comment)
            .HasMaxLength(2000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(x => x.Place)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.PlaceId, x.UserId })
            .IsUnique();
    }
}

public sealed class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");

        builder.HasKey(x => new { x.UserId, x.PlaceId });

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(x => x.Place)
            .WithMany(x => x.Favorites)
            .HasForeignKey(x => x.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PlaceId);
    }
}