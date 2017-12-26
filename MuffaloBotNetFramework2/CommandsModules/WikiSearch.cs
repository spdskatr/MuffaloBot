using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using MuffaloBotNetFramework2.DiscordComponent;

namespace MuffaloBotNetFramework2.CommandsModules
{
    [MuffaloBotCommandsModule]
    class WikiSearch
    {
        const string queryAddress = "http://rimworldwiki.com/api.php?action=query&list=search&format=json&srlimit=5&srprop=size|wordcount|timestamp&srsearch={0}";

        [Command("wikisearch")]
        public async Task Search(CommandContext ctx, string query)
        {
            WebClient webClient = new WebClient();
            string result = await webClient.DownloadStringTaskAsync(string.Format(queryAddress, query));
            JObject jObject = JObject.Parse(result);
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithTitle($"Results for '{query}'");
            builder.WithColor(DiscordColor.Azure);
            foreach (JToken token in jObject["query"]["search"])
            {
                builder.AddField(token["title"].ToString(),
                    $"**Info**\n{token["size"]} bytes\n" +
                    $"{token["wordcount"]} words\n" +
                    $"Last Edited UTC {DateTime.Parse(token["timestamp"].ToString())}\n" +
                    $"[Link](rimworldwiki.com/wiki/{token["title"].ToString().Replace(" ", "%20")})", true);
            }
            await ctx.RespondAsync(embed: builder.Build());
        }
    }
}
