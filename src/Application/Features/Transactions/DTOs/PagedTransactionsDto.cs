namespace BudgetTracker.Application.Features.Transactions.DTOs;

public record PagedTransactionsDto(
    List<TransactionDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
