using Devscord.DiscordFramework.Framework.Commands.Builders;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using Devscord.DiscordFramework.Framework.Commands.Services;
using NUnit.Framework;
using System.Linq;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.UnitTests.Commands
{
    [TestFixture]
    public class BotCommandsServiceTests
    {
        [Test]
        public void ShouldGenerateDefaultCommandTemplateBasedOnModel()
        {
            //Arrange
            var service = new BotCommandsTemplateBuilder();

            //Act
            var template = service.GetCommandTemplate(typeof(TestCommand));

            //Assert
            Assert.That(template.CommandName, Is.EqualTo("TestCommand"));
            Assert.That(template.Properties.Count(), Is.EqualTo(5));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.Text), Is.EqualTo(1));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.SingleWord), Is.EqualTo(2));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.UserMention), Is.EqualTo(1));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.Time), Is.EqualTo(0));
        }

        [Test]
        public void ShouldRenderTemplateCorrect()
        {
            //Arrange
            var templateBuilder = new BotCommandsTemplateBuilder();
            var renderingService = new BotCommandsTemplateRenderingService();

            //Act
            var template = templateBuilder.GetCommandTemplate(typeof(SmallTestCommand)); //tested in ShouldGenerateDefaultCommandTemplateBasedOnModel()
            var rendered = renderingService.RenderTextTemplate(template);

            //Assert
            Assert.That(rendered, Is.EqualTo("{{prefix}}[[SmallTestCommand]] {{prefix}}[[TestNumber]] ((Number)) {{prefix}}[[TestUser]] ((UserMention))<<optional>>"));
        }

        [Test]
        public void ShouldMapToTemplate()
        {
            //Arrange
            var template = new BotCommandsTemplateBuilder().GetCommandTemplate(typeof(SmallTestCommand));
            var customTemplate = new Regex(@"run\s*(?<TestUser>\<\@\S+\>)?\s*(?<TestNumber>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var input = "-run <@1234567890> 12";
            var parsingService = new BotCommandsParsingService(new BotCommandsPropertyConversionService(), new BotCommandsRequestValueGetterService()); //todo mock and test

            //Act
            var result = (SmallTestCommand) parsingService.ParseCustomTemplate(typeof(SmallTestCommand), template, customTemplate, input);

            //Assert
            Assert.That(result.TestNumber, Is.EqualTo(12));
            Assert.That(result.TestUser, Is.EqualTo("<@1234567890>"));
        }
    }
}
