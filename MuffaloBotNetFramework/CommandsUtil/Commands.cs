using MuffaloBotNetFramework.DiscordComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.XPath;

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

        public static string WikiSearch(string value, bool reddit, int limit = 10)
        {
            try
            {
                var results = WikiSearchImpl(value);
                if (results == null)
                {
                    return $"Could not find results for '{value}'.";
                }
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Found {results.Count} results total (showing first {limit} if applicable):  \n");
                if (reddit)
                {
                    stringBuilder.AppendLine($"| Title | Link |\n|-|-|");
                }
                foreach (var res in results.Take(10))
                {
                    if (reddit)
                    {
                        stringBuilder.AppendLine($"| **{res.Key}** | {res.Value} |");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"**{res.Key}**: {res.Value}  ");
                    }
                }
                return stringBuilder.ToString();
            }
            catch (HttpRequestException)
            {
                return "Could not connect to the wiki server.";
            }
        }
        internal static ICollection<KeyValuePair<string, string>> WikiSearchImpl(string query)
        {
            string t = HttpRequest.Request(string.Format(DiscordRoot.searchQuery, query));
            var matches = DiscordRoot.mediawikiapi.Matches(t);
            if (matches.Count == 0)
            {
                return null;
            }
            return EnumerateWikiEntries(matches);
        }
        internal static KeyValuePair<string, string>[] EnumerateWikiEntries(MatchCollection matches)
        {
            var arr = new KeyValuePair<string, string>[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                string val = matches[i].Groups[1].Value;
                arr[i] = new KeyValuePair<string, string>(val, string.Format(DiscordRoot.wikiPageFiller, val.Replace(' ', '_')));
            }
            return arr;
        }

        public static string SteamWorkshopSearch(string query, string steamkey, bool reddit, EPublishedFileQueryType queryType = EPublishedFileQueryType.Relevance)
        {
            try
            {
                var result = PublishedFileIdUtil.Query(query, steamkey, queryType:queryType);
                if (result.response.total == 0)
                {
                    return $"Could not find results for '{query}'";
                }
                var builder = new StringBuilder();
                builder.AppendLine($"Searched {result.response.total} results total (showing first 5 if applicable):  \n\n");
                if (reddit)
                {
                    builder.AppendLine("| Title | Views, Subscriptions, Favorites | Link |\n| --- | --- | --- |");
                }
                for (int i = 0; i < result.response.publishedfiledetails.Length; i++)
                {
                    var details = result.response.publishedfiledetails[i];
                    if (reddit)
                    {
                        builder.AppendLine($"| **{details.title}** | {details.views}, {details.subscriptions}, {details.favorited} | http://steamcommunity.com/sharedfiles/filedetails/?id={details.publishedFileId} |");
                    }
                    else
                    {
                        builder.AppendLine($"**{details.title}** ( {details.views} views | {details.subscriptions} subs | {details.favorited} favs ): http://steamcommunity.com/sharedfiles/filedetails/?id={details.publishedFileId}");
                    }
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

        public static string GetStuffStats(string name, bool reddit)
        {
            var statFactors = ThingDefDatabase.RequestStatFactors(name);
            var statOffsets = ThingDefDatabase.RequestStatOffsets(name);
            if (statFactors == null && statOffsets == null)
            {
                return $"No stuff stats found for '{name}'";
            }
            var stringBuilder = new StringBuilder($"Stuff stats for '**{name}**':  \n");
            if (statFactors != null)
            {
                stringBuilder.AppendLine("**Stat Factors**  \n");
                if (reddit)
                {
                    stringBuilder.AppendLine("| Name | Value |\n|-|-|");
                }
                foreach (var item in statFactors)
                {
                    if (reddit)
                    {
                        stringBuilder.AppendLine($"| {item.Key} | x{item.Value} |");
                    }
                    else
                    {
                        stringBuilder.AppendFormat("**{0}**: x{1}\n", item.Key, item.Value);
                    }
                }
                stringBuilder.AppendLine();
            }
            if (statOffsets != null)
            {
                stringBuilder.AppendLine("**Stat Offsets**  \n");
                if (reddit)
                {
                    stringBuilder.AppendLine("| Name | Value |\n|-|-|");
                }
                foreach (var item in statOffsets)
                {
                    if (reddit)
                    {
                        stringBuilder.AppendLine($"| {item.Key} | {item.Value} |");
                    }
                    else
                    {
                        stringBuilder.AppendFormat("**{0}**: {1}\n", item.Key, item.Value);
                    }
                }
            }
            return stringBuilder.ToString();
        }

        public static string GetBaseStats(string name, bool reddit)
        {
            var result = ThingDefDatabase.RequestStatBases(name);
            if (result == null)
            {
                return $"No base stats found for '{name}'";
            }
            var stringBuilder = new StringBuilder($"Base stats for '**{name}**':\n\n");
            if (reddit)
            {
                stringBuilder.AppendLine("| Name | Value |\n|-|-|");
            }
            foreach (var item in result)
            {
                if (reddit)
                {
                    stringBuilder.AppendLine($"| {item.Key} | {item.Value} |");
                }
                else
                {
                    stringBuilder.AppendFormat("**{0}**: {1}\n", item.Key, item.Value);
                }
            }
            return stringBuilder.ToString();
        }

        // Syntax highlighting is not supported by Reddit.
        public static string XPath(string path, bool syntaxHighlighting = true)
        {
            try
            {
                return CoreDefDatabase.GetSummaryForNodeSelection(path, syntaxHighlighting);
            }
            catch (XPathException)
            {
                return "**Invalid XPath!**";
            }
        }
    }
}
