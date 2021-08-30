using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FrankFurter.Tests
{
    public class TimeSeries_Tests
    {
        [Fact]
        public async Task Deserialize_TestAsync()
        {
            // get the stream
            using var stream = await TestFiles.GetStreamAsync("TimeSeries.json");

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var actual = await JsonSerializer.DeserializeAsync<TimeSeries>(stream, options);

            Assert.Equal(1m, actual.Amount);
            Assert.Equal("EUR", actual.Base);
            Assert.Equal(new DateTime(2021, 8, 2), actual.StartDate);
            Assert.Equal(new DateTime(2021, 8, 27), actual.EndDate);

            Assert.NotNull(actual.Rates);

            Assert.Equal(20, actual.Rates.Count); // should be 20 date entries

            var date1 = new DateTime(2021, 08, 02);
            Assert.True(actual.Rates.ContainsKey(date1), $"Didn't find {date1:yyyy-MM-dd}");

        }
    }
}
