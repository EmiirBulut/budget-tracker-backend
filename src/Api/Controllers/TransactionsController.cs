using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Application.Features.Transactions.Commands;
using BudgetTracker.Application.Features.Transactions.DTOs;
using BudgetTracker.Application.Features.Transactions.Queries;

namespace BudgetTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public TransactionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedTransactionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedTransactionsDto>> GetTransactions(
        [FromQuery] Guid? accountId,
        [FromQuery] Guid? cardId,
        [FromQuery] string? type,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetTransactionsQuery(accountId, cardId, type, startDate, endDate, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionDto>> CreateTransaction(
        [FromBody] CreateTransactionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateTransactionCommand(
                request.AccountId, request.CardId, request.Type,
                request.Amount, request.Category, request.Description, request.Date),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
