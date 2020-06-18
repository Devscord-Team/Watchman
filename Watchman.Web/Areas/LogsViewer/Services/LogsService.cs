using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    var content = reader.ReadToEnd();
                    var normalizedContent = $"[{content}]";
                    var logs = JsonConvert.DeserializeObject<IEnumerable<LogDto>>(content);
                    foreach (var log in logs)
                    {
                        yield return log;
                    }
                }
            }
        }
    }
}
