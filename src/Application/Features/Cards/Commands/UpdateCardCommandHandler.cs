using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Cards.DTOs;
using BudgetTracker.Application.Features.Cards.Queries;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Cards.Commands;

public class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, CardDto>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCardCommandHandler(ICardRepository cardRepository, ICurrentUserService currentUserService)
    {
        _cardRepository = cardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CardDto> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var card = await _cardRepository.GetByIdAsync(request.CardId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Card), request.CardId);

        card.Update(
            request.Name, request.CardType,
            request.Last4Digits, request.ExpiryDate,
            request.Currency, request.Color,
            request.CreditLimit, request.LinkedAccountId);

        await _cardRepository.SaveChangesAsync(cancellationToken);
        return GetCardsQueryHandler.MapToDto(card);
    }
}
