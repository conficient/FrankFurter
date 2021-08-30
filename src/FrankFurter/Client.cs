using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrankFurter
{
    /// <summary>
    /// A Frankfurter.app client for queries
    /// </summary>
    /// <remarks>
    /// See https://www.frankfurter.app/
    /// </remarks>
    public class Client : IDisposable
    {
        /// <summary>
        /// internal API
        /// </summary>
        private readonly INetworkInterface networkAPI;

        /// <summary>
        /// Historical rates only available from this date
        /// </summary>
        readonly DateTime minDate = new DateTime(1999, 1, 4);

        public Client(INetworkInterface networkAPI = null)
        {
            this.networkAPI = networkAPI ?? new NetworkAPI();
        }

        public void Dispose()
        {
            networkAPI?.Dispose();
        }

        #region Quick lookup methods

        /// <summary>
        /// Get the current rate for 
        /// </summary>
        /// <param name="currencyCode">Currency code to convert to, e.g. USD</param>
        /// <param name="baseCurrency">Base currency, default is EUR</param>
        /// <param name="amount">Amount to convert (Default is 1)</param>
        /// <returns>Converted amount if the currency was found, otherwise null</returns>
        public static async Task<decimal?> GetLatestRateAsync(string currencyCode,
                                                              string baseCurrency = EUR,
                                                              decimal amount = 1m)
        {
            using (var client = new Client())
            {
                var latest = await client.GetLatestAsync(baseCurrency, new string[] { currencyCode }, amount);
                return latest.GetRate(currencyCode);
            }
        }

        
        /// <summary>
        /// Get the rate for currency on specific date
        /// </summary>
        /// <param name="date">Date to retrieve</param>
        /// <param name="currencyCode">Currency code to convert to, e.g. USD</param>
        /// <param name="baseCurrency">Base currency, default is EUR</param>
        /// <param name="amount">Amount to convert (Default is 1)</param>
        /// <returns>Converted amount if the currency was found, otherwise null</returns>
        public static async Task<decimal?> GetRateForAsync(DateTime date, 
                                                           string currencyCode,
                                                           string baseCurrency = EUR,
                                                           decimal amount = 1m)
        {
            using (var client = new Client())
            {
                var latest = await client.GetHistoricalAsync(date, baseCurrency, new string[] { currencyCode }, amount);
                return latest.GetRate(currencyCode);
            }
        }


        #endregion


        /// <summary>
        /// Return list of supported currency codes
        /// </summary>
        /// <returns>
        /// Dictionary where the key is ISO 4217 currency code and value is description
        /// </returns>
        /// <remarks>
        /// see https://www.frankfurter.app/docs/#currencies
        /// </remarks>
        public async Task<Dictionary<string, string>> GetCurrenciesAsync()
        {
            const string url = "/currencies";
            var result = await networkAPI.QueryAsync(url);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(result);
        }

        /// <summary>
        /// Euro - the default base currency
        /// </summary>
        public const string EUR = "EUR";

        /// <summary>
        /// Return latest exchange rate for base (From) and targets (To)
        /// </summary>
        /// <param name="baseCurrency">BASE currency (default EUR)</param>
        /// <param name="toCurrencies">TO currencies (blank = all)</param>
        /// <param name="amount">Amount to convert (default = 1)</param>
        /// <returns>single date set of rates</returns>
        /// <remarks>
        /// see https://www.frankfurter.app/docs/#latest
        /// </remarks>
        public async Task<SingleDate> GetLatestAsync(string baseCurrency = EUR,
                                                  IEnumerable<string> toCurrencies = null,
                                                  decimal amount = 1m)
        {
            // create URL for /latest
            var url = new QueryBuilder("/latest")
                .Add(FROM, baseCurrency, EUR)
                .AddRange(TO, toCurrencies)
                .Add(AMT, amount);

            var result = await networkAPI.QueryAsync(url.ToString());
            return await JsonSerializer.DeserializeAsync<SingleDate>(result);
        }


        /// <summary>
        /// Return exchange rate for a single date
        /// </summary>
        /// <param name="date">Date to provide rates for</param>
        /// <param name="baseCurrency">BASE currency (default EUR)</param>
        /// <param name="toCurrencies">TO currencies (blank = all)</param>
        /// <param name="amount">Amount to convert (default = 1)</param>
        /// <returns>single date set of rates</returns>
        /// <remarks>
        /// see https://www.frankfurter.app/docs/#historical
        /// </remarks>
        public async Task<SingleDate> GetHistoricalAsync(DateTime date,
                                                      string baseCurrency = EUR,
                                                      IEnumerable<string> toCurrencies = null,
                                                      decimal amount = 1m)
        {
            if (date < minDate)
                throw new ArgumentOutOfRangeException(nameof(date), $"Minimum date is {minDate}");

            // create URL for historical query:
            var url = new QueryBuilder($"/{DateText(date)}")
                .Add(FROM, baseCurrency, EUR)
                .AddRange(TO, toCurrencies)
                .Add(AMT, amount);

            var result = await networkAPI.QueryAsync(url.ToString());
            return await JsonSerializer.DeserializeAsync<SingleDate>(result);
        }

        /// <summary>
        /// Return set of exchange rates for a date range
        /// </summary>
        /// <param name="dateFrom">Start date for the range</param>
        /// <param name="dateTo">End date for the range: if null, return all dates to latest</param>
        /// <param name="baseCurrency">BASE currency (default EUR)</param>
        /// <param name="toCurrencies">TO currencies (blank = all)</param>
        /// <param name="amount">Amount to convert (default = 1)</param>
        /// <returns>single date set of rates</returns>
        /// <remarks>
        /// see https://www.frankfurter.app/docs/#historical
        /// </remarks>
        public async Task<TimeSeries> GetTimeSeriesAsync(DateTime dateFrom,
                                                      DateTime? dateTo = null,
                                                      string baseCurrency = EUR,
                                                      IEnumerable<string> toCurrencies = null,
                                                      decimal amount = 1m)
        {
            if (dateFrom < minDate)
                throw new ArgumentOutOfRangeException(nameof(dateFrom), $"Minimum date is {minDate:d}");

            string dateToValue = string.Empty;
            if (dateTo.HasValue)
            {
                if (dateTo.Value < minDate)
                    throw new ArgumentOutOfRangeException(nameof(dateTo), $"Minimum date is {minDate:d}");
                if (dateTo.Value < dateFrom)
                    throw new ArgumentOutOfRangeException(nameof(dateTo), $"Date to {dateTo:d} less than from date {dateFrom:d}");
                dateToValue = DateText(dateTo.Value);
            }

            string baseUrl = $"{DateText(dateFrom)}..{dateToValue}";
            // create URL for historical query:
            var url = new QueryBuilder(baseUrl)
                .Add(FROM, baseCurrency, EUR)
                .AddRange(TO, toCurrencies)
                .Add(AMT, amount);

            var result = await networkAPI.QueryAsync(url.ToString());
            return await JsonSerializer.DeserializeAsync<TimeSeries>(result);
        }



        const string FROM = "from";
        const string TO = "to";
        const string AMT = "amount";

        private string DateText(DateTime date) => date.ToString("yyyy-MM-dd");

    }

}