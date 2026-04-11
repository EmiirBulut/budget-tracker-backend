using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Dashboard.DTOs;

public record AccountBalanceDto(
    Guid AccountId,
    string AccountName,
    Currency NativeCurrency,
    decimal NativeBalance,
    decimal ConvertedBalance);

public record DashboardSummaryDto(
    decimal TotalBalance,
    Currency DefaultCurrency,
    List<AccountBalanceDto> Accounts,
    DateTime RatesAsOf);
