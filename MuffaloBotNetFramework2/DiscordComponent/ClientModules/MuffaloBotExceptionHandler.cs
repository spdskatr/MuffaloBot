using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MuffaloBotNetFramework2.DiscordComponent
{
    class MuffaloBotExceptionHandler : IClientModule
    {
        public void BindToClient(DiscordClient client)
        {
            client.ClientErrored += HandleClientError;
        }

        public void InitializeFronJson(JObject jObject)
        {
        }

        async Task HandleClientError(ClientErrorEventArgs e)
        {
            await Console.Out.WriteLineAsync(e.Exception.ToString());
        }
    }
}
