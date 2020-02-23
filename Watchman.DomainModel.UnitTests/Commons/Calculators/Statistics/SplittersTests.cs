using AutoFixture;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;
using Watchman.DomainModel.Commons.Calculators.Statistics.Splitters;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.UnitTests.Commons.Calculators.Statistics
{
    [TestFixture]
    public class SplittersTests
    {
        [Test]
        [TestCase(5, 20, 4)]
        [TestCase(5, 25, 5)]
        [TestCase(2, 10, 5)]
        [TestCase(1, 10, 10)]
        public void DaySplitterTest(int days, int items, int shouldItemsPerDay)
        {
            //Arrange
            var splitter = new DaySplitter();
            var testCollection = TestEntityGenerator
                .Generate(TimeRange.Create(DateTime.Now.Date.AddHours(12).AddDays(-items), DateTime.Now.Date.AddHours(12)), days);

            //Act
            var splitted = splitter.Split(testCollection);

            //Assert
            var timeRanges = splitted.Select(x => x.Key);
            var itemsPerDay = splitted.First().Value.Count();
            var daysAreEqual = splitted.All(x => x.Value.Count() == itemsPerDay);

            Assert.That(timeRanges.Count(), Is.EqualTo(days));
            Assert.That(itemsPerDay, Is.EqualTo(shouldItemsPerDay));
            Assert.That(daysAreEqual, Is.True);
        }
    }
}
