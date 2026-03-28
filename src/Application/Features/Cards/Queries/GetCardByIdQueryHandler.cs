using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Cards.DTOs;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Cards.Queries;

public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCardByIdQueryHandler(ICardRepository cardRepository, ICurrentUserService currentUserService)
    {
        _cardRepository = cardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CardDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var card = await _cardRepository.GetByIdAsync(request.CardId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Card), request.CardId);
        return GetCardsQueryHandler.MapToDto(card);
    }
}
