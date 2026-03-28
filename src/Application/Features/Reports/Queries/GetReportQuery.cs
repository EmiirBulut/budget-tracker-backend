using MediatR;
using BudgetTracker.Application.Features.Reports.DTOs;

namespace BudgetTracker.Application.Features.Reports.Queries;

public record GetReportQuery(DateTime From, DateTime To) : IRequest<ReportSummaryDto>;
