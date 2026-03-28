using BudgetTracker.Application.Features.Reports.DTOs;

namespace BudgetTracker.Application.Common.Interfaces;

public interface IReportRepository
{
    Task<ReportSummaryDto> GetSummaryAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}
