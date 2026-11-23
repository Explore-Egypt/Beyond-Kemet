using ExploreEgypt.Domain.Entities.Catalog;

namespace ExploreEgypt.Domain.Entities.Engagement;

public class Favorite
{
    public Guid UserId { get; set; }

    public Guid PlaceId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Place Place { get; set; } = null!;
}