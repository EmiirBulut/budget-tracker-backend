using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Settings.DTOs;

public record UserPreferencesDto(
    Currency DefaultCurrency,
    Language Language,
    bool NotificationsEnabled);
