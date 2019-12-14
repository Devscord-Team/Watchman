using Devscord.DiscordFramework.Framework.Commands.Responses;
using NUnit.Framework;
using System.Linq;

namespace Devscord.DiscordFramework.UnitTests.Responses
{
    [TestFixture]
    public class ResponsesServiceTests
    {
        [Test]
        [Ignore("CI have problems with move .json to output directory - ")]
        public void ShouldFoundManyResponses()
        {
            //Arrange
            var responsesService = new ResponsesService();

            //Act
            var responses = responsesService.Responses;
            var result = responses.Count();

            //Assert
            Assert.That(result, Is.GreaterThan(0)); 
        }

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
