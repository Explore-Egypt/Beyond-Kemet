namespace ExploreEgypt.Domain.Entities.Catalog;

public class PlaceImage
{
    public Guid PlaceImageId { get; set; }

    public Guid PlaceId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string AltText { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }

    public Place Place { get; set; } = null!;
}
