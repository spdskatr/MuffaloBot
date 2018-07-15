using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MuffaloBot;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Net.Http;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace MuffaloBot.Commands
{
    public class EvalGobals
    {
        public CommandContext ctx;
    }
    public class Meta
    {
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
Beta testing discord: https://discord.gg/6MHVepE
This bot account will not have an invite link. It is exclusive to the RimWorld discord.");
            embedBuilder.WithColor(DiscordColor.Azure);
            return ctx.RespondAsync(embed: embedBuilder.Build());
        }
        [Command("version"), Hidden]
        public Task GetVersionAsync(CommandContext ctx)
        {
            AssemblyName name = Assembly.GetExecutingAssembly().GetName();
            return ctx.RespondAsync($"{name.Name} Version {name.Version}");
        }
        [Command("status"), RequireOwner, Hidden]
        public async Task SetStatusAsync(CommandContext ctx, string status)
        {
            await ctx.Client.UpdateStatusAsync(new DiscordGame(status));
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString());
        }
        [Command("die"), RequireOwner, Hidden]
        public async Task DieAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Restarting...");
            await ctx.Client.DisconnectAsync();
            Environment.Exit(0);
        }
        [Command("crash"), RequireOwner, Hidden]
        public Task CrashAsync(CommandContext ctx)
        {
            throw new Exception("oops.");
        }
        [Command("roleid")]
        public Task GetRoleAsync(CommandContext ctx, DiscordRole role)
        {
            return ctx.RespondAsync(role.Id.ToString());
        }
        [Command("isimmortal"), RequireOwner, Hidden]
        public Task IsImmortalAsync(CommandContext ctx)
        {
            return ctx.RespondAsync(Environment.GetCommandLineArgs().Any(s => s == "-immortal").ToString());
        }
        [Command("eval"), RequireOwner, Hidden]
        public async Task EvalAsync(CommandContext ctx, [RemainingText] string code)
        {
            await ctx.TriggerTypingAsync().ConfigureAwait(false);
            string actualCode = code.TrimStart('`', 'c', 's', 'h', 'a', 'r', 'p').TrimEnd('`');
            ScriptOptions options = ScriptOptions.Default.WithImports("System", "System.Collections.Generic", "System.Diagnostics", "System.Linq", "System.Net.Http", "System.Reflection", "System.Text", "System.Text.RegularExpressions", "System.Threading.Tasks", "DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Entities", "DSharpPlus.EventArgs", "DSharpPlus.Exceptions", "MuffaloBot", "MuffaloBot.Commands")
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));
            Script script = CSharpScript.Create(actualCode, options, typeof(EvalGobals));

            Exception ex = null;
            ScriptState state = null;
            try
            {
                state = await script.RunAsync(new EvalGobals() { ctx = ctx }).ConfigureAwait(false);
                ex = state.Exception;
            }
            catch (Exception e)
            {
                ex = e;
            }

            if (ex != null)
            {
                await ctx.RespondAsync($"**Error** ```{ex}```");
            }
            else
            {
                await ctx.RespondAsync($"Result: ```{state?.ReturnValue ?? "(null)"}```");
            }
        }
    }
}
