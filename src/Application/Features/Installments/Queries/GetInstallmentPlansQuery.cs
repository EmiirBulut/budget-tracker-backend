using MediatR;
using BudgetTracker.Application.Features.Installments.DTOs;

namespace BudgetTracker.Application.Features.Installments.Queries;

public record GetInstallmentPlansQuery : IRequest<List<InstallmentPlanDto>>;
