using ExploreEgypt.Domain.Entities.Catalog;

namespace ExploreEgypt.Domain.Entities.Marketplace;

public class Tour
{
    public Guid TourId { get; set; }

    public Guid CompanyId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal PricePerPerson { get; set; }

    public string Currency { get; set; } = string.Empty;

    public int DurationDays { get; set; }

    public bool IsPublished { get; set; }

    public Company Company { get; set; } = null!;

    public ICollection<TourStop> Stops { get; set; }
        = new List<TourStop>();

    public ICollection<Departure> Departures { get; set; }
        = new List<Departure>();
}