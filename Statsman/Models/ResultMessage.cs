using System;
using System.Collections.Generic;
using System.Text;

namespace Statsman.Models
{
    public class ResultMessage
    {
        public string Title { get; private set; }
        public string PeriodInfo { get; private set; }
        public string UserInfo { get; private set; }
        public string ChannelInfo { get; private set; }
        public string TimeRangeInfo { get; private set; }

        public ResultMessage(string title, string periodInfo, string userInfo, string channelInfo, string timeRangeInfo)
        {
            this.Title = title;
            this.PeriodInfo = periodInfo;
            this.UserInfo = userInfo;
            this.ChannelInfo = channelInfo;
            this.TimeRangeInfo = timeRangeInfo;
        }

        public IEnumerable<KeyValuePair<string, string>> GetArguments()
            => new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Period", this.PeriodInfo),
                new KeyValuePair<string, string>("User Info", this.UserInfo),
                new KeyValuePair<string, string>("Channel Info", this.ChannelInfo),
                new KeyValuePair<string, string>("Time Range", this.TimeRangeInfo)
            };
    }
}
