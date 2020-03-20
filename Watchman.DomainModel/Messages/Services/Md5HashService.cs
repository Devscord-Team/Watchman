using System.Security.Cryptography;
using System.Text;

namespace Watchman.DomainModel.Messages.Services
{
    public class Md5HashService
    {
        public string GetHash(Message message)
        {
            var stringToMd5 = message.SentAt.Ticks.ToString() + message.Author;
            var bytes = Encoding.ASCII.GetBytes(stringToMd5);
            using var md5 = MD5.Create();
            var hashed = md5.ComputeHash(bytes);

            var builder = new StringBuilder();
            foreach (var b in hashed)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
