namespace ExploreEgypt.Domain.Entities.Catalog;

public class Facility
{
    public Guid FacilityId { get; set; }

    public int FacilityTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public FacilityType FacilityType { get; set; } = null!;

    public ICollection<PlaceFacility> PlaceFacilities { get; set; }
        = new List<PlaceFacility>();
}
