using Microsoft.EntityFrameworkCore;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _dbContext;

    public CardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Card>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cards
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Card?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Card card, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cards.AddAsync(card, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
