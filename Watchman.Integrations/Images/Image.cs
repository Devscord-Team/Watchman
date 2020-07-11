using System.IO;

namespace Watchman.Integrations.Images
{
    public class Image
    {
        public string Name { get; private set; }
        public Stream Stream { get; private set; }

        public Image(string name, Stream stream)
        {
            this.Name = name;
            this.Stream = stream;
        }
    }
}
