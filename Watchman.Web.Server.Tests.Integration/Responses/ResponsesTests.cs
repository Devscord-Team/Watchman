using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Web.Server.Areas.Responses.Models.Dtos;

namespace Watchman.Web.Server.Tests.Integration.Responses
{
    [TestFixture]
    public class ResponsesTests
    {
        [Test]
        public void ShouldReturnAnyResponses()
        {
            //Arrange
            var client = Configurator.GetClient();
            var request = new RestRequest("/Responses/GetResponses", DataFormat.Json);

            //Act
            var response = client.Execute(request);
            var responses = JsonConvert.DeserializeObject<IEnumerable<ResponseDto>>(response.Content);

            //Assert
            Assert.That(response.IsSuccessful, Is.True);
            Assert.That(responses, Is.AtLeast(1));
        }
    }
}
