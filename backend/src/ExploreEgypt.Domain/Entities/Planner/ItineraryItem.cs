using ExploreEgypt.Domain.Entities.Catalog;

namespace ExploreEgypt.Domain.Entities.Planner;

public class ItineraryItem
{
    public Guid ItineraryItemId { get; set; }

    public Guid ItineraryId { get; set; }

    public Guid PlaceId { get; set; }

    public int DayNumber { get; set; }

    public int ItemOrder { get; set; }

    public string Notes { get; set; } = string.Empty;

    public Itinerary Itinerary { get; set; } = null!;

    public Place Place { get; set; } = null!;
}