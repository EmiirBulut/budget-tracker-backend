using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Common.Interfaces;

public interface IAccountRepository
{
    Task<List<Account>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
