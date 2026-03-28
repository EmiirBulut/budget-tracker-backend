using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Common.Interfaces;

public interface IInstallmentRepository
{
    Task<List<InstallmentPlan>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<InstallmentPlan?> GetByIdWithPaymentsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<InstallmentPayment?> GetPaymentByIdAsync(Guid planId, Guid paymentId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(InstallmentPlan plan, CancellationToken cancellationToken = default);
    Task AddPaymentsAsync(IEnumerable<InstallmentPayment> payments, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
