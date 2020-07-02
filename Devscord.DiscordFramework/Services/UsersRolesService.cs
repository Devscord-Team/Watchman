<<<<<<< HEAD
using Devscord.DiscordFramework.Commons;
=======
﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
>>>>>>> master
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Services
{
    public class UsersRolesService
    {
        public const string MUTED_ROLE_NAME = "muted";

<<<<<<< HEAD
        public UserRole CreateNewRole(DiscordServerContext server, NewUserRole userRole)
        {
            return Server.CreateNewRole(userRole, server).Result;
        }
=======
        public async Task<UserRole> CreateNewRole(DiscordServerContext server, NewUserRole userRole)
        {
            return await Server.CreateNewRole(userRole, server);
        }
>>>>>>> master
        public UserRole GetRoleByName(string name, DiscordServerContext server)
        {
            return Server.GetRoles(server.Id).FirstOrDefault(x => x.Name == name);
        }

<<<<<<< HEAD
        public IEnumerable<UserRole> GetRoles(DiscordServerContext server)
        {
            return Server.GetRoles(server.Id).ToList();
        }
=======
        public IEnumerable<UserRole> GetRoles(DiscordServerContext server)
        {
            return Server.GetRoles(server.Id);
        }
>>>>>>> master
    }
}
