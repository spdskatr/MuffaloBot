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
                    return Commands.WikiSearch(operands).Replace("\n", "\n\n");
                case "wshopsearch":
                    if (Program.infoPackage.SteamTokenValid())
                    {
                        return Commands.SteamWorkshopSearch(operands, Program.infoPackage.stea).Replace("\n", "\n\n");
                    }
                    break;
                case "basestats":
                    return Commands.GetBaseStats(operands).Replace("\n", "\n\n");
                default:
                    break;
            }
            return null;
        }
        internal static async void ReplyAsync(this Comment item, string message)
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
                ReplyAsync(item, message);
            }
        }
    }
}
