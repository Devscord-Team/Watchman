using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Watchman.Integrations.Images
{
    public class ImagesService
    {
        public IEnumerable<KeyValuePair<string,Stream>> GetImagesFromResources(Func<string, bool> selector = null)
        {
            var imagesProperty = typeof(Images).GetProperties().Where(x => x.PropertyType.Name == "Byte[]");
            foreach (var image in imagesProperty)
            {
                if (selector == null || selector.Invoke(image.Name))
                {
                    var imageAsByteArray = (byte[])image.GetValue(image);
                    yield return new KeyValuePair<string, Stream>(image.Name + ".png", new MemoryStream(imageAsByteArray));
                }
            }
        }
    }
}
