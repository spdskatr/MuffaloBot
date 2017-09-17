using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace MuffaloBotNetFramework.RedditComponent
{
    class RedditBase
    {
        class TokenRetrievalResponse
        {
#pragma warning disable CS0649
            public string access_token;
#pragma warning restore CS0649
        }
        static Regex commandRegex = new Regex("\\[\\s*([a-zA-Z0-9]+)\\s*([a-zA-Z0-9\\s]*)\\s*\\]");
        Reddit reddit;
        Subreddit sub;
        public RedditBase()
        {
        }
        public async void StartAsync(string refreshToken, string auth)
        {
            // Get token
            WebRequest req = await GetRequest(refreshToken, auth);
            WebResponse response = await req.GetResponseAsync();

            Stream resStream = response.GetResponseStream();
            string res = await new StreamReader(resStream).ReadToEndAsync();
            resStream.Close();
            
            var tk = JsonConvert.DeserializeObject<TokenRetrievalResponse>(res).access_token;
            Console.WriteLine("Reddit Component :: Got access token: " + tk);
            reddit = new Reddit(tk);
            Console.WriteLine("Reddit Component :: Starting...");
            sub = await reddit.GetSubredditAsync("bottesting");
            await Console.Out.WriteLineAsync("Reddit Component :: Connected successfully");
            await StartCommentStream();
            await Console.Error.WriteLineAsync("Reddit Component :: Completed Reddit bot super-task... Which isn't meant to happen");
        }

        private static async Task<WebRequest> GetRequest(string refreshToken, string auth)
        {
            WebRequest request = WebRequest.Create("https://www.reddit.com/api/v1/access_token");
            request.Method = "POST";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(auth));
            request.ContentType = "application/x-www-form-urlencoded";
            Stream stream = await request.GetRequestStreamAsync();
            byte[] bytes = Encoding.UTF8.GetBytes("grant_type=refresh_token&refresh_token=" + refreshToken);
            stream.Write(bytes, 0, bytes.Length);
            return request;
        }

        private async Task StartCommentStream()
        {
            foreach (var item in sub.Comments.GetListingStream())
            {
                string str = item.Body;
                // If I have downvoted an item, I have read it before
                if ((!item.Liked) ?? false) //Note: Comments property always return a List of 0. Do not use
                {
                    await Console.Out.WriteLineAsync("Detected comment already replied to.");
                    continue;
                }
                MatchCollection matches = commandRegex.Matches(str);
                int count = Math.Min(matches.Count, 3); //Max of 3 per post
                string[] responses = new string[count];
                for (int i = 0; i < count; i++)
                {
                    Match match = matches[i];
                    responses[i] = RedditRoot.ProcessCommand(match.Groups[1].Value, match.Groups[2].Value);
                }
                string reply = string.Join("\n---\n", from r in responses where r != null select r);
                if (reply != null && reply.Length > 0)
                {
                    await Console.Out.WriteLineAsync("Replied."); //DEBUG
                    //item.ReplyAsync(reply);
                    item.Downvote();
                }
            }
        }
    }
}
