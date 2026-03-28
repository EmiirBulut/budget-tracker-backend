namespace BudgetTracker.Application.Features.Installments.DTOs;

public record InstallmentPaymentDto(
    Guid Id,
    int MonthNumber,
    DateTime DueDate,
    DateTime? PaidDate,
    bool IsPaid);
