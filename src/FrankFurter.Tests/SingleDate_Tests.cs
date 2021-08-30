using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FrankFurter.Tests
{
    /// <summary>
    /// Test the SingleDate class
    /// </summary>
    public class SingleDate_Tests
    {
        private readonly ITestOutputHelper output;

        public SingleDate_Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Latest_Test()
        {
            using var stream = await TestFiles.GetStreamAsync("Latest.json");

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var actual = await JsonSerializer.DeserializeAsync<SingleDate>(stream, options);

            Assert.Equal(1m, actual.Amount);
            Assert.Equal("EUR", actual.Base);
            Assert.Equal(new DateTime(2021, 8, 27), actual.Date);
            Assert.NotNull(actual.Rates);

            Assert.Equal(32, actual.Rates.Count); // should be 32 rate entries

            // get a currency
            Assert.Equal(1.622m, actual.Rates["AUD"]);

            Assert.True(actual.Rates.ContainsKey("GBP")); // should exist
            Assert.False(actual.Rates.ContainsKey("gbp")); // case sensitive

        }

        [Fact]
        public void Serialize_Test()
        {
            var tmp = new SingleDate()
            {
                Amount = 1m,
                Base = "EUR",
                Date = new DateTime(2021, 8, 27),
                Rates = new System.Collections.Generic.Dictionary<string, decimal>
                {
                    {  "AUD", 1.622m }
                }
            };

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
            options.Converters.Add(new DateTimeConverter());

            var tms = JsonSerializer.Serialize(tmp, options);

            output.WriteLine(tms);
        }
    }
}
