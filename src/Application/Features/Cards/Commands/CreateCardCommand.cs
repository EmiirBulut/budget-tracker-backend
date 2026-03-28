using MediatR;
using BudgetTracker.Application.Features.Cards.DTOs;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Cards.Commands;

public record CreateCardCommand(
    string Name,
    CardCategory CardCategory,
    CardType CardType,
    string Last4Digits,
    string ExpiryDate,
    Currency Currency,
    string Color,
    decimal? CreditLimit,
    Guid? LinkedAccountId) : IRequest<CardDto>;
