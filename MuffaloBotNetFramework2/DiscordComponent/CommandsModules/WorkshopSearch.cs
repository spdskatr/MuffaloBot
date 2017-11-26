using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace MuffaloBotNetFramework2.DiscordComponent.CommandsModules
{
    class WorkshopSearch
    {
        const string query = "https://api.steampowered.com/IPublishedFileService/QueryFiles/v1/?key={0}&format=json&numperpage={1}&appid=294100&match_all_tags=1&search_text={2}&return_short_description=1&return_metadata=1&query_type={3}";
        SteamSearchResult ResultFromString(string str)
        {
            return JsonConvert.DeserializeObject<SteamSearchResult>(str);
        }
        SteamSearchResult Query(string content, string key, byte resultsCap = 5, EPublishedFileQueryType queryType = EPublishedFileQueryType.Relevance)
        {
            if (resultsCap > 20) resultsCap = 20;
            string request = string.Format(query, key, resultsCap, content, ((uint)queryType).ToString());
            HttpWebRequest req = WebRequest.CreateHttp(request);
            StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream());
            return ResultFromString(reader.ReadToEnd());
        }
        [Command("wshopsearch")]
        public async Task Search(CommandContext ctx, string query)
        {
            SteamSearchResult result = Query(query, MuffaloBot.steamApiKey);
            if (result.response.total == 0)
            {
                await ctx.RespondAsync("No results.");
            }
            else
            {
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
                embedBuilder.WithColor(DiscordColor.DarkBlue);
                embedBuilder.WithTitle("Steam Workshop search results");
                embedBuilder.WithDescription("Total results: " + result.response.total);
                for (int i = 0; i < result.response.publishedfiledetails.Length; i++)
                {
                    PublishedFile publishedFile = result.response.publishedfiledetails[i];
                    embedBuilder.AddField(publishedFile.title, $"**Views**: {publishedFile.views}\n**Subs**: {publishedFile.subscriptions}\n**Favs**: {publishedFile.favorited}\n**ID**: {publishedFile.publishedFileId}\n[Link](http://steamcommunity.com/sharedfiles/filedetails/?id={publishedFile.publishedFileId})", true);
                }
                await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }
    }

    /// <summary>
    /// This thing was made after countless hours of trial and error.
    /// </summary>
    public enum EPublishedFileQueryType : uint
    {
        TopRatedAllTime = 0,
        MostRecent = 1,
        Relevance = 3,
        Unknown6 = 6,
        MostSubscribed = 9
    }
#pragma warning disable CS0649
    class Response
    {
        public uint total;
        public PublishedFile[] publishedfiledetails;
    }
    class SteamSearchResult
    {
        public Response response;
    }
    class PublishedFile
    {
        public uint result;
        public uint views;
        public uint subscriptions;
        public uint favorited;
        public string publishedFileId;
        public string title;
        public string preview_url;
    }
#pragma warning restore CS0649
}
