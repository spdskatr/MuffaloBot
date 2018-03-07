using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBot.ClientModules
{
    class MuffaloBotCooldownManager : BaseModule
    {
        protected override void Setup(DiscordClient client)
        {
            Client = client;
        }
        public Dictionary<string, DateTime> cooldownDict = new Dictionary<string, DateTime>();
    }
}
