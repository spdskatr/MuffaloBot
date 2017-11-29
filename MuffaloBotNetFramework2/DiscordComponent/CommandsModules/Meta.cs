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
        public Task SetStatus(CommandContext ctx, string status)
        {
            return ctx.Client.UpdateStatusAsync(new DiscordGame(status)).ContinueWith(t => ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString()));
        }
        [Command("exception"), RequireOwner, Hidden]
        public Task Crash(CommandContext ctx)
        {
            throw new Exception("oops.");
        }
        [Command("reinit"), RequireOwner, Hidden]
        public Task ReinitAsync(CommandContext ctx)
        {
            return Task.Run((Action)MuffaloBot.InitializeClientComponents).ContinueWith(t => ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString()));
        }
        [Command("roleid")]
        public Task GetRole(CommandContext ctx, DiscordRole role)
        {
            return ctx.RespondAsync(role.Id.ToString());
        }
    }
}
