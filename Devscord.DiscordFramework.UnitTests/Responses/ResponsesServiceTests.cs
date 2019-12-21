using Devscord.DiscordFramework.Framework.Commands.Responses;
using NUnit.Framework;
using System.Linq;

namespace Devscord.DiscordFramework.UnitTests.Responses
{
    [TestFixture]
    public class ResponsesServiceTests
    {
        [Test]
        public void ShouldFindManyFieldsInResponse()
        {
            //Arrange
            var response = new Response { Message = "aaa{{first}}bbb{{second}}ccc" };

            //Act
            var fields = response.GetFields();
            var result = fields.Count();

            //Assert
            Assert.That(result, Is.EqualTo(2)); 
        }
    }
}
