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
        protected override void Setup(DiscordClient client)
        {
            client.SocketClosed += ExitApplication;
        }

        Task ExitApplication(DSharpPlus.EventArgs.SocketCloseEventArgs e)
        {
            MuffaloBotProgram.cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
