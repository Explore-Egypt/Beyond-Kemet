namespace ExploreEgypt.Domain.Entities.Catalog;

public class Governorate
{
    public int GovernorateId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<Place> Places { get; set; } = new List<Place>();
}
