using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;

namespace MuffaloBotNetFramework2.DiscordComponent
{
    class MuffaloBotExceptionHandler : IClientModule
    {
        public void BindToClient(DiscordClient client)
        {
            client.ClientErrored += HandleClientError;
        }

        public void InitializeFronJson(JObject jObject)
        {
        }

        public Task HandleClientError(ClientErrorEventArgs e)
        {
            return HandleClientError(e.Exception);
        }
        public async Task HandleClientError(Exception e)
        {
            await Console.Out.WriteLineAsync(e.ToString());
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithTitle("Unhandled exception");
            builder.WithDescription($"```\n{e.ToString()}```");
            builder.WithColor(DiscordColor.Red);
            DiscordChannel channel = await MuffaloBot.discordClient.CreateDmAsync(MuffaloBot.discordClient.CurrentApplication.Owner);
            await MuffaloBot.discordClient.SendMessageAsync(channel, embed: builder.Build());
        }
    }
}
