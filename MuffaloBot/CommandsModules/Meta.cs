using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MuffaloBot.DiscordComponent;
using MuffaloBot;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Net.Http;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace MuffaloBot.CommandsModules
{
    public class EvalGobals
    {
        public CommandContext ctx;
    }
    [MuffaloBotCommandsModule]
    class Meta
    {
        [Command("version"), Hidden]
        public Task GetVersion(CommandContext ctx)
        {
            AssemblyName name = Assembly.GetExecutingAssembly().GetName();
            return ctx.RespondAsync($"{name.Name} Version {name.Version}");
        }
        [Command("status"), RequireOwner, Hidden]
        public async Task SetStatus(CommandContext ctx, string status)
        {
            await ctx.Client.UpdateStatusAsync(new DiscordGame(status));
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:").ToString());
        }
        [Command("die"), RequireOwner]
        public async Task Die(CommandContext ctx)
        {
            await ctx.RespondAsync("Restarting...");
            await ctx.Client.DisconnectAsync();
        }
        [Command("exception"), RequireOwner, Hidden]
        public Task Crash(CommandContext ctx)
        {
            throw new Exception("oops.");
        }
        [Command("roleid")]
        public Task GetRole(CommandContext ctx, DiscordRole role)
        {
            return ctx.RespondAsync(role.Id.ToString());
        }
        [Command("eval"), RequireOwner]
        public async Task Eval(CommandContext ctx, [RemainingText] string code)
        {
            await ctx.TriggerTypingAsync().ConfigureAwait(false);
            string actualCode = code.TrimStart('`', 'c', 's', 'h', 'a', 'r', 'p').TrimEnd('`');
            ScriptOptions options = ScriptOptions.Default.WithImports("System", "System.Collections.Generic", "System.Diagnostics", "System.Linq", "System.Net.Http", "System.Reflection", "System.Text", "System.Text.RegularExpressions", "System.Threading.Tasks", "DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Entities", "DSharpPlus.EventArgs", "DSharpPlus.Exceptions", "MuffaloBot", "MuffaloBot.CommandsModules")
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
