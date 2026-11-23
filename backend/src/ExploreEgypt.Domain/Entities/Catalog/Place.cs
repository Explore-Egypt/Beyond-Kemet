using ExploreEgypt.Domain.Entities.Engagement;
using ExploreEgypt.Domain.Entities.Marketplace;
using ExploreEgypt.Domain.Entities.Planner;

namespace ExploreEgypt.Domain.Entities.Catalog;

public class Place
{
    public Guid PlaceId { get; set; }

    public int GovernorateId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public bool IsHiddenGem { get; set; }

    public bool IsPublished { get; set; }

    // Parent entities
    public Governorate Governorate { get; set; } = null!;

    public Category Category { get; set; } = null!;

    // Catalog relationships
    public ICollection<PlaceImage> Images { get; set; }
        = new List<PlaceImage>();

    public ICollection<PlaceFee> Fees { get; set; }
        = new List<PlaceFee>();

    public ICollection<PlaceFacility> PlaceFacilities { get; set; }
        = new List<PlaceFacility>();

    // Marketplace relationship
    public ICollection<TourStop> TourStops { get; set; }
        = new List<TourStop>();

    // Engagement relationships
    public ICollection<PlaceReview> Reviews { get; set; }
        = new List<PlaceReview>();

    public ICollection<Favorite> Favorites { get; set; }
        = new List<Favorite>();

    // Planner relationship
    public ICollection<ItineraryItem> ItineraryItems { get; set; }
        = new List<ItineraryItem>();
}