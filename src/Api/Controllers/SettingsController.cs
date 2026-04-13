using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Application.Features.Settings.Commands;
using BudgetTracker.Application.Features.Settings.DTOs;
using BudgetTracker.Application.Features.Settings.Queries;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISender _sender;

    public SettingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateProfileCommand(request.Email), cancellationToken);
        return NoContent();
    }

    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword), cancellationToken);
        return NoContent();
    }

    [HttpGet("preferences")]
    [ProducesResponseType(typeof(UserPreferencesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // Prevent browsers from caching personalized preference responses.
    // Without this, a previously authenticated response (200) is reused by the browser
    // as a 304 on subsequent requests, even when the user is no longer authenticated.
    // See docs/infinite-preferences-loop.md for full root-cause analysis.
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<UserPreferencesDto>> GetPreferences(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserPreferencesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("preferences")]
    [ProducesResponseType(typeof(UserPreferencesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserPreferencesDto>> UpdatePreferences(
        [FromBody] UpdateUserPreferencesRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateUserPreferencesCommand(request.DefaultCurrency, request.Language, request.NotificationsEnabled),
            cancellationToken);
        return Ok(result);
    }
}

public record UpdateProfileRequestDto(string Email);
public record ChangePasswordRequestDto(string CurrentPassword, string NewPassword);
public record UpdateUserPreferencesRequestDto(Currency DefaultCurrency, Language Language, bool NotificationsEnabled);
