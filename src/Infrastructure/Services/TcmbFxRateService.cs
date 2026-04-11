using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Infrastructure.Services;

internal sealed record FxRateCache(
    IReadOnlyDictionary<string, decimal> Rates,
    DateTime FetchedAt);

public class TcmbFxRateService : IFxRateService
{
    private const string CacheKey = "tcmb_fx_rates";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(4);
    private const string TcmbUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TcmbFxRateService> _logger;

    public TcmbFxRateService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<TcmbFxRateService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<decimal> GetRateAsync(
        Currency from, Currency to, CancellationToken ct = default)
    {
        if (from == to) return 1m;

        var rateCache = await GetOrLoadRatesAsync(ct);

        // All TCMB rates are "TRY per 1 unit of foreign currency"
        // To convert from → to:  amount * (fromTry / toTry)
        var fromTry = GetTryRate(rateCache.Rates, from);
        var toTry = GetTryRate(rateCache.Rates, to);

        return fromTry / toTry;
    }

    public async Task<DateTime> GetRatesTimestampAsync(CancellationToken ct = default)
    {
        var rateCache = await GetOrLoadRatesAsync(ct);
        return rateCache.FetchedAt;
    }

    private async Task<FxRateCache> GetOrLoadRatesAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue<FxRateCache>(CacheKey, out var cached) && cached is not null)
            return cached;

        _logger.LogInformation("TCMB FX rates cache miss — fetching from {Url}", TcmbUrl);

        var client = _httpClientFactory.CreateClient("tcmb");
        var xml = await client.GetStringAsync(TcmbUrl, ct);

        var rates = ParseRates(xml);
        var entry = new FxRateCache(rates, DateTime.UtcNow);

        _cache.Set(CacheKey, entry, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheTtl
        });

        _logger.LogInformation(
            "TCMB FX rates cached — {Count} currencies, valid for {Hours}h",
            rates.Count, CacheTtl.TotalHours);

        return entry;
    }

    private static IReadOnlyDictionary<string, decimal> ParseRates(string xml)
    {
        // TCMB XML structure:
        // <Tarih_Date ...>
        //   <Currency Kod="USD">
        //     <ForexBuying>38.1234</ForexBuying>
        //     <ForexSelling>38.5678</ForexSelling>
        //   </Currency>
        //   ...
        // </Tarih_Date>

        var doc = XDocument.Parse(xml);
        var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var el in doc.Root!.Elements("Currency"))
        {
            var code = (string?)el.Attribute("Kod");
            if (string.IsNullOrWhiteSpace(code)) continue;

            // Prefer ForexSelling; fall back to ForexBuying for cross-rates
            // that only populate one of the two fields
            var rateStr = GetElementValue(el, "ForexSelling")
                       ?? GetElementValue(el, "ForexBuying");

            if (rateStr is not null
                && decimal.TryParse(rateStr, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out var rate)
                && rate > 0)
            {
                rates[code] = rate;
            }
        }

        return rates;
    }

    private static string? GetElementValue(XElement parent, string name)
    {
        var val = parent.Element(name)?.Value?.Trim();
        return string.IsNullOrEmpty(val) ? null : val;
    }

    private static decimal GetTryRate(
        IReadOnlyDictionary<string, decimal> rates, Currency currency)
    {
        if (currency == Currency.TRY) return 1m;

        var code = currency.ToString().ToUpperInvariant();
        if (rates.TryGetValue(code, out var rate))
            return rate;

        throw new DomainException(
            $"Exchange rate for {code} is not available in TCMB data.");
    }
}
