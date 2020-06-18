using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Watchman.Web.Areas.LogsViewer.Models.Dtos;

namespace Watchman.Web.Areas.LogsViewer.Services
{
    public class LogsService
    {
        public IEnumerable<LogDto> GetLogs()
        {
            foreach (var filePath in Directory.GetFiles("logs").Where(x => Path.GetExtension(x) == ".json"))
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(stream);

                var content = streamReader.ReadToEnd();
                var normalizedContent = "[" + content.Trim(",[]".ToCharArray()) + "]";
                var logs = JsonConvert.DeserializeObject<LogDto[]>(normalizedContent);

                foreach (var log in logs)
                {
                    yield return log;
                }
            }
        }
    }
}

