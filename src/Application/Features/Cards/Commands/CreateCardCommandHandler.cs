using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Cards.DTOs;
using BudgetTracker.Application.Features.Cards.Queries;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Cards.Commands;

public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, CardDto>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateCardCommandHandler> _logger;

    public CreateCardCommandHandler(
        ICardRepository cardRepository,
        ICurrentUserService currentUserService,
        ILogger<CreateCardCommandHandler> logger)
    {
        _cardRepository = cardRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CardDto> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        // Business rule: credit cards require a limit; debit cards require a linked account.
        if (request.CardCategory == CardCategory.Credit && request.CreditLimit is null)
            throw new DomainException("Credit limit is required for credit cards.");

        if (request.CardCategory == CardCategory.Debit && request.LinkedAccountId is null)
            throw new DomainException("A linked account is required for debit cards.");

        var card = Card.Create(
            userId, request.Name,
            request.CardCategory, request.CardType,
            request.Last4Digits, request.ExpiryDate,
            request.Currency, request.Color,
            request.CreditLimit, request.LinkedAccountId);

        await _cardRepository.AddAsync(card, cancellationToken);
        await _cardRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Card created: {CardId} for user: {UserId}", card.Id, userId);

        return GetCardsQueryHandler.MapToDto(card);
    }
}
