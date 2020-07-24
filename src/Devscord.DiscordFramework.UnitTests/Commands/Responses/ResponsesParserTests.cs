using Devscord.DiscordFramework.Framework.Commands.Responses;
using NUnit.Framework;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.UnitTests.Commands.Responses
{
    [TestFixture]
    public class ResponsesParserTests
    {
        [Test]
        [TestCase("a{{ToReplace}}a", "ToReplace", "a", "aaa", true)]
        [TestCase("a{{ToReplace}}a", "ToReplace", "a", "abc", false)]
        [TestCase("a{{_a_}}a", "_a_", "a", "aaa", true)]
        public void ShouldProcessResponseCorrectly(string input, string paramKey, string paramValue, string expectedResult, bool shouldTrue)
        {
            //Arrange
            var responsesParser = new ResponsesParser();
            var response = new Response { Message = input };
            var param = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(paramKey, paramValue) };

            //Act
            var parsed = responsesParser.Parse(response, param);

            //Assert
            if (shouldTrue)
            {
                Assert.That(parsed, Is.EqualTo(expectedResult));
            }
            else
            {
                Assert.That(parsed, Is.Not.EqualTo(expectedResult));
            }
        }
    }
}
