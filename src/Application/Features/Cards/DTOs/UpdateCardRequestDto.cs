using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Cards.DTOs;

public record UpdateCardRequestDto(
    string Name,
    CardType CardType,
    string Last4Digits,
    string ExpiryDate,
    Currency Currency,
    string Color,
    decimal? CreditLimit,
    Guid? LinkedAccountId);
