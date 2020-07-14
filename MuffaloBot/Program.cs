using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using MuffaloBot.Converters;

namespace MuffaloBot
{
    class Program
    {
        public DiscordClient client;
        public CommandsNextModule commandsNext;
        static Program _i;
        public static Program instance
        {
            get
            {
                if (_i == null) _i = new Program();
                return _i;
            }
        }
        public Program()
        {
            client = new DiscordClient(new DiscordConfiguration()
            {
                UseInternalLogHandler = true,
#if DEBUG
                LogLevel = LogLevel.Debug,
#else
                LogLevel = LogLevel.Info,
#endif
                TokenType = TokenType.Bot,
                Token = AuthResources.BotToken // Create a new AuthResources resource file
            });
            commandsNext = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = "!",
                EnableDefaultHelp = false
            });
            commandsNext.RegisterCommands(Assembly.GetExecutingAssembly());
            client.DebugLogger.LogMessage(LogLevel.Info, "MuffaloBot", $"Registered {commandsNext.RegisteredCommands.Count} commands", DateTime.Now);
            commandsNext.SetHelpFormatter<MuffaloBotHelpFormatter>();
            LoadModules();
        }
        public void LoadModules()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(BaseModule).IsAssignableFrom(t))
                {
                    try
                    {
                        client.AddModule((BaseModule)Activator.CreateInstance(t));
                        client.DebugLogger.LogMessage(LogLevel.Info, "MuffaloBot", $"Loaded module {t.FullName}", DateTime.Now);
                    }
                    catch (Exception e)
                    {
                        client.DebugLogger.LogMessage(LogLevel.Error, "MuffaloBot", $"Could not load module {t.FullName}: {e}", DateTime.Now);
                    }
                }
            }
        }
        public async Task StartAsync()
        {
            await client.ConnectAsync();
            await Task.Delay(-1);
        }
        static void Main(string[] args)
        {
            Program.instance.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
