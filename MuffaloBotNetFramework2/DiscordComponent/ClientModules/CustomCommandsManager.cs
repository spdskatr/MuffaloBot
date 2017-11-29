using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json.Linq;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using DSharpPlus.Entities;

namespace MuffaloBotNetFramework2.DiscordComponent.ClientModules
{
    class CustomCommandsManager : IClientModule
    {
        Dictionary<string, string> customCommands = new Dictionary<string, string>();
        public void BindToClient(DiscordClient client)
        {
            client.MessageCreated += TestTrigger;
        }

        async Task TestTrigger(MessageCreateEventArgs e)
        {
            if (customCommands.TryGetValue(e.Message.Content, out string response))
            {
                await e.Message.RespondAsync(response);
            }
        }

        public void InitializeFronJson(JObject jObject)
        {
            customCommands = JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject["customCommands"].ToString());
        }

        public IEnumerable<string> AllCustomCommands()
        {
            return customCommands.Keys;
        }
    }
}
