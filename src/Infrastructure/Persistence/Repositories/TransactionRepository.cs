using Microsoft.EntityFrameworkCore;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _dbContext;

    public TransactionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Transaction>> GetByUserIdAsync(
        Guid userId,
        Guid? accountId,
        Guid? cardId,
        string? type,
        DateTime? startDate,
        DateTime? endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(userId, accountId, cardId, type, startDate, endDate);

        return await query
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(
        Guid userId,
        Guid? accountId,
        Guid? cardId,
        string? type,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default)
    {
        return await BuildQuery(userId, accountId, cardId, type, startDate, endDate)
            .CountAsync(cancellationToken);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Transaction> BuildQuery(
        Guid userId,
        Guid? accountId,
        Guid? cardId,
        string? type,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);

        if (cardId.HasValue)
            query = query.Where(t => t.CardId == cardId.Value);

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<TransactionType>(type, true, out var parsedType))
            query = query.Where(t => t.Type == parsedType);

        if (startDate.HasValue)
        {
            var start = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
            query = query.Where(t => t.Date >= start);
        }

        if (endDate.HasValue)
        {
            var end = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
            query = query.Where(t => t.Date <= end);
        }

        return query;
    }
}
