using System;
using System.IO;
using System.Threading.Tasks;

namespace FrankFurter
{
    /// <summary>
    /// Abstraction of the JSON network interface
    /// </summary>
    public interface INetworkInterface : IDisposable
    {
        Task<Stream> QueryAsync(string url);
    }
}
