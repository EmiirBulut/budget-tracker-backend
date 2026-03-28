using MediatR;

namespace BudgetTracker.Application.Features.Installments.Commands;

public record MarkPaymentPaidCommand(Guid PlanId, Guid PaymentId) : IRequest;
