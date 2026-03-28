using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Application.Features.Cards.Commands;
using BudgetTracker.Application.Features.Cards.DTOs;
using BudgetTracker.Application.Features.Cards.Queries;

namespace BudgetTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ISender _sender;

    public CardsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CardDto>>> GetCards(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCardsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardDto>> GetCardById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCardByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CardDto>> CreateCard([FromBody] CreateCardRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateCardCommand(
                request.Name, request.CardCategory, request.CardType,
                request.Last4Digits, request.ExpiryDate, request.Currency,
                request.Color, request.CreditLimit, request.LinkedAccountId),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardDto>> UpdateCard(Guid id, [FromBody] UpdateCardRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateCardCommand(
                id, request.Name, request.CardType,
                request.Last4Digits, request.ExpiryDate, request.Currency,
                request.Color, request.CreditLimit, request.LinkedAccountId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveCard(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new ArchiveCardCommand(id), cancellationToken);
        return NoContent();
    }
}
