using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Reports.DTOs;

namespace BudgetTracker.Application.Features.Reports.Queries;

public class GetReportQueryHandler : IRequestHandler<GetReportQuery, ReportSummaryDto>
{
    private readonly IReportRepository _reportRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetReportQueryHandler(IReportRepository reportRepository, ICurrentUserService currentUserService)
    {
        _reportRepository = reportRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ReportSummaryDto> Handle(GetReportQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        return await _reportRepository.GetSummaryAsync(userId, request.From, request.To, cancellationToken);
    }
}
