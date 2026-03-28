using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Cards.DTOs;

public record CardDto(
    Guid Id,
    string Name,
    string CardCategory,
    string CardType,
    string Last4Digits,
    string ExpiryDate,
    string Currency,
    string Color,
    decimal? CreditLimit,
    Guid? LinkedAccountId,
    bool IsArchived,
    DateTime CreatedAt);
