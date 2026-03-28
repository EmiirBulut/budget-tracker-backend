using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetByUserIdAsync(
        Guid userId,
        Guid? accountId,
        Guid? cardId,
        string? type,
        DateTime? startDate,
        DateTime? endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountByUserIdAsync(
        Guid userId,
        Guid? accountId,
        Guid? cardId,
        string? type,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
