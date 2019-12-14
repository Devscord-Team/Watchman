using Devscord.DiscordFramework.Framework.Commands.Responses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.UnitTests.Responses
{
    [TestFixture]
    public class ResponsesParserTests
    {
        [Test]
        public void ShouldProcessResponseCorrectly()
        {
            //Arrange
            var responsesParser = new ResponsesParser();
            var response = new Response { Message = "a{{ToReplace}}a" };
            var param = new KeyValuePair<string, string>("ToReplace", "a");

            //Act
            var parsed = responsesParser.Parse(response, param);

            //Assert
            Assert.That(parsed, Is.EqualTo("aaa"));
        }
    }
}
