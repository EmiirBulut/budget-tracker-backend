namespace BudgetTracker.Application.Features.Transactions.DTOs;

public record TransactionDto(
    Guid Id,
    Guid? AccountId,
    Guid? CardId,
    string Type,
    decimal Amount,
    string Category,
    string Description,
    DateTime Date,
    Guid? InstallmentPlanId,
    DateTime CreatedAt);
