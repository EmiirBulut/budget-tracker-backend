using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Transactions.DTOs;

public record CreateTransactionRequestDto(
    Guid? AccountId,
    Guid? CardId,
    TransactionType Type,
    decimal Amount,
    string Category,
    string Description,
    DateTime Date);
