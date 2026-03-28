using Microsoft.EntityFrameworkCore;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Reports.DTOs;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _dbContext;

    public ReportRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReportSummaryDto> GetSummaryAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var transactions = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Date >= from && t.Date <= to)
            .ToListAsync(cancellationToken);

        var totalIncome = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpense = transactions
            .Where(t => t.Type == TransactionType.Expense || t.Type == TransactionType.Installment)
            .Sum(t => t.Amount);

        var byCategory = transactions
            .Where(t => t.Type == TransactionType.Expense || t.Type == TransactionType.Installment)
            .GroupBy(t => t.Category)
            .Select(g => new CategoryBreakdownDto(g.Key, g.Sum(t => t.Amount)))
            .OrderByDescending(c => c.Total)
            .ToList();

        var byMonth = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new MonthlyBreakdownDto(
                g.Key.Year,
                g.Key.Month,
                g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                g.Where(t => t.Type == TransactionType.Expense || t.Type == TransactionType.Installment).Sum(t => t.Amount)))
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToList();

        return new ReportSummaryDto(totalIncome, totalExpense, totalIncome - totalExpense, byCategory, byMonth);
    }
}
