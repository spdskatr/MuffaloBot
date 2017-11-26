using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace MuffaloBotNetFramework2.DiscordComponent.CommandsModules
{
    [Group("Simple")]
    class BasicConnectivityTests
    {
        [Command("muffalo"), Aliases("muffalobot", "muffy")]
        public async Task Muffalo(CommandContext ctx)
        { 
            await ctx.RespondAsync("😎🤖 Muffy is my name, RimWorld is my game 🤖😎");
        }
    }
}
