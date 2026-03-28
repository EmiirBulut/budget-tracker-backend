using Microsoft.EntityFrameworkCore;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Infrastructure.Persistence.Repositories;

public class InstallmentRepository : IInstallmentRepository
{
    private readonly AppDbContext _dbContext;

    public InstallmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<InstallmentPlan>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InstallmentPlans
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<InstallmentPlan?> GetByIdWithPaymentsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InstallmentPlans
            .Include(p => p.Payments)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId, cancellationToken);
    }

    public async Task<InstallmentPayment?> GetPaymentByIdAsync(Guid planId, Guid paymentId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InstallmentPayments
            .FirstOrDefaultAsync(p =>
                p.Id == paymentId &&
                p.InstallmentPlanId == planId &&
                _dbContext.InstallmentPlans.Any(ip => ip.Id == planId && ip.UserId == userId),
                cancellationToken);
    }

    public async Task AddAsync(InstallmentPlan plan, CancellationToken cancellationToken = default)
    {
        await _dbContext.InstallmentPlans.AddAsync(plan, cancellationToken);
    }

    public async Task AddPaymentsAsync(IEnumerable<InstallmentPayment> payments, CancellationToken cancellationToken = default)
    {
        await _dbContext.InstallmentPayments.AddRangeAsync(payments, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
