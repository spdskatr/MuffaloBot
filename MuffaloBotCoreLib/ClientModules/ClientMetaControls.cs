using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBotCoreLib.ClientModules
{
    public class ClientMetaControls : BaseModule
    {
        protected override void Setup(DiscordClient client)
        {
            Client = client;
            client.GuildMemberAdded += GuildMemberAdded;
        }
        const ulong TargetGuild = 214523379766525963;
        const ulong TargetMember = 114899599579283459;
        private async Task GuildMemberAdded(DSharpPlus.EventArgs.GuildMemberAddEventArgs e)
        {
            if (e.Guild.Id == TargetGuild)
            {
                DiscordGuild guild = await Client.GetGuildAsync(TargetGuild);
                switch (guild.MemberCount)
                {
                    case 2985:
                    case 2990:
                    case 2995:
                        DiscordDmChannel channel = await Client.CreateDmAsync(await Client.GetUserAsync(TargetMember));
                        await channel.SendMessageAsync("Member count: " + guild.MemberCount);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
