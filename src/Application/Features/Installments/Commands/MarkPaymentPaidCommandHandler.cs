using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Installments.Commands;

public class MarkPaymentPaidCommandHandler : IRequestHandler<MarkPaymentPaidCommand>
{
    private readonly IInstallmentRepository _installmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkPaymentPaidCommandHandler(IInstallmentRepository installmentRepository, ICurrentUserService currentUserService)
    {
        _installmentRepository = installmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(MarkPaymentPaidCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var payment = await _installmentRepository.GetPaymentByIdAsync(request.PlanId, request.PaymentId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.InstallmentPayment), request.PaymentId);

        if (payment.IsPaid)
            throw new DomainException("This payment is already marked as paid.");

        payment.MarkPaid(DateTime.UtcNow);
        await _installmentRepository.SaveChangesAsync(cancellationToken);
    }
}
