using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;

namespace MuffaloBotNetFramework2.DiscordComponent.ClientModules
{
    public class RoleOnMessageManager : IClientModule
    {
        DiscordChannel channel;
        DiscordRole givenRole;
        public void BindToClient(DiscordClient client)
        {
            client.MessageCreated += MessageCreated;
        }

        async Task MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (e.Channel == channel && givenRole != null)
            {
                DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                await member.GrantRoleAsync(givenRole, "Speaking in channel #" + e.Channel.Name);
            }
        }

        public void SetChannelAndRole(DiscordChannel channel, DiscordRole role)
        {
            this.channel = channel;
            givenRole = role;
        }

        public void InitializeFronJson(JObject jObject)
        {
        }
    }
}
