using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MuffaloBotNetFramework.CommandsUtil
{
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
    static class PublishedFileIdUtil
    {
        static SteamSearchResult ResultFromString(string str)
        {
            return JsonConvert.DeserializeObject<SteamSearchResult>(str);
        }
        internal static SteamSearchResult Query(string content, string key, byte resultsCap = 5, EPublishedFileQueryType queryType = EPublishedFileQueryType.Relevance)
        {
            if (resultsCap > 20) resultsCap = 20;
            return ResultFromString(HttpRequest.Request(string.Format(query, key, resultsCap, content, ((uint)queryType).ToString())));
        }
        const string query = "https://api.steampowered.com/IPublishedFileService/QueryFiles/v1/?key={0}&format=json&numperpage={1}&appid=294100&match_all_tags=1&search_text={2}&return_short_description=1&return_metadata=1&query_type={3}";
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
