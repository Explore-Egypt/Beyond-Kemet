using ExploreEgypt.Domain.Entities.Catalog;

namespace ExploreEgypt.Domain.Entities.Marketplace;

public class TourStop
{
    public Guid TourStopId { get; set; }

    public Guid TourId { get; set; }

    public Guid PlaceId { get; set; }

    public int StopOrder { get; set; }

    public Tour Tour { get; set; } = null!;

    public Place Place { get; set; } = null!;
}