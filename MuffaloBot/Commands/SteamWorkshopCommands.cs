using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MuffaloBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MuffaloBot.Commands
{
    public class SteamWorkshopCommands
    {
        private const string baseUrl = "https://api.steampowered.com/";
        private const string relativeModPageEndpoint = "ISteamRemoteStorage/GetPublishedFileDetails/v1/";
        private const string relativeUserEndpoint = "ISteamUser/GetPlayerSummaries/v2/?key={0}&steamids={1}";
        private HttpClient httpClient = new HttpClient();
        private JsonSerializer jsonSerializer = new JsonSerializer();

        [Command("wshopmod"), Description("Displays info about a workshop mod.")]
        public async Task Preview(CommandContext ctx, [RemainingText, Description("The published file id or steam url of the mod")] string publishedFileId)
        {
            // Show Muffy typing while we wait.
            await ctx.TriggerTypingAsync();

            // Check if input is a steam url. If it is - extract published file id from it.
            if (Uri.TryCreate(publishedFileId, UriKind.Absolute, out Uri uri))
            {
                var queries = HttpUtility.ParseQueryString(uri.Query);
                publishedFileId = queries.Get("id");
            }

            if (publishedFileId is null || publishedFileId == string.Empty)
            {
                await ctx.RespondAsync("No results.");
                return;
            }

            // We only want the one and first workshop item.
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("itemcount", "1"),
                new KeyValuePair<string, string>("publishedfileids[0]", publishedFileId)
            });

            // The mod request.
            PublishedFileInfoModel.Publishedfiledetail mod;
            var modEndpointUrl = $"{baseUrl}{relativeModPageEndpoint}";
            using (var modResult = await this.httpClient.PostAsync(modEndpointUrl, content))
            using (var modResponse = await modResult.Content.ReadAsStreamAsync())
            using (var modStreamReader = new StreamReader(modResponse))
            using (var modJsonReader = new JsonTextReader(modStreamReader))
            {
                var publishedFileIdInfo = this.jsonSerializer.Deserialize<PublishedFileInfoModel>(modJsonReader);
                mod = publishedFileIdInfo?.response?.PublishedFileDetails?.First();
            }

            // Checks if the workshop item exists at all, and belongs to the RimWorld domain.
            if (mod?.Creator_App_Id != 294100)
            {
                await ctx.RespondAsync("No results.");
                return;
            }

            // We want mod author as well, which we only get the id of above, so we make another request to get author info.
            List<Player> users;
            var userEndpointUrl = string.Format($"{baseUrl}{relativeUserEndpoint}", AuthResources.SteamApiKey, mod.Creator);
            using (var userResult = await this.httpClient.GetAsync(userEndpointUrl))
            using (var userResponse = await userResult.Content.ReadAsStreamAsync())
            using (var userStreamReader = new StreamReader(userResponse))
            using (var userJsonReader = new JsonTextReader(userStreamReader))
            {
                var userModel = this.jsonSerializer.Deserialize<UserModel>(userJsonReader);
                users = userModel.response.players;
            }

            if (users.Count == 0)
            {
                await ctx.RespondAsync("No results.");
                return;
            }

            var user = users.First();

            // Limiting the description to x nr of characters.
            var description = string.Concat(mod.Description.Take(150));

            // Remove all steam markup from description.
            var cleanDescription = $"{Regex.Replace(description, @"(\[\S*\])", string.Empty)}...";
            // Remove all links.
            cleanDescription = Regex.Replace(cleanDescription, @"(\[url=\S*\[/url\])", string.Empty);
            var versions = string.Join(", ", mod.Tags.Where(x => x.Tag != "Scenario" && x.Tag != "Mod").Select(x => x.Tag));
            var lastUpdated = mod.Time_Updated;

            var modEmbed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Gold)
                .WithTitle(mod.Title)
                .WithUrl($"http://steamcommunity.com/sharedfiles/filedetails/?id={mod.PublishedFileId}")
                .AddField("Author", user.PersonaName, true)
                .AddField("Versions", versions, true)
                .AddField("Last Update", DateTimeOffset.FromUnixTimeSeconds(lastUpdated).Date.ToShortDateString(), true)
                .AddField("Description", cleanDescription)
                .WithThumbnailUrl(user.avatarmedium)
                .WithImageUrl(mod.Preview_Url)
                .Build();

            await ctx.RespondAsync(embed: modEmbed);
        }

        private const string query = baseUrl + "IPublishedFileService/QueryFiles/v1/?key={0}&format=json&numperpage={1}&appid=294100&match_all_tags=1&search_text={2}&return_short_description=1&return_metadata=1&query_type={3}";

        private JObject Query(string content, string key, byte resultsCap = 5)
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
            try
            {
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
            catch (Exception e)
            {
                await ctx.RespondAsync("Oops! The Steam API doesn\'t seem to want to cooperate right now. Try again later :(");
                if (!(e is ArgumentNullException || e is NullReferenceException)) throw;
            }
        }
    }
}