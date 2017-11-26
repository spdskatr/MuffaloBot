using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json.Linq;

namespace MuffaloBotNetFramework2.DiscordComponent.ClientModules
{
    class QuoteManager : IClientModule
    {
        public string[] quotes;
        public void BindToClient(DiscordClient client)
        {
        }

        public void InitializeFronJson(JObject jObject)
        {
            quotes = jObject["quotes"].Values<string>().ToArray();
        }
    }
}
