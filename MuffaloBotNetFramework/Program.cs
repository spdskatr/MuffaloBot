using System;
using DSharpPlus;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MuffaloBotNetFramework.RedditComponent;
using Newtonsoft.Json;
using System.IO;
using MuffaloBotNetFramework.DiscordComponent;

namespace MuffaloBotNetFramework
{
    class Program
    {
        internal class InfoPackage
        {
            public string disc;
            public string redd;
            public string stea;
            public string redd_appid;
            internal bool DiscordTokenValid()
                => disc != null && disc.Length != 0;
            internal bool RedditTokenValid()
                => redd != null && redd.Length != 0 && redd_appid != null && redd_appid.Length != 0;
            internal bool SteamTokenValid()
                => stea != null && stea.Length != 0;
        }
        const string MessageRespondOverflow = "The result that came out of the bot was over 2000 characters long!";

        internal static DiscordClient discord;

#if RedditEnabled
        internal static RedditBase rbase;
#endif

        internal static InfoPackage infoPackage;

        internal static bool sandbox = true;

        static volatile int heartbeatcount;

        static int lastheartbeat;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting MuffaloBot...");
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string text = File.ReadAllText("config.json");
            InfoPackage package = infoPackage = await Task.Run(() => JsonConvert.DeserializeObject<InfoPackage>(text));
#if RedditEnabled
            if (package.RedditTokenValid())
                (rbase = new RedditBase()).StartAsync(package.redd, package.redd_appid);
#endif
            if (package.DiscordTokenValid())
                await StartDiscordComponent(package.disc);
        }

        static async Task StartDiscordComponent(string token)
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

        static Task Say(string message, ulong guildId, int channelPos)
        {
            return discord.Guilds[guildId].Channels[channelPos].SendMessageAsync(message);
        }
    }
}