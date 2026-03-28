namespace BudgetTracker.Application.Features.Installments.DTOs;

public record InstallmentPlanDto(
    Guid Id,
    Guid CardId,
    string Name,
    string Category,
    decimal TotalAmount,
    decimal MonthlyPayment,
    int NumberOfMonths,
    DateTime StartDate,
    DateTime CreatedAt,
    List<InstallmentPaymentDto>? Payments = null);
