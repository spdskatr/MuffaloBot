using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MuffaloBotNetFramework2.DiscordComponent;
using MuffaloBotNetFramework2.InternalModules;
using MuffaloBotNetFramework2;

namespace MuffaloBotNetFramework2.CommandsModules
{
    [MuffaloBotCommandsModule]
    class ClientModuleCommands
    {
        [Command("quote")]
        public async Task QuoteWithOption(CommandContext ctx, int index = 0)
        {
            string[] quotes = MuffaloBot.GetModule<QuoteManager>().quotes;
            Random random = new Random();
            switch (index)
            {
                case 0:
                    await ctx.RespondAsync(quotes[random.Next(0, quotes.Length)]);
                    break;
                default:
                    if (index == quotes.Length + 1)
                    {
                        await ctx.RespondAsync($"Welcome to the secret muffalo internet. Please proceed to the main page: `!quote 9.75`");
                        return;
                    }
                    if (index > quotes.Length)
                    {
                        await ctx.RespondAsync($"Tried to get quote number {index} when there are only {quotes.Length} quotes.");
                        return;
                    }
                    await ctx.RespondAsync(quotes[index - 1]);
                    break;
            }
        }

        [Command("mbhelp")]
        public async Task MuffaloBotHelp(CommandContext ctx, string command = null)
        {
            if (command == null)
            {
                await ctx.RespondAsync(embed: MuffaloBot.GetModule<HelpProvider>().GeneralHelp());
            }
            else
            {
                await ctx.RespondAsync(embed: MuffaloBot.GetModule<HelpProvider>().EmbedFromHelpCommand(command));
            }
        }
    }
}
