using System.Security.Claims;
using ExploreEgypt.Application.Features.Users.DTOs;
using ExploreEgypt.Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExploreEgypt.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var profile = await _userManagementService.GetMyProfileAsync(userId);

        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateMyProfileRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _userManagementService.UpdateMyProfileAsync(
            userId,
            request);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangeMyPassword(
        [FromBody] ChangeMyPasswordRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _userManagementService.ChangeMyPasswordAsync(
            userId,
            request);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManagementService.GetUsersAsync(
            search,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetUserById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManagementService.GetUserByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPatch("{userId:guid}/status")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateUserStatus(
        Guid userId,
        [FromBody] UpdateUserStatusRequest request)
    {
        if (!TryGetCurrentUserId(out var adminUserId))
        {
            return Unauthorized();
        }

        if (request.IsActive is not bool isActive)
        {
            return BadRequest(new { Message = "IsActive is required." });
        }

        var result = await _userManagementService.UpdateUserStatusAsync(
            userId,
            adminUserId,
            isActive);

        return result.Succeeded ? Ok(result) : Conflict(result);
    }

    [HttpPut("{userId:guid}/roles")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateUserRoles(
        Guid userId,
        [FromBody] UpdateUserRolesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userManagementService.UpdateUserRolesAsync(
            userId,
            request.Roles,
            cancellationToken);

        return result.Succeeded ? Ok(result) : Conflict(result);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}