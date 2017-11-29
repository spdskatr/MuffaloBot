using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MuffaloBotNetFramework2.DiscordComponent.ClientModules;

namespace MuffaloBotNetFramework2.DiscordComponent.CommandsModules
{
    [MuffaloBotCommandsModule]
    class Meta
    {
        [Command("status"), RequireOwner, Hidden]
        public async Task SetStatus(CommandContext ctx, string status)
        {
            await ctx.Client.UpdateStatusAsync(new DiscordGame(status));
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString());
        }
        [Command("exception"), RequireOwner, Hidden]
        public Task Crash(CommandContext ctx)
        {
            throw new Exception("oops.");
        }
        [Command("reinit"), RequireOwner, Hidden]
        public async Task ReinitAsync(CommandContext ctx)
        {
            await Task.Run((Action)MuffaloBot.InitializeClientComponents);
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString());
        }
        [Command("roleonmessage"), RequireOwner, Hidden]
        public async Task RoleOnMessage(CommandContext ctx, DiscordChannel channel, DiscordRole role)
        {
            MuffaloBot.GetModule<RoleOnMessageManager>().SetChannelAndRole(channel, role);
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString());
        }
    }
}
