using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Installments.DTOs;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Features.Installments.Commands;

public class CreateInstallmentPlanCommandHandler : IRequestHandler<CreateInstallmentPlanCommand, InstallmentPlanDto>
{
    private readonly IInstallmentRepository _installmentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateInstallmentPlanCommandHandler> _logger;

    public CreateInstallmentPlanCommandHandler(
        IInstallmentRepository installmentRepository,
        ICurrentUserService currentUserService,
        ILogger<CreateInstallmentPlanCommandHandler> logger)
    {
        _installmentRepository = installmentRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<InstallmentPlanDto> Handle(CreateInstallmentPlanCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var plan = InstallmentPlan.Create(
            userId, request.CardId, request.Name, request.Category,
            request.TotalAmount, request.MonthlyPayment,
            request.NumberOfMonths, request.StartDate);

        await _installmentRepository.AddAsync(plan, cancellationToken);
        await _installmentRepository.SaveChangesAsync(cancellationToken);

        // Auto-generate one InstallmentPayment row per month.
        var payments = Enumerable.Range(1, request.NumberOfMonths).Select(month =>
            InstallmentPayment.Create(
                plan.Id,
                month,
                request.StartDate.AddMonths(month - 1))).ToList();

        await _installmentRepository.AddPaymentsAsync(payments, cancellationToken);
        await _installmentRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("InstallmentPlan created: {PlanId} for user: {UserId}", plan.Id, userId);

        return new InstallmentPlanDto(
            plan.Id, plan.CardId, plan.Name, plan.Category,
            plan.TotalAmount, plan.MonthlyPayment, plan.NumberOfMonths,
            plan.StartDate, plan.CreatedAt);
    }
}
