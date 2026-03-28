using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Installments.DTOs;

namespace BudgetTracker.Application.Features.Installments.Queries;

public class GetInstallmentPlansQueryHandler : IRequestHandler<GetInstallmentPlansQuery, List<InstallmentPlanDto>>
{
    private readonly IInstallmentRepository _installmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInstallmentPlansQueryHandler(IInstallmentRepository installmentRepository, ICurrentUserService currentUserService)
    {
        _installmentRepository = installmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<InstallmentPlanDto>> Handle(GetInstallmentPlansQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var plans = await _installmentRepository.GetAllByUserIdAsync(userId, cancellationToken);
        return plans.Select(p => new InstallmentPlanDto(
            p.Id, p.CardId, p.Name, p.Category,
            p.TotalAmount, p.MonthlyPayment, p.NumberOfMonths,
            p.StartDate, p.CreatedAt)).ToList();
    }
}
