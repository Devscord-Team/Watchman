using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages
{
    public class PreGeneratedDayStatistic : Entity, IAggregateRoot
    {
        public ulong ServerId { get; private set; }
        public IEnumerable<UserDayStatistic> UserDayStatistics { get; private set; }
        public IEnumerable<ChannelDayStatistic> ChannelDayStatistics { get; private set; }
        public IEnumerable<ChannelUserDayStatistic> ChannelUserDayStatistics { get; private set; }
        public int Count { get; private set; }
        public DateTime Date { get; private set; }

        public PreGeneratedDayStatistic(ulong serverId, int count, DateTime date)
        {
            this.ServerId = serverId;
            this.Count = count;
            this.Date = date;
        }

        public int GetUserStatisticsSum(ulong userId)
        {
            if (this.UserDayStatistics == null || !this.UserDayStatistics.Any(x => x.UserId == userId))
            {
                return 0;
            }
            return this.UserDayStatistics
                .Where(x => x.UserId == userId)
                .Sum(x => x.Count);
        }

        public int GetChannelStatisticsSum(ulong channelId)
        {
            if(this.ChannelDayStatistics == null || !this.ChannelDayStatistics.Any(x => x.ChannelId == channelId))
            {
                return 0;
            }
            return this.ChannelDayStatistics
                .Where(x => x.ChannelId == channelId)
                .Sum(x => x.Count);
        }

        public int GetChannelUserStatisticsSum(ulong channelId, ulong userId)
        {
            if (this.ChannelUserDayStatistics == null || !this.ChannelUserDayStatistics.Any(x => x.ChannelId == channelId || x.UserId == userId))
            {
                return 0;
            }
            return this.ChannelUserDayStatistics
                .Where(x => x.ChannelId == channelId && x.UserId == userId)
                .Sum(x => x.Count);
        }

        public void SetUserDayStatistics(IEnumerable<UserDayStatistic> userDayStatistics)
        {
            if(this.UserDayStatistics != null && !this.UserDayStatistics.SequenceEqual(userDayStatistics))
            {
                return;
            }
            if(userDayStatistics.Sum(x => x.Count) != this.Count)
            {
                throw new ArgumentException("Incorrect messages sum");
            }
            if (userDayStatistics.Select(x => x.UserId).Where(x => x > 0).Distinct().Count() != userDayStatistics.Count())
            {
                throw new ArgumentException("Incorrect users in userDayStatistics");
            }
            this.UserDayStatistics = userDayStatistics;
            this.Update();
        }

        public void SetChannelDayStatistics(IEnumerable<ChannelDayStatistic> channelDayStatistics)
        {
            if (this.ChannelDayStatistics != null && !this.ChannelDayStatistics.SequenceEqual(channelDayStatistics))
            {
                return;
            }
            if (channelDayStatistics.Sum(x => x.Count) != this.Count)
            {
                throw new ArgumentException("Incorrect messages sum");
            }
            if (channelDayStatistics.Select(x => x.ChannelId).Where(x => x > 0).Distinct().Count() != channelDayStatistics.Count())
            {
                throw new ArgumentException("Incorrect channels in channelDayStatistics");
            }
            this.ChannelDayStatistics = channelDayStatistics;
            this.Update();
        }

        public void SetChannelUserDayStatistics(IEnumerable<ChannelUserDayStatistic> channelUserDayStatistics)
        {
            if (this.ChannelUserDayStatistics != null && !this.ChannelUserDayStatistics.SequenceEqual(channelUserDayStatistics))
            {
                return;
            }
            if (channelUserDayStatistics.Sum(x => x.Count) != this.Count)
            {
                throw new ArgumentException("Incorrect messages sum");
            }
            if(channelUserDayStatistics.Where(x => x.ChannelId > 0 && x.UserId > 0).Select(x => $"{x.ChannelId} {x.UserId}").Distinct().Count() != channelUserDayStatistics.Count())
            {
                throw new ArgumentException("Incorrect pairs channel-user in channelUserDayStatistics");
            }
            this.ChannelUserDayStatistics = channelUserDayStatistics;
            this.Update();
        }
    }

    public class UserDayStatistic
    {
        public ulong UserId { get; private set; }
        public int Count { get; private set; }

        public UserDayStatistic(ulong userId, int count)
        {
            this.UserId = userId;
            this.Count = count;
        }
    }

    public class ChannelDayStatistic
    {
        public ulong ChannelId { get; private set; }
        public int Count { get; private set; }

        public ChannelDayStatistic(ulong channelId, int count)
        {
            this.ChannelId = channelId;
            this.Count = count;
        }
    }

    public class ChannelUserDayStatistic
    {
        public ulong UserId { get; private set; }
        public ulong ChannelId { get; private set; }
        public int Count { get; private set; }

        public ChannelUserDayStatistic(ulong userId, ulong channelId, int count)
        {
            this.UserId = userId;
            this.ChannelId = channelId;
            this.Count = count;
        }
    }
}
