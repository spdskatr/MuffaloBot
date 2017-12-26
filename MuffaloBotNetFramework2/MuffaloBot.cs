using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net.WebSocket;
using MuffaloBotNetFramework2.DiscordComponent;
using System.Reflection;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using System.Threading;

namespace MuffaloBotNetFramework2
{
    public static class MuffaloBot
    {
        public static DiscordClient discordClient;
        public static CommandsNextModule commandsNext;
        public static List<IInternalModule> internalModules;
        public static JObject jsonData;
        public static string token;
        public static string steamApiKey;
        public static string globalJsonKey = "https://raw.githubusercontent.com/spdskatr/MuffaloBot/master/config/global_config.json";
        public static Thread mainThread;
        public static T GetModule<T>() where T : IInternalModule
        {
            for (int i = 0; i < internalModules.Count; i++)
            {
                if (internalModules[i] is T result)
                    return result;
            }
            return default(T);
        }
        static void Main(string[] args)
        {
            mainThread = Thread.CurrentThread;
            try
            {
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                return; // Exit gracefully
            }
        }
        public static async Task MainAsync(string[] args)
        {
            // Main program
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
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            }
            commandsNext = discordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = "!",
                EnableDefaultHelp = false
            });

            InstantiateAllModulesFromAssembly(Assembly.GetExecutingAssembly());
            InitializeClientComponents();

            Console.WriteLine("------\n\n");
            await discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        static void InstantiateAllModulesFromAssembly(Assembly assembly)
        {
            internalModules = new List<IInternalModule>();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                // Client modules
                if (typeof(IInternalModule).IsAssignableFrom(types[i]) && !types[i].IsAbstract && types[i].GetConstructor(new Type[0]) != null)
                {
                    IInternalModule clientModule = (IInternalModule)Activator.CreateInstance(types[i]);
                    clientModule.BindToClient(discordClient);
                    internalModules.Add(clientModule);
                    Console.WriteLine($"Registered internal module {types[i].FullName} from assembly {assembly.GetName().FullName}");
                }

                // DSharpPlus base modules
                else if (typeof(BaseModule).IsAssignableFrom(types[i]) && !types[i].IsAbstract && types[i].GetConstructor(new Type[0]) != null)
                {
                    discordClient.AddModule((BaseModule)Activator.CreateInstance(types[i]));
                }

                // CommandsNext modules
                else if (types[i].GetCustomAttribute<MuffaloBotCommandsModuleAttribute>() != null && !types[i].IsAbstract && types[i].GetConstructor(new Type[0]) != null)
                {
                    commandsNext.RegisterCommands(types[i]);
                    Console.WriteLine($"Registered command module {types[i].FullName} from assembly {assembly.FullName}");
                }
            }
        }
        public static void InitializeClientComponents()
        {
            HttpClient webClient = new HttpClient();
            string str = webClient.GetStringAsync(globalJsonKey).GetAwaiter().GetResult();
            Console.WriteLine(str);
            jsonData = JObject.Parse(str);
            for (int i = 0; i < internalModules.Count; i++)
            {
                internalModules[i].InitializeFronJson(jsonData);
            }
        }
    }
}
