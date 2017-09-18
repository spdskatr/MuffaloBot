using DSharpPlus;
using MuffaloBotNetFramework.DiscordComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework.CommandsUtil
{
    class _DiscordMessage
    {
        internal string message = "";
        internal DiscordEmbed embed;
        public static implicit operator _DiscordMessage(string str)
        {
            return new _DiscordMessage { message = str };
        }
        public static implicit operator _DiscordMessage(DiscordEmbed embed)
        {
            return new _DiscordMessage { embed = embed };
        }
    }
    static class DiscordUtil
    {
        struct EmbedEntry
        {
            internal string label;
            internal string desc;
            //Uses the \t separator to separate title and content. Use the \\t escape sequence for the raw \t character. 
            public static implicit operator EmbedEntry(string str)
            {
                var entry = new EmbedEntry();
                var strs = str.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                entry.label = strs[0].Replace("\\t", "\t");
                entry.desc = strs[1].Replace("\\t", "\t");
                return entry;
            }
        }
        internal static DiscordEmbed CreateEmbed(string title = "", int color = 0x00ffff, params string[] entries)
        {
            var embed = new DiscordEmbed()
            {
                Title = title
            };
            for (int i = 0; i < entries.Length; i++)
            {
                EmbedEntry entry = entries[i];
                embed.Fields.Add(new DiscordEmbedField() { Name = entry.label, Value = entry.desc });
            }
            return embed;
        }
        internal static string Emoji(ulong guildId, string name)
        {
            if (DiscordBase.sandbox)
            {
                return $":{name}:";
            }
            var emojis = Program.dBase.discord.Guilds[guildId].Emojis;
            for (int i = 0; i < emojis.Count; i++)
            {
                if (emojis[i].Name == name)
                {
                    return emojis[i].ToString();
                }
            }
            return $":{name}:";
        }
    }
}
