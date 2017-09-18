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
#if BETA
        const string targetSubreddit = "bottesting";
#else
        const string targetSubreddit = "RimWorld";
#endif

        internal class InfoPackage
        {
            public string disc;
            public string redd;
            public string stea;
            public string redd_appid;
            internal bool DiscordTokenValid()
                => this != null && disc != null && disc.Length != 0;
            internal bool RedditTokenValid()
                => this != null && redd != null && redd.Length != 0 && redd_appid != null && redd_appid.Length != 0;
            internal bool SteamTokenValid()
                => this != null && stea != null && stea.Length != 0;
        }

#if RedditEnabled
        internal static RedditBase rbase;
#endif

        internal static InfoPackage infoPackage;

        internal static DiscordBase dBase;

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
            {
                try
                {
                    (rbase = new RedditBase()).StartAsync(package.redd, package.redd_appid, targetSubreddit);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync("Could not start Reddit component due to exception being thrown. Exception was:\n" + ex);
                }
            }
#endif
            if (package.DiscordTokenValid())
            {
                try
                {
                    await (dBase = new DiscordBase()).StartDiscordComponent(package.disc);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync("Could not start Discord component due to exceoption being thrown. Exception was:\n" + ex);
                }
            }
        }

    }
}