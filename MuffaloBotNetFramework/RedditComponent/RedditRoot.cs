using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffaloBotNetFramework.CommandsUtil;
using RedditSharp.Things;
using RedditSharp;
using System.Threading;

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
                case "xpath":
                    return Commands.XPath(operands, false);
                default:
                    break;
            }
            return null;
        }
        internal static async Task ReplyAsync(this Comment item, string message, bool addFooter = true)
        {
            if (addFooter)
            {
                message += "\n\n> I am MuffaloBot | Made by [spdskatr](https://www.reddit.com/user/spdskatr/) | [Source code](https://github.com/spdskatr/MuffaloBot) | [Commands reference](https://github.com/spdskatr/MuffaloBot/blob/master/CommandsReference.md)";
            }
            Retry:
            try
            {
                item.Reply(message);
            }
            // If sending too fast...
            catch (RateLimitException e)
            {
                await Console.Out.WriteLineAsync($"Reddit Component :: Experienced rate limit exception when replying. Resending in {e.TimeToReset.Milliseconds}ms... (This can occur frequently when 2 threads are fighting for internet access)");
                Thread.Sleep(e.TimeToReset.Milliseconds);
                goto Retry;
            }
        }
    }
}
