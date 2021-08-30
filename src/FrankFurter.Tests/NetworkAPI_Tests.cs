//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Xunit;

//namespace FrankFurter.Tests
//{
//     //this class is disabled so we do not repeatedly call the
//     //live API, but can be enabled for one-off testing as required

//    public class NetworkAPI_Tests : IDisposable
//    {

//        readonly NetworkAPI client = new NetworkAPI();

//        public void Dispose() => client?.Dispose();

//        [Fact]
//        public async Task QueryAsync_Works()
//        {
//            const string url = "/currencies";

//            using var stream = await client.QueryAsync(url);

//            var json = await new StreamReader(stream).ReadToEndAsync();

//            Assert.NotNull(json);
//        }

//        [Fact]
//        public async Task QueryAsync_FailsForBadUrl()
//        {
//            // This url should return 404
//            const string url = "/badurl";

//            await Assert.ThrowsAsync<System.Net.Http.HttpRequestException>(async () =>
//            {
//                using var stream = await client.QueryAsync(url);
//            });
//        }

//    }
//}
