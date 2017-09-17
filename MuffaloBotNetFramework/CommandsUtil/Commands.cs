using MuffaloBotNetFramework.DiscordComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MuffaloBotNetFramework.CommandsUtil
{
    static class Commands
    {
        public static string Description(string name)
        {
            string result = ThingDefDatabase.RequestField(name, "description");
            if (result.Length == 0)
            {
                return $"No description found for '{name}'";
            }
            return $"Description for '**{name}**': {result}";
        }

        public static string WikiSearch(string value, int limit = 10)
        {
            try
            {
                var results = WikiSearchImpl(value);
                if (results == null)
                {
                    return $"Could not find results for '{value}'.";
                }
                return $"Found {results.Count} results total (showing first {limit} if applicable): \n{string.Join("\n", results.Take(10))}";
            }
            catch (HttpRequestException)
            {
                return "Could not connect to the wiki server.";
            }
        }
        internal static ICollection<string> WikiSearchImpl(string query)
        {
            string t = HttpRequest.Request(string.Format(DiscordRoot.searchQuery, query));
            var matches = DiscordRoot.mediawikiapi.Matches(t);
            if (matches.Count == 0)
            {
                return null;
            }
            return EnumerateWikiEntries(matches);
        }
        internal static string[] EnumerateWikiEntries(MatchCollection matches)
        {
            var arr = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                arr[i] = string.Format(DiscordRoot.wikiPageFiller, matches[i].Groups[1].Value.Replace(' ', '_'));
            }
            return arr;
        }

        public static string SteamWorkshopSearch(string query, string steamkey)
        {
            try
            {
                var result = PublishedFileIdUtil.Query(query, steamkey);
                if (result.response.total == 0)
                {
                    return $"Could not find results for '{query}'";
                }
                var builder = new StringBuilder();
                string v = (result.response.total > 5) ? " (showing first 5):" : ":";
                builder.AppendLine($"Found {result.response.total} results total{v}");
                for (int i = 0; i < result.response.publishedfiledetails.Length; i++)
                {
                    var details = result.response.publishedfiledetails[i];
                    builder.AppendLine($"**{details.title}**: http://steamcommunity.com/sharedfiles/filedetails/?id={details.publishedFileId}"); //Fill in address here
                }
                return builder.ToString();
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return $"Could not find results for '{query}'";
            }
            catch (HttpRequestException)
            {
                return "Could not connect to the Steam api.";
            }
        }

        public static string GetField(string v)
        {
            var args = DiscordRoot.commandArgsSeparator.Match(v);
            var field = args.Groups[1].Value;
            var def = args.Groups[2].Value;
            var result = ThingDefDatabase.RequestField(def, field);
            if (result.Length == 0)
            {
                return $"Could not find field '{field}' in def '{def}'.";
            }
            return $"'**{field}**' of '**{def}**': {result}";
        }

        public static string GetStuffStats(string name)
        {
            var statFactors = ThingDefDatabase.RequestStatFactors(name);
            var statOffsets = ThingDefDatabase.RequestStatOffsets(name);
            if (statFactors == null && statOffsets == null)
            {
                return $"No stuff stats found for '{name}'";
            }
            var stringBuilder = new StringBuilder($"Stuff stats for '**{name}**':\n");
            if (statFactors != null)
            {
                stringBuilder.AppendLine("**Stat Factors**");
                foreach (var item in statFactors)
                {
                    stringBuilder.AppendFormat("**{0}**: x{1}\n", item.Key, item.Value);
                }
                stringBuilder.AppendLine();
            }
            if (statOffsets != null)
            {
                stringBuilder.AppendLine("**Stat Offsets**");
                foreach (var item in statOffsets)
                {
                    stringBuilder.AppendFormat("**{0}**: {1}\n", item.Key, item.Value);
                }
            }
            return stringBuilder.ToString();
        }

        public static string GetBaseStats(string name)
        {
            var result = ThingDefDatabase.RequestStatBases(name);
            if (result == null)
            {
                return $"No base stats found for '{name}'";
            }
            var stringBuilder = new StringBuilder($"Base stats for '**{name}**':\n");
            foreach (var item in result)
            {
                stringBuilder.AppendFormat("**{0}**: {1}\n", item.Key, item.Value);
            }
            return stringBuilder.ToString();
        }
    }
}
