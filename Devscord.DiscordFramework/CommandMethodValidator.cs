using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Devscord.DiscordFramework
{
    public class CommandMethodValidator
    {
        public bool IsValid(Contexts contexts, MethodInfo method)
        {
            this.CheckPermissions(method, contexts);
            return true;
        }

        private void CheckPermissions(MethodInfo method, Contexts contexts)
        {
            Log.Information("Checking permissions for user {user} for method {method}", contexts.User.ToString(), method.Name);
            if (method.HasAttribute<AdminCommand>() && !contexts.User.IsAdmin())
            {
                throw new NotAdminPermissionsException();
            }
            Log.Information("User {user} have permissions for method {method}", contexts.User.ToString(), method.Name);
        }
    }
}
