namespace BudgetTracker.Application.Features.Reports.DTOs;

public record CategoryBreakdownDto(string Category, decimal Total);
public record MonthlyBreakdownDto(int Year, int Month, decimal Income, decimal Expense);

public record ReportSummaryDto(
    decimal TotalIncome,
    decimal TotalExpense,
    decimal NetBalance,
    List<CategoryBreakdownDto> ByCategory,
    List<MonthlyBreakdownDto> ByMonth);
