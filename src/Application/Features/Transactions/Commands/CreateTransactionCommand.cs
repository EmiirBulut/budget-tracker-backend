using MediatR;
using BudgetTracker.Application.Features.Transactions.DTOs;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Transactions.Commands;

public record CreateTransactionCommand(
    Guid? AccountId,
    Guid? CardId,
    TransactionType Type,
    decimal Amount,
    string Category,
    string Description,
    DateTime Date) : IRequest<TransactionDto>;
