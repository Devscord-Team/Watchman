using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Watchman.Integrations.Images
{
    public interface IImagesService
    {
        IEnumerable<Image> GetImagesFromResources(Func<string, bool> selector = null);
    }

    public class ImagesService : IImagesService
    {
        public ImagesService()
        {
        }

        public IEnumerable<Image> GetImagesFromResources(Func<string, bool> selector = null)
        {
            var imagesProperty = typeof(Images).GetProperties().Where(x => x.PropertyType.Name == "Byte[]");
            foreach (var image in imagesProperty)
            {
                if (selector == null || selector.Invoke(image.Name))
                {
                    var imageAsByteArray = (byte[])image.GetValue(image);
                    yield return new Image(image.Name + ".png", new MemoryStream(imageAsByteArray));
                }
            }
        }
    }
}
