using MediatR;
using BudgetTracker.Application.Features.Cards.DTOs;

namespace BudgetTracker.Application.Features.Cards.Queries;

public record GetCardByIdQuery(Guid CardId) : IRequest<CardDto>;
