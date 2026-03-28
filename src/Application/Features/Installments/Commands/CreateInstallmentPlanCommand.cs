using MediatR;
using BudgetTracker.Application.Features.Installments.DTOs;

namespace BudgetTracker.Application.Features.Installments.Commands;

public record CreateInstallmentPlanCommand(
    Guid CardId,
    string Name,
    string Category,
    decimal TotalAmount,
    decimal MonthlyPayment,
    int NumberOfMonths,
    DateTime StartDate) : IRequest<InstallmentPlanDto>;
