namespace BudgetTracker.Domain.Entities;

public class InstallmentPlan
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CardId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public decimal MonthlyPayment { get; private set; }
    public int NumberOfMonths { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation — populated by EF Core Include()
    public IReadOnlyCollection<InstallmentPayment> Payments { get; private set; } = new List<InstallmentPayment>();

    private InstallmentPlan() { }

    public static InstallmentPlan Create(
        Guid userId,
        Guid cardId,
        string name,
        string category,
        decimal totalAmount,
        decimal monthlyPayment,
        int numberOfMonths,
        DateTime startDate)
    {
        return new InstallmentPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CardId = cardId,
            Name = name,
            Category = category,
            TotalAmount = totalAmount,
            MonthlyPayment = monthlyPayment,
            NumberOfMonths = numberOfMonths,
            StartDate = startDate,
            CreatedAt = DateTime.UtcNow
        };
    }
}
