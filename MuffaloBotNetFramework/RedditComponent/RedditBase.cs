using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using MuffaloBotNetFramework.CommandsUtil;

namespace MuffaloBotNetFramework.RedditComponent
{
    class RedditBase
    {
        class TokenRetrievalResponse
        {
#pragma warning disable CS0649
            public string error;
            public string access_token;
#pragma warning restore CS0649
        }
        internal static Regex commandRegex = new Regex("{{\\s*([^{}\\s]+)\\s*([^{}]*)\\s*}}");
        static Regex muffaloBotIgnoreRegex = new Regex("{{([Mm]uffalo[Bb]ot[Ii]gnore|mbignore|mbi)}}");
        Reddit reddit;
        Subreddit sub;
        public RedditBase()
        {
        }
        public async void StartAsync(string refreshToken, string auth, string targetSubreddit)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    var result = MainLoopAsync(refreshToken, auth, targetSubreddit).Wait(2700_000);
                    if (result)
                    {
                        Console.Error.WriteLine("[ERR] Reddit component unexpectedly terminated. Restarting...\n");
                    }
                    else
                    {
                        Console.WriteLine("Reddit Component :: 45 minute task thread has ended, restarting thread and getting new token...");
                    }
                }
            });
        }

        async Task MainLoopAsync(string refreshToken, string auth, string targetSubreddit)
        {
            A:
            try
            {
                // Get token
                WebRequest req = await GetRequest(refreshToken, auth);
                WebResponse response = await req.GetResponseAsync();

                Stream resStream = response.GetResponseStream();
                string res = await new StreamReader(resStream).ReadToEndAsync();
                resStream.Close();

                var tokenRetrievalResponse = JsonConvert.DeserializeObject<TokenRetrievalResponse>(res);
                if (tokenRetrievalResponse.error != null && tokenRetrievalResponse.error.Length > 0)
                {
                    await Console.Error.WriteLineAsync("Reddit Component :: Error occurred when retrieving token. Response:\n" + res);
                    return;
                }

                var tk = tokenRetrievalResponse.access_token;
                await Console.Out.WriteLineAsync("Reddit Component :: Got access token: " + tk);
                reddit = new Reddit(tk);
                await Console.Out.WriteLineAsync("Reddit Component :: Connecting to subreddit " + targetSubreddit);
                sub = await reddit.GetSubredditAsync(targetSubreddit);
                await Console.Out.WriteLineAsync("Reddit Component :: Connected successfully");
                await StartCommentStream();
                // Code isn't meant to get here
                await Console.Error.WriteLineAsync("Reddit Component :: Completed Reddit bot super-task... Which isn't meant to happen");
            }
            catch (WebException)
            {
                await Console.Error.WriteLineAsync($"Reddit Component :: A web error occurred when running MuffaloBot, probably due to token expiring. Restarting component.");
                goto A;
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync("[ERR] Exception in Reddit component main loop: " + e);
            }
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
                if ((!item.Liked) ?? false | muffaloBotIgnoreRegex.IsMatch(str)) //Note: Comments property always return a List of 0. Do not use
                {
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
                string reply = string.Join("\n\n---\n\n", from r in responses where r != null select r);
                if (reply != null && reply.Length > 0)
                {
                    await Console.Out.WriteLineAsync("Reddit Component :: New reply: " + item.Id);
                    await item.ReplyAsync(reply);
                    item.Downvote();
                }
            }
        }
    }
}
