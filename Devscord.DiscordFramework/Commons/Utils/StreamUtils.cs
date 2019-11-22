using Devscord.DiscordFramework.Commons.Extensions;
using System.IO;

namespace Devscord.DiscordFramework.Commons.Utils
{
    public static class StreamUtils
    {
        public static MemoryStream ConvertStreamToMemoryStream(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();

            if (stream != null)
            {
                byte[] buffer = stream.ReadFully();

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
