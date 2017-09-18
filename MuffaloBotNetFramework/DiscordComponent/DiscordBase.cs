using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework.DiscordComponent
{
    class DiscordBase
    {
        internal DiscordClient discord;
        internal static bool sandbox = true;
        const string MessageRespondOverflow = "The result that came out of the bot was over 2000 characters long!";
        static volatile int heartbeatcount;
        static int lastheartbeat;
        internal async Task StartDiscordComponent(string token)
        {
            bool ready = false;
            try
            {
                discord = new DiscordClient(new DiscordConfig
                {
                    Token = token,
                    TokenType = TokenType.Bot
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to start Discord component, probably due to invalid token. Exception was: \n{0}", e);
                return;
            }
            sandbox = false;
            discord.MessageCreated += async e =>
            {
                if (e.Author == discord.CurrentUser) return;
                // Help msg
                if (e.Message.Content == "!mbhelp")
                {
                    var task = discord.CreateDmAsync(e.Author);
                    var dmchannel = await task;
                    await dmchannel.SendMessageAsync(DiscordRoot.help_msg);
                    return;
                }
                // Response to Wolfy's self destruct trigger
                if (e.Message.Content == "Selfdestruct initialized." && e.Author.IsBot)
                {
                    await e.Message.RespondAsync("No! Don't!");
                }
                if (e.Message.Content == "Deleting system32 will begin in 5 seconds" && e.Author.IsBot)
                {
                    await e.Message.RespondAsync("Stop! Resist the temptation!");
                    await Task.Delay(1000);
                    var message = await e.Message.RespondAsync("Noooo!");
                    await Task.Delay(1000);
                    await message.EditAsync("Noooooooooooooooooooooooooooo!");
                    await Task.Delay(1000);
                    await message.EditAsync("Nooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo!");
                }
                var t = ProcessMessage(e.Message);
                if (t != null) await t;
            };
            discord.Ready += async e =>
            {
                await Console.Out.WriteLineAsync($"Discord component :: Ready! Gateway ver {e.Client.GatewayVersion}");
                ready = true;
            };
            var timer = new Timer(async s =>
            {
                if (heartbeatcount == lastheartbeat && ready)
                {
                    await Console.Out.WriteLineAsync("Heartbeat timed out. Reconnecting...");
                    await discord.ReconnectAsync();
                }
                lastheartbeat = heartbeatcount;
            }, null, 120000, 120000);
            discord.Heartbeated += async (e) =>
            {
                await Console.Out.WriteLineAsync($"Discord component :: {DateTime.UtcNow} UTC :: Ping {discord.Ping}");
                heartbeatcount++;
            };
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        static Task ProcessMessage(DiscordMessage message)
        {
            var s = DiscordRoot.ProcessString(message.Content, message.Channel.Guild.Id, message.Channel.Position);
            if (s != null)
            {
                if (s.message.Length > 2000)
                    return message.RespondAsync(MessageRespondOverflow);
                return message.RespondAsync(s.message, embed: s.embed);
            }
            else
                return null;
        }

        Task Say(string message, ulong guildId, int channelPos)
        {
            return discord.Guilds[guildId].Channels[channelPos].SendMessageAsync(message);
        }
    }
}
