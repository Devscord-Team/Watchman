using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Integrations.Images;

namespace Watchman.Integration.Tests.Images
{
    [TestFixture]
    public class ImagesServiceTests
    {
        [Test]
        public void ShouldReturnAnyImage()
        {
            //Arrange
            var imagesService = new ImagesService();

            //Act
            var images = imagesService.GetImagesFromResources();

            //Assert
            var firstImage = images.FirstOrDefault();
            Assert.That(firstImage, Is.Not.Null);
            Assert.That(firstImage.Stream.Length, Is.AtLeast(2137));
        }
    }
}
