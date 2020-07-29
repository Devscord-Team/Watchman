using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.Areas.Administration.Services
{
    public class TrustRolesService
    {
        private readonly ConfigurationService _configurationService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly CheckUserSafetyService _checkUserSafetyService;

        public TrustRolesService(ConfigurationService configurationService, MessagesServiceFactory messagesServiceFactory, CheckUserSafetyService checkUserSafetyService)
        {
            this._configurationService = configurationService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._checkUserSafetyService = checkUserSafetyService;
        }

        public async Task TrustThisRole(string roleName, Contexts contexts)
        {
            var roles = this._configurationService.GetConfigurationItem<TrustedUserRolesNames>(contexts.Server.Id).Value.ToList();
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (roles.Contains(roleName))
            {
                await messagesService.SendResponse(x => x.RoleAlreadyIsTrusted(roleName));
                return;
            }
            if (contexts.Server.Roles.All(x => x.Name != roleName))
            {
                throw new RoleNotFoundException(roleName);
            }
            roles.Add(roleName);
            var newTrustedRoles = new TrustedUserRolesNames(contexts.Server.Id)
            {
                Value = roles
            };
            await this._configurationService.SaveNewConfiguration(newTrustedRoles);
            await messagesService.SendResponse(x => x.RoleSetAsTrusted(roleName));
            await this._checkUserSafetyService.Refresh();
        }

        public async Task DontTrustThisRole(string roleName, Contexts contexts)
        {
            var roles = this._configurationService.GetConfigurationItem<TrustedUserRolesNames>(contexts.Server.Id).Value.ToList();
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (!roles.Contains(roleName))
            {
                await messagesService.SendResponse(x => x.RoleAlreadyIsUntrusted(roleName));
                return;
            }
            roles.Remove(roleName);
            var newTrustedRoles = new TrustedUserRolesNames(contexts.Server.Id)
            {
                Value = roles
            };
            await this._configurationService.SaveNewConfiguration(newTrustedRoles);
            await messagesService.SendResponse(x => x.RoleSetAsUntrusted(roleName));
            await this._checkUserSafetyService.Refresh();
        }

        public IEnumerable<string> GetTrustedRolesNames(ulong serverId)
        {
            return this._configurationService.GetConfigurationItem<TrustedUserRolesNames>(serverId).Value;
        }
    }
}
