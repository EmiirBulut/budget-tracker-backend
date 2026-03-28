using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public Currency Currency { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    public static Account Create(
        Guid userId,
        string name,
        AccountType type,
        Currency currency,
        decimal initialBalance)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Type = type,
            Currency = currency,
            Balance = initialBalance,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, AccountType type, Currency currency)
    {
        Name = name;
        Type = type;
        Currency = currency;
    }

    public void Archive()
    {
        IsArchived = true;

    }

    public void UpdateBalance(decimal delta)
    {
        Balance += delta;
    }
}
