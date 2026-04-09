using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Settings.DTOs;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Features.Settings.Queries;

public record GetUserPreferencesQuery : IRequest<UserPreferencesDto>;

public class GetUserPreferencesQueryHandler : IRequestHandler<GetUserPreferencesQuery, UserPreferencesDto>
{
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetUserPreferencesQueryHandler> _logger;

    public GetUserPreferencesQueryHandler(
        IUserPreferencesRepository preferencesRepository,
        ICurrentUserService currentUserService,
        ILogger<GetUserPreferencesQueryHandler> logger)
    {
        _preferencesRepository = preferencesRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<UserPreferencesDto> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);

        if (preferences is null)
        {
            preferences = UserPreferences.CreateDefault(userId);
            await _preferencesRepository.AddAsync(preferences, cancellationToken);
            await _preferencesRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created default preferences for user {UserId}", userId);
        }

        return new UserPreferencesDto(
            preferences.DefaultCurrency,
            preferences.Language,
            preferences.NotificationsEnabled);
    }
}
