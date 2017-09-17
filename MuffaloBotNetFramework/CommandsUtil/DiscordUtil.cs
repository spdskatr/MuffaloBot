using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework
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
            if (Program.sandbox)
            {
                return $":{name}:";
            }
            var emojis = Program.discord.Guilds[guildId].Emojis;
            for (int i = 0; i < emojis.Count; i++)
            {
                if (emojis[i].Name == name)
                {
                    return emojis[i].ToString();
                }
            }
            return $":{name}:";
        }
        internal static string CapFirst(this string str)
        {
            if (str.Length == 0) return str;
            if (str.Length > 1)
            {
                return str[0].ToString().ToUpper() + str.Substring(1);
            }
            return str.ToUpper();
        }
        static readonly Regex whitespaceRegex = new Regex("^\\s*$");
        internal static bool EmptyOrContainsOnlyWhitespace(this string str)
        {
            return (str.Length > 0) ? whitespaceRegex.Match(str).Success : true;
        }
    }
}
