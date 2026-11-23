namespace ExploreEgypt.Domain.Entities.Catalog;

public class FacilityType
{
    public int FacilityTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<Facility> Facilities { get; set; }
        = new List<Facility>();
}
