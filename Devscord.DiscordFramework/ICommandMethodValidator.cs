using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Devscord.DiscordFramework
{
    public interface ICommandMethodValidator
    {
        bool IsValid(Contexts contexts, MethodInfo method);
    }
}
