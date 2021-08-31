using System;
using System.Threading.Tasks;
using Xunit;

namespace FrankFurter.Tests
{
    /// <summary>
    /// Test the Client class [using a dummy network interface]
    /// </summary>
    public class Client_Tests
    {
        /// <summary>
        /// test client
        /// </summary>
        readonly Client client = new Client(new TestInterface());

        [Fact]
        public async Task GetCurrenciesAsync_Test()
        {
            var currencies = await client.GetCurrenciesAsync();

            Assert.NotNull(currencies);

            Assert.Equal(33, currencies.Count);

            // present
            Assert.True(currencies.ContainsKey("AUD"));
            Assert.True(currencies.ContainsKey("ZAR"));
            // not present
            Assert.False(currencies.ContainsKey("XXX"));
        }

        [Fact]
        public async Task LatestAsync_Test()
        {
            var actual = await client.GetLatestAsync();

            Assert.Equal(1m, actual.Amount);
            Assert.Equal("EUR", actual.Base);

            Assert.Equal(32, actual.Rates.Count);

            // present
            Assert.True(actual.Rates.ContainsKey("AUD"));
            Assert.True(actual.Rates.ContainsKey("ZAR"));
            // not present
            Assert.False(actual.Rates.ContainsKey("XXX"));
        }

        [Fact]
        public async Task HistoricalAsync_Test()
        {
            var dateFrom = new DateTime(1999, 1, 4);
            var actual = await client.GetHistoricalAsync(dateFrom);

            Assert.Equal(1m, actual.Amount);
            Assert.Equal("EUR", actual.Base);
            Assert.Equal(dateFrom, actual.Date);

            Assert.Equal(27, actual.Rates.Count); // there are only 27 rates in 1999 dataset

            // check a rate
            Assert.True(actual.Rates.ContainsKey("AUD"));
            Assert.Equal(1.91m, actual.GetRate("AUD"));
        }

        [Fact]
        public async Task TimeSeriesAsync_TestFromOnly()
        {
            var dateFrom = new DateTime(2021, 8, 2);
            var actual = await client.GetTimeSeriesAsync(dateFrom);

            Assert.Equal(1m, actual.Amount);
            Assert.Equal("EUR", actual.Base);
            Assert.Equal(dateFrom, actual.StartDate);

            // there should be 20 dates
            Assert.Equal(20, actual.Rates.Count);

            // check dates
            Assert.True(actual.Rates.ContainsKey(dateFrom));

            var firstRates = actual.Rates[dateFrom];
            Assert.True(firstRates.ContainsKey("GBP"));
            Assert.Equal(0.85568m, firstRates["GBP"]);
        }


        [Fact]
        public async Task TimeSeriesAsync_TestFromTo()
        {
            var dateFrom = new DateTime(2021, 8, 2);
            var dateTo = new DateTime(2021, 8, 27);
            var actual = await client.GetTimeSeriesAsync(dateFrom, dateTo);


            Assert.Equal(1m, actual.Amount);
            Assert.Equal("EUR", actual.Base);
            Assert.Equal(dateFrom, actual.StartDate);
            Assert.Equal(dateTo, actual.EndDate);

            // there should be 20 dates
            Assert.Equal(20, actual.Rates.Count);

            // check dates
            Assert.True(actual.Rates.ContainsKey(dateFrom));

            var firstRates = actual.Rates[dateFrom];
            Assert.True(firstRates.ContainsKey("GBP"));
            Assert.Equal(0.85568m, firstRates["GBP"]);
        }

    }
}
