namespace ExploreEgypt.Domain.Entities.Planner;

public class Itinerary
{
    public Guid ItineraryId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int DurationDays { get; set; }

    public string AreaPreference { get; set; } = string.Empty;

    public decimal? Budget { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ItineraryItem> Items { get; set; }
        = new List<ItineraryItem>();
}