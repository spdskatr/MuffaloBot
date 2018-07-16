using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBot.Commands
{
    public class SteanWorkshopCommands
    {
        const string query = "https://api.steampowered.com/IPublishedFileService/QueryFiles/v1/?key={0}&format=json&numperpage={1}&appid=294100&match_all_tags=1&search_text={2}&return_short_description=1&return_metadata=1&query_type={3}";
        JObject Query(string content, string key, byte resultsCap = 5)
        {
            if (resultsCap > 20) resultsCap = 20;
            string request = string.Format(query, key, resultsCap, content, 3.ToString());
            HttpWebRequest req = WebRequest.CreateHttp(request);
            StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream());
            return JObject.Parse(reader.ReadToEnd());
        }
        [Command("wshopsearch"), Description("Searches the steam workshop for content.")]
        public async Task Search(CommandContext ctx, [RemainingText, Description("The search query.")] string query)
        {
            JObject result = Query(query, AuthResources.SteamApiKey, 5);
            if (result["response"]["total"].Value<int>() == 0)
            {
                await ctx.RespondAsync("No results.");
            }
            else
            {
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
                embedBuilder.WithColor(DiscordColor.DarkBlue);
                embedBuilder.WithTitle($"Results for '{query}'");
                embedBuilder.WithDescription("Total results: " + result["response"]["total"]);
                foreach (JToken item in result["response"]["publishedfiledetails"])
                {
                    embedBuilder.AddField(item["title"].ToString(),
                        $"**Views**: {item["views"]}\n" +
                        $"**Subs**: {item["subscriptions"]}\n" +
                        $"**Favs**: {item["favorited"]}\n**ID**: {item["publishedfileid"]}\n" +
                        $"[Link](http://steamcommunity.com/sharedfiles/filedetails/?id={item["publishedfileid"]})",
                        true);
                }
                await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }
    }
}
