using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.ResponsesManagers;
using Watchman.DomainModel.Responses.Areas.Administration;

namespace Watchman.Discord.IntegrationTests.Responses
{
    public class ResponsesManagersTests
    {
        [Test]
        [TestCase("FrameworkExceptions", typeof(FrameworkExceptionsResponsesManager))]
        [TestCase("Administration", typeof(AdministrationResponsesManager))]
        [TestCase("AntiSpam", typeof(AntiSpamResponsesManager))]
        [TestCase("Help", typeof(HelpResponsesManager))]
        [TestCase("Muting", typeof(MutingResponsesManager))]
        [TestCase("Responses", typeof(ResponsesResponsesManager))]
        [TestCase("UselessFeatures", typeof(UselessFeaturesResponsesManager))]
        [TestCase("Users", typeof(UsersResponsesManager))]
        [TestCase("Warns", typeof(WarnsResponsesManager))]
        public void AllMethodsShouldBeEquivalentToDataInJsons(string area, Type responsesManager)
        {
            //Arrange
            var resourcesResponsesService = new ResourcesResponsesService();
            var methods = responsesManager.GetMethods(BindingFlags.Static | BindingFlags.Public);

            //Act
            var responsesInJsons = resourcesResponsesService.GetResponses(area).ToList();
            responsesInJsons.Should().HaveSameCount(methods);

            var pairs = methods.Join(responsesInJsons, x => x.Name, x => x.OnEvent, (method, response) => (method, response));
            foreach ((var method, var response) in pairs)
            {
                var parameterNames = this.GetMethodParameterNames(method).ToList();
                response.AvailableVariables.Should().BeEquivalentTo(parameterNames);
            }
        }

        private IEnumerable<string> GetMethodParameterNames(MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                if(parameter.ParameterType == typeof(IResponsesService))
                {
                    continue;
                }
                if(parameter.ParameterType != typeof(Contexts))
                {
                    yield return parameter.Name;
                    continue;
                }
                foreach (var name in new[] { "context_user", "context_channel", "context_server" })
                {
                    yield return name;
                }
            }
        }
    }
}
