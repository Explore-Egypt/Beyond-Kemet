namespace ExploreEgypt.Domain.Entities.Catalog;

public class Category
{
    public int CategoryId { get; set; }

    public int? ParentCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Category? ParentCategory { get; set; }

    public ICollection<Category> SubCategories { get; set; }
        = new List<Category>();

    public ICollection<Place> Places { get; set; } = new List<Place>();
}
