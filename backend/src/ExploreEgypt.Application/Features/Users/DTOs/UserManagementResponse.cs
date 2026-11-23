namespace ExploreEgypt.Application.Features.Users.DTOs;

public class UserManagementResponse
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}