﻿using NUnit.Framework;
using System;
using System.Linq;
using Watchman.Common.Models;
using Watchman.DomainModel.Commons.Calculators.Statistics.Splitters;

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
                .Generate(TimeRange.Create(DateTime.Now.Date.AddHours(12).AddDays(-days), DateTime.Now.Date.AddHours(12)), items);

            //Act
            var splitted = splitter.Split(testCollection).ToList();

            //Assert
            var timeRanges = splitted.Select(x => x.Key);
            var itemsPerDay = splitted.First().Value.Count();
            var daysAreEqual = splitted.All(x => x.Value.Count() == itemsPerDay);

            Assert.That(timeRanges.Count(), Is.EqualTo(days));
            Assert.That(itemsPerDay, Is.EqualTo(shouldItemsPerDay));
            Assert.That(daysAreEqual, Is.True);
        }

        [Ignore("Not yet implemented")]
        [Test]
        [TestCase]
        [TestCase(5, 20, 4)]
        [TestCase(5, 25, 5)]
        [TestCase(2, 10, 5)]
        [TestCase(1, 10, 10)]
        public void MonthSplitterTest(int months, int items, int shouldItemsPerMonth)
        {
            //Arrange
            var splitter = new MonthSplitter();
            var timeRange = TimeRange.Create(DateTime.UtcNow, DateTime.UtcNow.AddMonths(months));
            var testCollection = TestEntityGenerator
                .Generate(timeRange, i =>
                {
                    if (i == 0 || items % i == 0)
                    {
                        return null;
                    }

                    var time = DateTime.UtcNow.AddDays(i);
                    var entity = new TestEntity();
                    entity.SetCreatedAt(time);
                    return entity;
                });

            //Act
            var splitted = splitter.Split(testCollection).ToList();

            //Assert
            var timeRanges = splitted.Select(x => x.Key);
            var itemsPerMonth = splitted.First().Value.Count();
            var daysAreEqual = splitted.All(x => x.Value.Count() == itemsPerMonth);

            Assert.That(timeRanges.Count(), Is.EqualTo(months));
            Assert.That(itemsPerMonth, Is.EqualTo(shouldItemsPerMonth));
            Assert.That(daysAreEqual, Is.True);
        }
    }
}
