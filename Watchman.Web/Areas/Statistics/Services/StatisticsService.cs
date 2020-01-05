using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.Web.Areas.Statistics.Models;

namespace Watchman.Web.Areas.Statistics.Services
{
    public class StatisticsService
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;

        public StatisticsService(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
        }

        public IEnumerable<StatisticsPerChannelDto> GetStatisticsPerChannel()
        {
            return default;
        }

        public IEnumerable<StatisticsPerPeriodAndChannelDto> GetStatisticsPerPeriodAndChannel()
        {
            return default;
        }

        public IEnumerable<StatisticsPerPeriodDto> GetStatisticsPerPeriod()
        {
            return default;
        }

        public IEnumerable<StatisticsPerUserDto> GetStatisticsPerUser()
        {
            return TestDataStatisticsPerUser();
        }

        //todo delete
        public IEnumerable<StatisticsPerUserDto> TestDataStatisticsPerUser()
        {
            var result = new List<StatisticsPerUserDto>();
            for (var i = 0; i < 3; i++)
            {
                var statsPerUser = new StatisticsPerUserDto
                {
                    User = "TestUser" + i,
                    TotalMessages = new Random().Next(200, 100000),
                    StatisticsPerChannel = TestDataStatisticsPerChannels(),
                    StatisticsPerPeriod = TestDataStatisticsPerPeriod(),
                    StatisticsPerPeriodAndChannel = TestDataStatisticsPerPeriodAndChannel()

                };
                result.Add(statsPerUser);
            }
            return result;
        }

        public IEnumerable<StatisticsPerChannelDto> TestDataStatisticsPerChannels()
        {
            var result = new List<StatisticsPerChannelDto>();
            for (var i = 0; i < 5; i++)
            {
                var statsPerChannel = new StatisticsPerChannelDto
                {
                    Channel = "TestChannel" + i,
                    TotalMessages = new Random().Next(0, 200)
                };
                result.Add(statsPerChannel);
            }
            return result;
        }

        public IEnumerable<StatisticsPerPeriodDto> TestDataStatisticsPerPeriod()
        {
            var result = new List<StatisticsPerPeriodDto>();
            for (var i = 0; i < 5; i++)
            {
                var statsPerPeriod = new StatisticsPerPeriodDto
                {
                    Period = Period.Day,
                    TotalMessages = new Random().Next(0, 200),
                    TimeRange = new TimeRange 
                    { 
                        Start = DateTime.Now.Date.AddDays(-i),
                        End = DateTime.Now.Date.AddDays(-i - 1)
                    }
                };
                result.Add(statsPerPeriod);
            }
            return result;
        }

        public IEnumerable<StatisticsPerPeriodAndChannelDto> TestDataStatisticsPerPeriodAndChannel()
        {
            var result = new List<StatisticsPerPeriodAndChannelDto>();
            for (var i = 0; i < 5; i++)
            {
                var statsPerPeriodAndChannel = new StatisticsPerPeriodAndChannelDto
                {
                    Channel = "TestPeriodChannel" + i,
                    Period = Period.Day,
                    TimeRange = new TimeRange
                    {
                        Start = DateTime.Now.Date.AddDays(-i),
                        End = DateTime.Now.Date.AddDays(-i - 1)
                    },
                    TotalMessages = new Random().Next(0, 200)
                };
                result.Add(statsPerPeriodAndChannel);
            }
            return result;
        }
    }
}
