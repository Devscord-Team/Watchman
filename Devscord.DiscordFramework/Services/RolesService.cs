using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class RolesService
    {
        public Task CreateNewRole(Contexts contexts, UserRole userRole)
        {
            return Server.CreateNewRole(userRole, contexts.Server);
        }
    }
}
