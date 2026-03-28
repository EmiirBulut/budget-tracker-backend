using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Cards.DTOs;

namespace BudgetTracker.Application.Features.Cards.Queries;

public class GetCardsQueryHandler : IRequestHandler<GetCardsQuery, List<CardDto>>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCardsQueryHandler(ICardRepository cardRepository, ICurrentUserService currentUserService)
    {
        _cardRepository = cardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<CardDto>> Handle(GetCardsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var cards = await _cardRepository.GetAllByUserIdAsync(userId, cancellationToken);
        return cards.Select(MapToDto).ToList();
    }

    internal static CardDto MapToDto(Domain.Entities.Card c) => new(
        c.Id, c.Name,
        c.CardCategory.ToString(), c.CardType.ToString(),
        c.Last4Digits, c.ExpiryDate,
        c.Currency.ToString(), c.Color,
        c.CreditLimit, c.LinkedAccountId,
        c.IsArchived, c.CreatedAt);
}
