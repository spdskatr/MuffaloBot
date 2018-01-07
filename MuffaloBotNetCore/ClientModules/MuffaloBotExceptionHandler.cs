using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Exceptions;
using MuffaloBotNetFramework2;

namespace MuffaloBotNetFramework2.ClientModules
{
    class MuffaloBotExceptionHandler : BaseModule
    {
        public async Task HandleClientError(CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException || e.Exception is UnauthorizedException || e.Exception is ChecksFailedException || e.Exception.Message.StartsWith("Could not convert specified value to given type.")) return;
            await HandleClientError(e.Exception, "Command " + (e.Command?.Name ?? "unknown"));
        }

        public Task HandleClientError(ClientErrorEventArgs e)
        {
            return HandleClientError(e.Exception, "Event " + e.EventName);
        }
        public async Task HandleClientError(Exception e, string action)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithTitle("Unhandled exception");
            builder.WithDescription($"Action: {action}\n```\n{e.ToString()}```");
            builder.WithColor(DiscordColor.Red);
            DiscordChannel channel = await Client.CreateDmAsync(Client.CurrentApplication.Owner);
            await MuffaloBot.discordClient.SendMessageAsync(channel, embed: builder.Build());
        }

        protected override void Setup(DiscordClient client)
        {
            Client = client;
            client.ClientErrored += HandleClientError;
            client.GetCommandsNext().CommandErrored += HandleClientError;
        }
    }
}
