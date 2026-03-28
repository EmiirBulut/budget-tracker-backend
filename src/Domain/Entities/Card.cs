using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public class Card
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public CardCategory CardCategory { get; private set; }
    public CardType CardType { get; private set; }
    public string Last4Digits { get; private set; } = string.Empty;
    public string ExpiryDate { get; private set; } = string.Empty;
    public Currency Currency { get; private set; }
    public string Color { get; private set; } = string.Empty;
    public decimal? CreditLimit { get; private set; }
    public Guid? LinkedAccountId { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Card() { }

    public static Card Create(
        Guid userId,
        string name,
        CardCategory cardCategory,
        CardType cardType,
        string last4Digits,
        string expiryDate,
        Currency currency,
        string color,
        decimal? creditLimit,
        Guid? linkedAccountId)
    {
        return new Card
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            CardCategory = cardCategory,
            CardType = cardType,
            Last4Digits = last4Digits,
            ExpiryDate = expiryDate,
            Currency = currency,
            Color = color,
            CreditLimit = creditLimit,
            LinkedAccountId = linkedAccountId,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        CardType cardType,
        string last4Digits,
        string expiryDate,
        Currency currency,
        string color,
        decimal? creditLimit,
        Guid? linkedAccountId)
    {
        Name = name;
        CardType = cardType;
        Last4Digits = last4Digits;
        ExpiryDate = expiryDate;
        Currency = currency;
        Color = color;
        CreditLimit = creditLimit;
        LinkedAccountId = linkedAccountId;
    }

    public void Archive() => IsArchived = true;
}
