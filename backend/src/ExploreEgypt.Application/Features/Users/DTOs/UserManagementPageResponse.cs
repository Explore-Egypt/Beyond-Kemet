namespace ExploreEgypt.Application.Features.Users.DTOs;

public class UserManagementPageResponse
{
    public IReadOnlyList<UserManagementResponse> Items { get; set; }
        = Array.Empty<UserManagementResponse>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}