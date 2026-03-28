using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Cards.DTOs;

public record CreateCardRequestDto(
    string Name,
    CardCategory CardCategory,
    CardType CardType,
    string Last4Digits,
    string ExpiryDate,
    Currency Currency,
    string Color,
    decimal? CreditLimit,
    Guid? LinkedAccountId);
