using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MuffaloBotNetFramework2.DiscordComponent;
using MuffaloBotNetFramework2;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace MuffaloBotCoreLib.CommandsModules
{
    [MuffaloBotCommandsModule]
    class Meta
    {
        [Command("version"), RequireOwner, Hidden]
        public Task GetVersion(CommandContext ctx)
        {
            return ctx.RespondAsync("MuffaloBotCoreLib Version " + Assembly.GetExecutingAssembly().GetName().Version);
        }
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
        [Command("usercount")]
        public Task GetUserCount(CommandContext ctx)
        {
            return ctx.Client.GetGuildAsync(ctx.Guild.Id).ContinueWith(t => ctx.RespondAsync("User count: " + t.Result.MemberCount));
        }
        [Command("die"), RequireOwner, Hidden]
        public async Task Die(CommandContext ctx, string coreLibPath = null)
        {
            List<string> newArgs = new List<string>();
            if (coreLibPath != null)
            {
                if (Uri.TryCreate(coreLibPath, UriKind.Absolute, out Uri uri))
                {
                    newArgs.Add("--corelib-path=" + coreLibPath);
                }
                else
                {
                    await ctx.RespondAsync("Path not valid; Skipping...");
                }
            }
            await ctx.RespondAsync("Restarting...");
            await ctx.Client.DisconnectAsync();
            await Console.Out.WriteLineAsync("\n\n\nRESTARTING\\n\n\n");
            ctx.Client.Dispose();
            new Thread(() => RestartBot(newArgs)).Start();
        }
        void RestartBot(List<string> newArgs)
        {
            MuffaloBot.MainAsync(newArgs.ToArray()).ConfigureAwait(false);
        }
    }
}
