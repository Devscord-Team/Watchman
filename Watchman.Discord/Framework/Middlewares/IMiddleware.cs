using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Framework.Middlewares
{
    public interface IMiddleware<T>
    {
        Task Process(T data);
    }
}
