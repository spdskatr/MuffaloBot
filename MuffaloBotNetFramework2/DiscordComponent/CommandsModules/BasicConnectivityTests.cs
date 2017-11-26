using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace MuffaloBotNetFramework2.DiscordComponent.CommandsModules
{
    [MuffaloBotCommandsModule]
    class BasicConnectivityTests
    {
        [Command("muffalo"), Aliases("muffalobot", "muffy"), Description("Simple test command.")]
        public async Task Muffalo(CommandContext ctx)
        { 
            await ctx.RespondAsync("😎🤖 Muffy is my name, RimWorld is my game 🤖😎");
        }
        [Command("wolfy"), Description("Simple test command with an emoji")]
        public async Task Wolfy(CommandContext ctx)
        {
            DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":awoo:");
            await ctx.RespondAsync(emoji.ToString());
        }
    }
}
