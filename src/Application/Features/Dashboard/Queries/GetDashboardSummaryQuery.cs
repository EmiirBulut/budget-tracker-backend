using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Dashboard.DTOs;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Features.Dashboard.Queries;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAccountRepository _accountRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IFxRateService _fxRateService;
    private readonly ILogger<GetDashboardSummaryQueryHandler> _logger;

    public GetDashboardSummaryQueryHandler(
        ICurrentUserService currentUserService,
        IAccountRepository accountRepository,
        IUserPreferencesRepository preferencesRepository,
        IFxRateService fxRateService,
        ILogger<GetDashboardSummaryQueryHandler> logger)
    {
        _currentUserService = currentUserService;
        _accountRepository = accountRepository;
        _preferencesRepository = preferencesRepository;
        _fxRateService = fxRateService;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        // Sequential queries — both repositories share the same scoped DbContext instance,
        // which does not support concurrent operations.
        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? UserPreferences.CreateDefault(userId);

        var activeAccounts = (await _accountRepository.GetAllByUserIdAsync(userId, cancellationToken))
            .Where(a => !a.IsArchived)
            .ToList();

        var targetCurrency = preferences.DefaultCurrency;

        var accountBalances = new List<AccountBalanceDto>(activeAccounts.Count);
        decimal total = 0m;

        foreach (var account in activeAccounts)
        {
            var rate = await _fxRateService.GetRateAsync(
                account.Currency, targetCurrency, cancellationToken);

            var converted = Math.Round(account.Balance * rate, 2);
            total += converted;

            accountBalances.Add(new AccountBalanceDto(
                account.Id,
                account.Name,
                account.Currency,
                account.Balance,
                converted));
        }

        var ratesAsOf = await _fxRateService.GetRatesTimestampAsync(cancellationToken);

        _logger.LogInformation(
            "Dashboard summary for user {UserId}: {Count} accounts, total {Total} {Currency}",
            userId, activeAccounts.Count, Math.Round(total, 2), targetCurrency);

        return new DashboardSummaryDto(
            Math.Round(total, 2),
            targetCurrency,
            accountBalances,
            ratesAsOf);
    }
}
