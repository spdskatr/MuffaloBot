using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MuffaloBot.Modules
{
    public class JsonDataModule : BaseModule
    {
        public JObject data;
        protected override void Setup(DiscordClient client)
        {
            Client = client;
            ReloadDataAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public async Task ReloadDataAsync()
        {
            HttpClient http = new HttpClient();
            string result = await http.GetStringAsync("https://raw.githubusercontent.com/spdskatr/MuffaloBot/master/MuffaloBot/Data/data.json").ConfigureAwait(false);
            JObject jObject = JObject.Parse(result);
            data = jObject;
        }
    }
}
