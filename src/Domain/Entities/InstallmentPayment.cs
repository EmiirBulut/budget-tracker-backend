namespace BudgetTracker.Domain.Entities;

public class InstallmentPayment
{
    public Guid Id { get; private set; }
    public Guid InstallmentPlanId { get; private set; }
    public int MonthNumber { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private InstallmentPayment() { }

    public static InstallmentPayment Create(Guid installmentPlanId, int monthNumber, DateTime dueDate)
    {
        return new InstallmentPayment
        {
            Id = Guid.NewGuid(),
            InstallmentPlanId = installmentPlanId,
            MonthNumber = monthNumber,
            DueDate = dueDate,
            IsPaid = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkPaid(DateTime paidDate)
    {
        IsPaid = true;
        PaidDate = paidDate;
    }
}
