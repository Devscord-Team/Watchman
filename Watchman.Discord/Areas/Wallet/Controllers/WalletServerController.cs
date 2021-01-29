﻿using Devscord.DiscordFramework.Framework.Architecture.Controllers;

namespace Watchman.Discord.Areas.Wallet.Controllers
{
    /*
     * every server should have basic account balance and users that can manage that (with different permissions),
     * every server should have sections with own budget (like 1000 money monthly for "career questions" section, 100 money monthly for "political questions" section etc)
     * as "money for section" I mean money that moderator that is responsible for this section, can give to users monthly (for example if max budget is 1000, and there is +/- 10 helpful answers monthly, he/she can give 50-200 money for helpful answer -> depends on how much helpful it was)
     * 
     * budget vs income
     * sections have budget instead income, so that they cannot to cross they maximum budget, or in easier words -> they cannot save money from month to month
     * income - servers have monthly income from us - when they grow, they have to have more money, but there should be restricted rules and income in free account should be as small as it is possible
     * there should be also option to buy more income or just single money boost from us 
     * (inflaction is bad for rich people, but can help poor people... and is easiest way to manipulate government)
     */
    public class WalletServerController : IController 
    {
        public void CreateSection()
        {
        }

        public void RemoveSection()
        {
        }

        public void CreateSectionsGroup()
        {
        }

        public void AddSectionToGroup()
        {
        }

        public void RemoveSectionFromGroup()
        {
        }

        public void SetUserPermissionsInSections()
        {
        }

        public void SetUserPermissionsInSectionsGroup()
        {
        }

        public void SetSectionMonthlyBudget()
        {
        }

        public void SetServerMonthlyIncome()
        {

        }

        public void SetTransactionTax()
        {

        }

        public void SetBoostingAdditionalIncome()
        {

        }
    }
}
