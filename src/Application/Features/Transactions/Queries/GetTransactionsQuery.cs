using MediatR;
using BudgetTracker.Application.Features.Transactions.DTOs;

namespace BudgetTracker.Application.Features.Transactions.Queries;

public record GetTransactionsQuery(
    Guid? AccountId,
    Guid? CardId,
    string? Type,
    DateTime? StartDate,
    DateTime? EndDate,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedTransactionsDto>;
