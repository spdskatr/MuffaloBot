using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffaloBotNetFramework.CommandsUtil;
using RedditSharp.Things;
using RedditSharp;

namespace MuffaloBotNetFramework.RedditComponent
{
    static class RedditRoot
    {
        // Command is a string of alphanumeric chars
        // Operands is a space separated list of alphanumeric chars
        internal static string ProcessCommand(string command, string operands)
        {
            switch (command)
            {
                case "wikisearch":
                    return Commands.WikiSearch(operands);
                case "wshopsearch":
                    if (Program.infoPackage.SteamTokenValid())
                    {
                        return Commands.SteamWorkshopSearch(operands, Program.infoPackage.stea);
                    }
                    break;
                case "basestats":
                    return Commands.GetBaseStats(operands);
                case "stuffstats":
                    return Commands.GetStuffStats(operands);
                default:
                    break;
            }
            return null;
        }
        internal static async Task ReplyAsync(this Comment item, string message)
        {
            try
            {
                await Task.Run(() => item.Reply(message));
            }
            // If sending too fast...
            catch (RateLimitException e)
            {
                await Console.Out.WriteLineAsync($"Reddit Component :: Experienced rate limit exception when replying. Resending in {e.TimeToReset.Milliseconds}ms...");
                await Task.Delay(e.TimeToReset);
                await ReplyAsync(item, message);
            }
        }
    }
}
