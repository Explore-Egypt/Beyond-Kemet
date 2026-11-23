namespace ExploreEgypt.Domain.Entities.Catalog;

public class PlaceFee
{
    public Guid PlaceFeeId { get; set; }

    public Guid PlaceId { get; set; }

    public string FeeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; }

    public Place Place { get; set; } = null!;
}
