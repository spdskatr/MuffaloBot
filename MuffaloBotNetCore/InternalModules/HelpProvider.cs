using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using MuffaloBotNetFramework2.DiscordComponent;
using MuffaloBotNetFramework2;

namespace MuffaloBotNetFramework2.InternalModules
{
    class HelpProvider : IInternalModule
    {
        public Dictionary<string, HelpEntry> dictionary;
        public class HelpEntry
        {
            public string usage = "(no usage)";
            public string description = "(no description)";
            public Dictionary<string, string> parameters = new Dictionary<string, string>();
            public string[] aliases = new string[0];
            public float cooldown = 0f;
            public string[] examples = new string[0];
        }
        public void BindToClient(DiscordClient client)
        {
        }
        public void InitializeFronJson(JObject jObject)
        {
            dictionary = jObject["help"].ToObject<Dictionary<string, HelpEntry>>();
        }
        public DiscordEmbed EmbedFromHelpCommand(string command)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithColor(DiscordColor.Green);
            if (dictionary.TryGetValue(command, out HelpEntry entry))
            {
                embedBuilder.WithTitle("Help for " + command);
                embedBuilder.WithDescription($"**Usage:** `{entry.usage}`\n**Description:** {entry.description}");
                if (entry.parameters != null)
                {
                    foreach (string key in entry.parameters.Keys)
                    {
                        embedBuilder.AddField($"Parameter `{key}`", entry.parameters[key], true);
                    }
                }
                if (entry.examples != null && entry.examples.Length > 0)
                {
                    embedBuilder.AddField("Examples", string.Join("\n", entry.examples.Select(s => $"`{s}`")));
                }
                if (entry.aliases != null && entry.aliases.Length > 0)
                {
                    embedBuilder.AddField("Aliases", string.Join(", ", entry.aliases.Select(s => $"`{s}`")));
                    if (entry.cooldown > 0f)
                    {
                        embedBuilder.AddField("Cooldown", $"{entry.cooldown}s");
                    }
                }
            }
            else
            {
                embedBuilder.WithTitle(command == "404" ? "404" : "No help found");
                embedBuilder.WithDescription(command == "404" ? "404 404 404 404 404 404 404 404 404 404 404 404 404" : "Could not find help for your query.");
            }
            return embedBuilder.Build();
        }
        public DiscordEmbed GeneralHelp()
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle("MuffaloBot help");
            embedBuilder.WithColor(DiscordColor.Green);
            embedBuilder.WithDescription("This embed shows all documented commands. For more commands, type `!mbhelp <command>`");
            embedBuilder.AddField("All commands", string.Join(", ", dictionary.Keys.Select(s => string.Format("`{0}`", dictionary[s].usage))));
            embedBuilder.AddField("Custom commands (no help)", string.Join(", ", MuffaloBot.GetModule<CustomCommandsManager>().AllCustomCommands().Select(s => $"`{s}`")));
            
            return embedBuilder.Build();
        }
    }
}
