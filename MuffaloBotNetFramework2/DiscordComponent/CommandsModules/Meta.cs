using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace MuffaloBotNetFramework2.DiscordComponent.CommandsModules
{
    [MuffaloBotCommandsModule]
    class Meta
    {
        [Command("status"), RequireOwner, Hidden]
        public async Task SetStatus(CommandContext ctx, string status)
        {
            await ctx.Client.UpdateStatusAsync(new Game(status));
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString());
        }
        [Command("exception"), RequireOwner, Hidden]
        public async Task Crash(CommandContext ctx)
        {
            await MuffaloBot.GetModule<MuffaloBotExceptionHandler>().HandleClientError(new Exception("oops"));
        }
    }
}
