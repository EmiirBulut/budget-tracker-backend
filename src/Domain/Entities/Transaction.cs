using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? AccountId { get; private set; }
    public Guid? CardId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public Guid? InstallmentPlanId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(
        Guid userId,
        Guid? accountId,
        Guid? cardId,
        TransactionType type,
        decimal amount,
        string category,
        string description,
        DateTime date,
        Guid? installmentPlanId = null)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccountId = accountId,
            CardId = cardId,
            Type = type,
            Amount = amount,
            Category = category,
            Description = description,
            Date = date,
            InstallmentPlanId = installmentPlanId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
