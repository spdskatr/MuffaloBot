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
                    return Commands.WikiSearch(operands, true);
                case "wshopsearch":
                    if (Program.infoPackage.SteamTokenValid())
                    {
                        return Commands.SteamWorkshopSearch(operands, Program.infoPackage.stea, true);
                    }
                    break;
                case "basestats":
                    return Commands.GetBaseStats(operands, true);
                case "stuffstats":
                    return Commands.GetStuffStats(operands, true);
                case "xpath":
                    return Commands.XPath(operands, false).FormatNewLinesForReddit();
                case "field":
                    return Commands.GetField(operands);
                default:
                    break;
            }
            return null;
        }
        internal static async Task ReplyAsync(this Comment item, string message, bool addFooter = true)
        {
            if (addFooter)
            {
                message += "\n\n| I am MuffaloBot | Made by [spdskatr](https://www.reddit.com/user/spdskatr/) | [Source code](https://github.com/spdskatr/MuffaloBot) | [Commands reference](https://github.com/spdskatr/MuffaloBot/blob/master/CommandsReference.md) |\n|-|-|-|-|";
            }
            Retry:
            try
            {
                item.Reply(message);
            }
            // If sending too fast...
            catch (RateLimitException e)
            {
                await Console.Out.WriteLineAsync($"Reddit Component :: Experienced rate limit exception when replying. Trying again in {e.TimeToReset.TotalMilliseconds}ms.(This can occur frequently when 2 threads are fighting for internet access)");
                await Task.Delay((int)Math.Ceiling(e.TimeToReset.TotalMilliseconds));
                goto Retry;
            }
        }
    }
}
