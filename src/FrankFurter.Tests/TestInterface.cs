using System;
using System.IO;
using System.Threading.Tasks;

namespace FrankFurter.Tests
{
    public class TestInterface : INetworkInterface
    {
        readonly StringComparison it = StringComparison.InvariantCultureIgnoreCase;

        public void Dispose()
        {
            // nothing to do
        }

        public Task<Stream> QueryAsync(string url)
        {
            // currencies
            if (url.StartsWith("/currencies", it)) return TestFiles.GetStreamAsync("Currencies.json");

            // latest
            if (url.StartsWith("/latest", it)) return TestFiles.GetStreamAsync("Latest.json");

            // time series
            if (url.StartsWith("/2021-08-02..")) return TestFiles.GetStreamAsync("TimeSeries.json");

            // single date file
            if (url.StartsWith("/1999", it)) return TestFiles.GetStreamAsync("Historical.json");

            // all other urls will fail
            throw new NotSupportedException($"Unsupported url {url}");
        }
    }
}
