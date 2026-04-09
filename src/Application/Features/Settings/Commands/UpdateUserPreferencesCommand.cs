using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Settings.DTOs;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Settings.Commands;

public record UpdateUserPreferencesCommand(
    Currency DefaultCurrency,
    Language Language,
    bool NotificationsEnabled) : IRequest<UserPreferencesDto>;

public class UpdateUserPreferencesCommandValidator : AbstractValidator<UpdateUserPreferencesCommand>
{
    public UpdateUserPreferencesCommandValidator()
    {
        RuleFor(x => x.DefaultCurrency)
            .IsInEnum().WithMessage("Currency is invalid.");

        RuleFor(x => x.Language)
            .IsInEnum().WithMessage("Language is invalid.");
    }
}

public class UpdateUserPreferencesCommandHandler : IRequestHandler<UpdateUserPreferencesCommand, UserPreferencesDto>
{
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateUserPreferencesCommandHandler> _logger;

    public UpdateUserPreferencesCommandHandler(
        IUserPreferencesRepository preferencesRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateUserPreferencesCommandHandler> logger)
    {
        _preferencesRepository = preferencesRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<UserPreferencesDto> Handle(UpdateUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);

        if (preferences is null)
        {
            preferences = UserPreferences.CreateDefault(userId);
            await _preferencesRepository.AddAsync(preferences, cancellationToken);
        }

        preferences.Update(request.DefaultCurrency, request.Language, request.NotificationsEnabled);
        await _preferencesRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Preferences updated for user {UserId}", userId);

        return new UserPreferencesDto(
            preferences.DefaultCurrency,
            preferences.Language,
            preferences.NotificationsEnabled);
    }
}
