using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.Framework.Middlewares
{
    public interface IMiddleware<T>
    {
        Task Process(T data);
    }
}
