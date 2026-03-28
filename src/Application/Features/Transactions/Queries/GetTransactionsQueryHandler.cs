using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Transactions.DTOs;

namespace BudgetTracker.Application.Features.Transactions.Queries;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, PagedTransactionsDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTransactionsQueryHandler(
        ITransactionRepository transactionRepository,
        ICurrentUserService currentUserService)
    {
        _transactionRepository = transactionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedTransactionsDto> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var items = await _transactionRepository.GetByUserIdAsync(
            userId, request.AccountId, request.CardId,
            request.Type, request.StartDate, request.EndDate,
            request.Page, request.PageSize, cancellationToken);

        var total = await _transactionRepository.CountByUserIdAsync(
            userId, request.AccountId, request.CardId,
            request.Type, request.StartDate, request.EndDate, cancellationToken);

        var dtos = items.Select(t => new TransactionDto(
            t.Id, t.AccountId, t.CardId,
            t.Type.ToString(), t.Amount,
            t.Category, t.Description,
            t.Date, t.InstallmentPlanId, t.CreatedAt)).ToList();

        return new PagedTransactionsDto(dtos, total, request.Page, request.PageSize);
    }
}
