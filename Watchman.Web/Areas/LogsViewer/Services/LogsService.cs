using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Watchman.Web.Areas.LogsViewer.Models.Dtos;

namespace Watchman.Web.Areas.LogsViewer.Services
{
    public class LogsService
    {
        public IEnumerable<LogDto> GetLogs(GetLogsRequest request)
        {
            foreach (var filePath in Directory.GetFiles("logs").Where(x => Path.GetExtension(x) == ".json"))
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(stream);

                var content = streamReader.ReadToEnd();
                var normalizedContent = "[" + content.Trim(",[]".ToCharArray()) + "]";
                var logs = JsonConvert.DeserializeObject<IEnumerable<LogDto>>(normalizedContent);

                foreach (var log in logs.Where(x => !this.ShouldSkip(request, x)))
                {
                    yield return log;
                }
            }
        }

        private bool ShouldSkip(GetLogsRequest request, LogDto log)
        {
            if (request.FromTime.HasValue && request.FromTime < log.Timestamp)
            {
                return false;
            }

            if (request.ToTime.HasValue && request.ToTime > log.Timestamp)
            {
                return false;
            }

            if (request.Level != log.Level)
            {
                return false;
            }

            foreach (var requirement in request.PropertiesRequirements)
            {
                if (!log.Properties.ContainsKey(requirement.Key))
                {
                    return false;
                }

                if (log.Properties[requirement.Key] != requirement.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

