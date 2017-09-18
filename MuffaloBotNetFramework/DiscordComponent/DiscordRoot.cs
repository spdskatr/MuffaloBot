using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using System.Net.Http;
using MuffaloBotNetFramework.CommandsUtil;

namespace MuffaloBotNetFramework.DiscordComponent
{
    static partial class DiscordRoot
    {
        // Processes a string that comes from a message.
        internal static _DiscordMessage ProcessString(string str, ulong guildID, int channelPos)
        {
            if (str.Length == 0) return null;
            switch (str)
            {
                case "!jdalt":
                    if (guildID == 214523379766525963u)
                    {
                        var jdalt = Program.discord.Guilds[214523379766525963u].Members.FirstOrDefault(m => m.Id == 221459835315683330);
                        return new DSharpPlus.DiscordEmbed()
                        {
                            Author = new DSharpPlus.DiscordEmbedAuthor()
                            {
                                Name = $"{jdalt.DisplayName}#{jdalt.Discriminator}",
                                IconUrl = jdalt.AvatarUrl
                            },
                            Description = "I'm expecting an Easter egg of me 9 AM tomorrow sharp"
                        };
                    }
                    break;
                case "!stan":
                    if (guildID == 214523379766525963u)
                    {
                        var stan = Program.discord.Guilds[214523379766525963u].Members.FirstOrDefault(m => m.Id == 144848386158362624);
                        return new DSharpPlus.DiscordEmbed()
                        {
                            Author = new DSharpPlus.DiscordEmbedAuthor()
                            {
                                Name = $"{stan.DisplayName}#{stan.Discriminator}",
                                IconUrl = stan.AvatarUrl
                            },
                            Description = "*I am not a command. Please try again later.*"
                        };
                    }
                    break;
                case "!spdskatrsroom":
                    return "You found ~==🥔spdskatr🥔==~'s room. Nobody's home...";
                case "!tempest":
                    return "[BOT ADOPTER]😎Tempest😎";
                case "!zirr":
                    return "[BOT ADOPTER]😎zirr😎";
                case "!wiki":
                    return "http://rimworldwiki.com";
                case "!muffalo":
                    return "😎🤖 Muffy is my name, RimWorld is my game 🤖😎";
                case "!apensnitzel":
                    return DiscordUtil.Emoji(guildID, "awoo");
                case "!wolfy":
                    return DiscordUtil.Emoji(guildID, "awoo");
                case "!tynan":
                    return "[DEV]~=😇Tynan Sylvester😇=~";
                case "!ison":
                    return "[DEV]~=😇Piotr Walczak😇=~";
                case "!zorba":
                    return "[DEV][THE PACK]~=😇Ben Rog-Wilhem😇=~";
                case "!qwertyuiopasdfghjklzxcvbnm":
                    return "mnbvcxzlkjhgfdsapoiuytrewq";
                case "!mnbvcxzlkjhgfdsapoiuytrewq":
                    return "stoppit";
                case "!stoppit":
                    return "please stop";
                case "!please stop":
                    return "What is the cube root of 59319?"; //39
                case "!What is the cube root of 59319?":
                    return "ok you win";
                case "!ok you win":
                    return "qwertyuiopasdfghjklzxcvbnm";
                case "!cortie":
                    return "Someone who spent 30 minutes straight to find this command.";
                case "!del C:\\Windows\\System32":
                    return "Successfully deleted 0 file(s)";
                case "!crash":
                    return "An unhandled exception occurred when executing instruction 'crash'. Log is at path '/dev/null'.";
                case "!rm -rf /":
                    return "Successfully deleted 0 file(s)";
                case "!kek":
                    return "kek";
                default:
                    break;
            }
            if (str[0] != '!') return null;
            if (Regex.Match(str, "!<?:awoo:").Success)
            {
                return "qwertyuiopasdfghjklzxcvbnm";
            }
            var breakdown = commandBreakdown.Match(str);
            switch (breakdown.Groups[1].Value)
            {
#if BETA
                case "say":
                    return breakdown.Groups[2].Value;
#endif
                case "xpathselect":
                    if (str.Length > 13)
                    {
                        return CoreDefDatabase.GetSummaryForNodeSelection(breakdown.Groups[2].Value);
                    }
                    return "Type `!usage xpathselect` for help with this command.";
                case "desc":
                    if (str.Length > 6)
                    {
                        return Commands.Description(breakdown.Groups[2].Value);
                    }
                    return "Type `!usage desc` for help with this command.";
                case "basestats":
                    if (str.Length > 11)
                    {
                        return Commands.GetBaseStats(breakdown.Groups[2].Value);
                    }
                    return "Type `!usage basestats` for help with this command.";
                case "stuffstats":
                    if (str.Length > 12)
                    {
                        return Commands.GetStuffStats(breakdown.Groups[2].Value);
                    }
                    return "Type `!usage stuffstats` for help with this command.";
                case "field":
                    if (str.Length > 7)
                    {
                        return Commands.GetField(breakdown.Groups[2].Value);
                    }
                    return "Type `!usage field` for help with this command.";
                case "usage":
                    switch (breakdown.Groups[2].Value)
                    {
                        case "basestats":
                            return "!basestats <item>\nExamples:\n!basestats steel\n!basestats wood\n!basestats ai persona core";
                        case "stuffstats":
                            return "!stuffstats <item>\nExamples:\n!stuffstats steel\n!stuffstats granite blocks\n!stuffstats alpaca wool";
                        case "usage":
                            return "!usage <command>\nExamples:\n!usage basestats\n!usage stuffstats\n!usage field\n(I think you already know how to use this)";
                        case "field":
                            return "!field <field_name> <item>\nExamples:\n!field stacklimit chemfuel\n!field category muffalo wool";
                        case "desc":
                            return "!desc <item>\nExamples:\n!desc steel\n!desc fueled smithy\n!desc corn\n!desc rice plant";
                        case "wikisearch":
                            return "!wikisearch <search term>\nExamples:\n!wikisearch wood\n!wikisearch potato plant\n!wikisearch raider";
                        case "wshopsearch":
                            return "!wshopsearch <search term>\nExamples:\n!wshopsearch zombieland\n!wshopsearch spdskatr\n!wshopsearch colony manager";
                        case "xpathselect":
                            return "!xpathselect <XPath path>\nExamples:\n!xpathselect */ThingDef[defName='WoodLog']/statBases/";
                        default:
                            break;
                    }
                    return "Please specify a command name.";
                case "wikisearch":
                    if (str.Length > 12)
                    {
                        return Commands.WikiSearch(breakdown.Groups[2].Value);
                    }
                    return "Type `!usage wikisearch` for help with this command.";
                case "wshopsearch":
                    if (Program.infoPackage.SteamTokenValid() && str.Length > 16)
                    {
                        return Commands.SteamWorkshopSearch(breakdown.Groups[2].Value, Program.infoPackage.stea);
                    }
                    return "Type `!usage wshopsearch` for help with this command.";
                case "sudo":
                    return "sudo do it yourself";
                default:
                    break;
            }
            return null;
        }


        //Why is this even here
        private static string RandomUnicodeCharacters(int length = 8)
        {
            char[] str = new char[length];
            //0x0021, 0x007e
            for (int i = 0; i < length; i++)
            {
                if (random.NextDouble() < 0.5d)
                {
                    var val = random.Next(0x0021, 0x007e);
                    str[i] = (char)val;

                }
                else
                {
                    var val = random.Next(0x00a1, 0x058f);
                    str[i] = (char)val;
                }
            }
            return new string(str);
        }
    }
}
