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
        ulong roleId;
        JObject jObjectCache;
        bool ready;
        public void BindToClient(DiscordClient client)
        {
            client.MessageCreated += MessageCreated;
            client.Ready += OnReady;
        }

        Task OnReady(DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            ready = true;
            LoadJson();
            return Task.CompletedTask;
        }

        async Task MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (e.Channel == channel)
            {
                DiscordRole role = e.Guild.GetRole(roleId);
                if (role != null)
                {
                    DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                    await member.GrantRoleAsync(role, "Speaking in channel #" + e.Channel.Name);
                }
            }
        }

        public void InitializeFronJson(JObject jObject)
        {
            jObjectCache = jObject;
            if (ready) LoadJson();
        }

        void LoadJson()
        {
            channel = null;
            roleId = 0;
            JToken roleOnMessageToken = jObjectCache["roleOnMessage"];
            if (roleOnMessageToken != null)
            {
                DiscordGuild guild = MuffaloBot.discordClient.Guilds[roleOnMessageToken["guild"].Value<ulong>()];
                channel = guild.GetChannelsAsync().GetAwaiter().GetResult().First(c => c.Id == roleOnMessageToken["channel"].Value<ulong>());
                roleId = roleOnMessageToken["role"].Value<ulong>();
            }
        }
    }
}
