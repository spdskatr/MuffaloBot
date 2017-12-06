using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json.Linq;
using MuffaloBotNetFramework2.DiscordComponent;

namespace MuffaloBotCoreLib.InternalModules
{
    class QuoteManager : IInternalModule
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
