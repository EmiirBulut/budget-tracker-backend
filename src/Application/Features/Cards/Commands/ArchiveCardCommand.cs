using MediatR;

namespace BudgetTracker.Application.Features.Cards.Commands;

public record ArchiveCardCommand(Guid CardId) : IRequest;
