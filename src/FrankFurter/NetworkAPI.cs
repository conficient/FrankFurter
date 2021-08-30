using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrankFurter
{
    /// <summary>
    /// Network interface implementation that uses the Frankfurter API
    /// </summary>
    public class NetworkAPI : INetworkInterface
    {
        readonly HttpClient client = new HttpClient();

        private readonly string baseUrl;

        /// <summary>
        /// The default Frankfurter API base URL
        /// </summary>
        public const string defaultUrl = "https://api.frankfurter.app";

        /// <summary>
        /// Get query result
        /// </summary>
        /// <param name="baseUrl"></param>
        public NetworkAPI(string baseUrl = defaultUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
        }

        public void Dispose()
        {
            client?.Dispose();
        }

        /// <summary>
        /// Return content stream for relative url, e.g. /currencies
        /// </summary>
        /// <param name="url">URL relative to base, with / prefix</param>
        /// <returns></returns>
        public Task<Stream> QueryAsync(string url)
        {
            return client.GetStreamAsync(GetFullUrl(url));
        }

        internal string GetFullUrl(string url) => $"{baseUrl}{url}";
    }
}
