using Devscord.DiscordFramework.Commons.Extensions;
using System.IO;

namespace Devscord.DiscordFramework.Commons.Utils
{
    public static class StreamUtils
    {
        public static MemoryStream ConvertStreamToMemoryStream(Stream stream)
        {
            var memoryStream = new MemoryStream();

            if (stream != null)
            {
                var buffer = stream.ReadFully();

                if (buffer != null)
                {
                    var binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write(buffer);
                }
            }
            return memoryStream;
        }
    }
}
