using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net.WebSocket;
using MuffaloBotNetFramework2.DiscordComponent;
using System.Reflection;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;

namespace MuffaloBotNetFramework2
{
    static class MuffaloBot
    {
        public static DiscordClient discordClient;
        public static CommandsNextModule commandsNext;
        public static List<IClientModule> clientModules;
        public static JObject jsonData;
        public static string token;
        public static string steamApiKey;
        public const string globalJsonKey = "https://raw.githubusercontent.com/spdskatr/MuffaloBot/master/config/global_config.json";
        public static T GetModule<T>() where T : IClientModule
        {
            for (int i = 0; i < clientModules.Count; i++)
            {
                if (clientModules[i] is T result)
                    return result;
            }
            return default(T);
        }
        static void Main(string[] args)
        {
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            token = File.ReadAllText("token.txt");
            steamApiKey = File.ReadAllText("steam_apikey.txt");
            Console.WriteLine("Starting up...\n\n------");

            discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
            {
                await Console.Out.WriteLineAsync("Detected Windows 7; Switching to alternate WebSocket.");
                discordClient.SetWebSocketClient<WebSocket4NetClient>();
            }
            if (Type.GetType("Mono.Runtime") != null)
            {
                await Console.Out.WriteLineAsync("Detected Mono; Switching to alternate WebSocket.");
                discordClient.SetWebSocketClient<WebSocketSharpClient>();
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            }
            commandsNext = discordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = "!",
                EnableDefaultHelp = false
            });

            InstantiateAllModules();
            InitializeClientComponents();

            Console.WriteLine("------\n\n");
            await discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
        static void InstantiateAllModules()
        {
            clientModules = new List<IClientModule>();
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                // Client modules
                if (typeof(IClientModule).IsAssignableFrom(types[i]) && !types[i].IsAbstract && types[i].GetConstructor(new Type[0]) != null)
                {
                    IClientModule clientModule = (IClientModule)Activator.CreateInstance(types[i]);
                    clientModule.BindToClient(discordClient);
                    clientModules.Add(clientModule);
                    Console.WriteLine($"Registered client module {types[i].FullName}");
                }

                // CommandsNext modules
                if (types[i].GetCustomAttribute<MuffaloBotCommandsModuleAttribute>() != null)
                {
                    commandsNext.RegisterCommands(types[i]);
                    Console.WriteLine($"Registered command module {types[i].FullName}");
                }
            }
        }
        public static void InitializeClientComponents()
        {
            WebClient webClient = new WebClient();
            string str = webClient.DownloadString(globalJsonKey);
            Console.WriteLine(str);
            jsonData = JObject.Parse(str);
            for (int i = 0; i < clientModules.Count; i++)
            {
                clientModules[i].InitializeFronJson(jsonData);
            }
        }
    }
}
