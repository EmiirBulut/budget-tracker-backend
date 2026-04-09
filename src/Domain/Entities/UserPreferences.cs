using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public class UserPreferences
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Currency DefaultCurrency { get; private set; }
    public Language Language { get; private set; }
    public bool NotificationsEnabled { get; private set; }

    private UserPreferences() { }

    public static UserPreferences CreateDefault(Guid userId)
    {
        return new UserPreferences
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DefaultCurrency = Currency.USD,
            Language = Language.English,
            NotificationsEnabled = true
        };
    }

    public void Update(Currency defaultCurrency, Language language, bool notificationsEnabled)
    {
        DefaultCurrency = defaultCurrency;
        Language = language;
        NotificationsEnabled = notificationsEnabled;
    }
}
