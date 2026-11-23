using ExploreEgypt.Domain.Entities.Catalog;

namespace ExploreEgypt.Domain.Entities.Engagement;

public class PlaceReview
{
    public Guid ReviewId { get; set; }

    public Guid PlaceId { get; set; }

    public Guid UserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Place Place { get; set; } = null!;
}