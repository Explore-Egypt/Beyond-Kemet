namespace ExploreEgypt.Domain.Entities.Marketplace;

public class Company
{
    public Guid CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public CompanyVerificationStatus VerificationStatus { get; set; }
        = CompanyVerificationStatus.Pending;

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CompanyMember> Members { get; set; }
        = new List<CompanyMember>();

    public ICollection<Tour> Tours { get; set; }
        = new List<Tour>();
}