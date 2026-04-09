using Microsoft.EntityFrameworkCore;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Infrastructure.Persistence.Repositories;

public class UserPreferencesRepository : IUserPreferencesRepository
{
    private readonly AppDbContext _dbContext;

    public UserPreferencesRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(UserPreferences preferences, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserPreferences.AddAsync(preferences, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
