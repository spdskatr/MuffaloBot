using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net.WebSocket;
using MuffaloBot.DiscordComponent;
using System.Reflection;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using System.Threading;
using static MuffaloBot.AuthResourcesCreateNewIfDownloadingFromRepo;

namespace MuffaloBot
{
    public class MuffaloBotProgram
    {
        public static Dictionary<DiscordClient, MuffaloBotProgram> createdInstances = new Dictionary<DiscordClient, MuffaloBotProgram>();

        public DiscordClient discordClient;
        public CommandsNextModule commandsNext;
        public List<IInternalModule> internalModules;
        public JObject jsonData;
        public string token;
        public string steamApiKey;
        public string globalJsonKey = "https://raw.githubusercontent.com/spdskatr/MuffaloBot/master/config/global_config.json";
        public CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public T GetModule<T>() where T : IInternalModule
        {
            for (int i = 0; i < internalModules.Count; i++)
            {
                if (internalModules[i] is T result)
                    return result;
            }
            return default(T);
        }

        public static void Main(string[] args)
        {
            try
            {
                new MuffaloBotProgram().StartAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
        public async Task StartAsync(string[] args)
        {
            // Main program
            token = BOT_TOKEN;
            steamApiKey = STEAM_APIKEY;
            Console.WriteLine("Starting up...\n\n------");

            discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });
            commandsNext = discordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = "!",
                EnableDefaultHelp = false
            });

            InstantiateAllModulesFromAssembly(Assembly.GetExecutingAssembly());
            InitializeClientComponents();
            createdInstances.Add(discordClient, this);
            await discordClient.ConnectAsync();
            await Task.Delay(-1, cancellationTokenSource.Token);
        }

        void InstantiateAllModulesFromAssembly(Assembly assembly)
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
        public void InitializeClientComponents()
        {
            HttpClient webClient = new HttpClient();
            string str = webClient.GetStringAsync(globalJsonKey).GetAwaiter().GetResult();
            jsonData = JObject.Parse(str);
            for (int i = 0; i < internalModules.Count; i++)
            {
                internalModules[i].InitializeFronJson(jsonData);
            }
        }
    }
}
