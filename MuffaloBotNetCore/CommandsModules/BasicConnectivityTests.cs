using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MuffaloBotNetFramework2.DiscordComponent;

namespace MuffaloBotNetFramework2.CommandsModules
{
    [MuffaloBotCommandsModule]
    class BasicConnectivityTests
    {
        [Command("muffalo"), Aliases("muffalobot", "muffy")]
        public async Task Muffalo(CommandContext ctx)
        { 
            await ctx.RespondAsync("😎🤖 Muffy is my name, RimWorld is my game 🤖😎");
        }
        [Command("wolfy")]
        public async Task Wolfy(CommandContext ctx)
        {
            DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":awoo:");
            await ctx.RespondAsync(emoji.ToString());
        }
        [Command("about")]
        public Task About(CommandContext ctx)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle("About MuffaloBot");
            embedBuilder.WithUrl("https://github.com/spdskatr/MuffaloBot");
            embedBuilder.WithDescription(@"Contributors: spdskatr
Library: [DSharpPlus](https://dsharpplus.emzi0767.com/) (.NET)
Other libraries: [Magick.NET](https://github.com/dlemstra/Magick.NET) Wrapper for [ImageMagick](http://www.imagemagick.org/) (.NET)
Hosted by: Zirr
GitHub Repository: https://github.com/spdskatr/MuffaloBot
This bot account will not have an invite link. It is exclusive to a few guilds that have the bot.");
            embedBuilder.WithColor(DiscordColor.Azure);
            return ctx.RespondAsync(embed: embedBuilder.Build());
        }
    }
}
