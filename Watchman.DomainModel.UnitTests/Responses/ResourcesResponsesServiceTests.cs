﻿using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.DomainModel.Responses.Areas.Administration;

namespace Watchman.DomainModel.UnitTests.Responses
{
    public class ResourcesResponsesServiceTests
    {
        [Test]
        [TestCase("_Template")]
        [TestCase("Administration")]
        [TestCase("AntiSpam")]
        [TestCase("Help")]
        [TestCase("Muting")]
        [TestCase("Responses")]
        [TestCase("UselessFeatures")]
        [TestCase("Users")]
        [TestCase("Warns")]
        public async Task ShouldFindResources(string area)
        {
            //Arrange
            var service = new ResourcesResponsesService();

            //Act
            var result = (await service.GetResponses(area)).ToList();

            //Assert
            result.Should().HaveCountGreaterThan(0);
        }
    }
}
