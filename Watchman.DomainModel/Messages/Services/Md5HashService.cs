using System.Security.Cryptography;
using System.Text;

namespace Watchman.DomainModel.Messages.Services
{
    public interface IHashService
    {
        public string GetHash(Message message);
    }

    public class Md5HashService : IHashService
    {
        public string GetHash(Message message)
        {
            var stringToMd5 = message.SentAt.Ticks.ToString() + message.Author; // todo: should be message.Author.Id, but it's gonna destroy all message history in base
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
