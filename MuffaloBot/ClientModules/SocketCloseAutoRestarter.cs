using DSharpPlus;
using MuffaloBot.DiscordComponent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBot.InternalModules
{
    public class SocketCloseAutoRestarter : BaseModule
    {
        DiscordClient client;
        protected override void Setup(DiscordClient client)
        {
            this.client = client;
            client.SocketClosed += ExitApplication;
        }

        Task ExitApplication(DSharpPlus.EventArgs.SocketCloseEventArgs e)
        {
            // Exit gracefully
            MuffaloBotProgram.createdInstances[client].cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
