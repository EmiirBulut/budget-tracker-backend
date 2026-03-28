using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Cards.Commands;

public class ArchiveCardCommandHandler : IRequestHandler<ArchiveCardCommand>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICurrentUserService _currentUserService;

    public ArchiveCardCommandHandler(ICardRepository cardRepository, ICurrentUserService currentUserService)
    {
        _cardRepository = cardRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ArchiveCardCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var card = await _cardRepository.GetByIdAsync(request.CardId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Card), request.CardId);

        card.Archive();
        await _cardRepository.SaveChangesAsync(cancellationToken);
    }
}
