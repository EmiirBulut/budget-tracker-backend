namespace BudgetTracker.Application.Features.Installments.DTOs;

public record CreateInstallmentPlanRequestDto(
    Guid CardId,
    string Name,
    string Category,
    decimal TotalAmount,
    decimal MonthlyPayment,
    int NumberOfMonths,
    DateTime StartDate);
