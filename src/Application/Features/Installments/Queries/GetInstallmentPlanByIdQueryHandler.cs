using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Installments.DTOs;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Installments.Queries;

public class GetInstallmentPlanByIdQueryHandler : IRequestHandler<GetInstallmentPlanByIdQuery, InstallmentPlanDto>
{
    private readonly IInstallmentRepository _installmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInstallmentPlanByIdQueryHandler(IInstallmentRepository installmentRepository, ICurrentUserService currentUserService)
    {
        _installmentRepository = installmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<InstallmentPlanDto> Handle(GetInstallmentPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var plan = await _installmentRepository.GetByIdWithPaymentsAsync(request.PlanId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.InstallmentPlan), request.PlanId);

        var payments = plan.Payments?.Select(p => new InstallmentPaymentDto(
            p.Id, p.MonthNumber, p.DueDate, p.PaidDate, p.IsPaid)).ToList();

        return new InstallmentPlanDto(
            plan.Id, plan.CardId, plan.Name, plan.Category,
            plan.TotalAmount, plan.MonthlyPayment, plan.NumberOfMonths,
            plan.StartDate, plan.CreatedAt, payments);
    }
}
