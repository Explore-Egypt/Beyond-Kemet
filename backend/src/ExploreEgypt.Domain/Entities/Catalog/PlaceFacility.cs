namespace ExploreEgypt.Domain.Entities.Catalog;

public class PlaceFacility
{
    public Guid PlaceFacilityId { get; set; }

    public Guid PlaceId { get; set; }

    public Guid FacilityId { get; set; }

    public decimal? DistanceKm { get; set; }

    public Place Place { get; set; } = null!;

    public Facility Facility { get; set; } = null!;
}
