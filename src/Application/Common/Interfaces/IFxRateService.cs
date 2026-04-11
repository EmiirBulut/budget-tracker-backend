using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Common.Interfaces;

public interface IFxRateService
{
    /// <summary>
    /// Returns the multiplier such that: amount_in_from * rate = amount_in_to.
    /// Returns 1 when from == to.
    /// </summary>
    Task<decimal> GetRateAsync(Currency from, Currency to, CancellationToken ct = default);

    /// <summary>
    /// Returns the UTC timestamp of when the current cached rates were fetched.
    /// </summary>
    Task<DateTime> GetRatesTimestampAsync(CancellationToken ct = default);
}
