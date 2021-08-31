using System.IO;
using System.Threading.Tasks;

namespace FrankFurter.Tests
{
    /// <summary>
    /// Helper class for getting test files
    /// </summary>
    internal static class TestFiles
    {

        /// <summary>
        /// Get test file stream
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Task<Stream> GetStreamAsync(string name)
        {
            var file = Path.Combine("./Files", name);
            if (File.Exists(file))
                return Task.FromResult<Stream>(File.OpenRead(file));

            throw new FileNotFoundException(file);
        }

    }
}
