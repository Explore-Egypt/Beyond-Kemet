namespace ExploreEgypt.Domain.Entities.Marketplace;

public class CompanyMember
{
    public Guid CompanyMemberId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid UserId { get; set; }

    public bool IsPrimaryContact { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
}