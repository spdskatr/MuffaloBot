using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json.Linq;
using MuffaloBotNetFramework2.DiscordComponent;

namespace MuffaloBotNetFramework2.InternalModules
{
    class QuoteManager : IInternalModule
    {
        public const string prefix = "!quote ";
        public string[] quotes;
        public void BindToClient(DiscordClient client)
        {
            client.MessageCreated += SecretMuffaloInternet;
        }

        private async Task SecretMuffaloInternet(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (e.Message.Content.StartsWith(prefix))
            {
                string sub = e.Message.Content.Substring(prefix.Length);
                switch (sub)
                {
                    case "9.75":
                        await e.Message.RespondAsync(@"```md
<= =SECRET MUFFALO INTERNET= =>
       __**Homepage**__
   In Devilstrand, We Devour
#-----------------------------#
           Site Map
Homepage      |            9.75
???           |             ???
???           |             ???
???           |             ???
#-----------------------------#
```");
                        break;
                    case "???":
                        await e.Message.RespondAsync(@"```md
<= =SECRET MUFFALO INTERNET= =>
     __**Facepalm page**__
   In Devilstrand, We Devour
#-----------------------------#
           Message
 No, you're not meant to type
 '!quote ???', ??? is just a
   symbol to tell you that
  information is unknown or
         forbidden. 🤦

 And that is exactly why this
 is a secret page. A command
   hidden in plain sight. 🤦
#-----------------------------#
           Site Map
Homepage      |            9.75
Facepalm page |             ???
???           |             ???
???           |             ???
#-----------------------------#
```");
                        break;
                }
            }
        }

        public void InitializeFronJson(JObject jObject)
        {
            quotes = jObject["quotes"].Values<string>().ToArray();
        }
    }
}
