using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MuffaloBotNetFramework2.DiscordComponent;
using MuffaloBotCoreLib.InternalModules;
using MuffaloBotNetFramework2;

namespace MuffaloBotCoreLib.CommandsModules
{
    [MuffaloBotCommandsModule]
    class ClientModuleCommands
    {
        [Command("quote")]
        public async Task QuoteWithOption(CommandContext ctx, string option = "random")
        {
            string[] quotes = MuffaloBot.GetModule<QuoteManager>().quotes;
            Random random = new Random();
            switch (option)
            {
                case "random":
                    await ctx.RespondAsync(quotes[random.Next(0, quotes.Length)]);
                    break;
                default:
                    if (int.TryParse(option, out int index))
                    {
                        if (index > quotes.Length)
                        {
                            await ctx.RespondAsync($"Tried to get quote number {index} when there are only {quotes.Length} quotes.");
                            return;
                        }
                        await ctx.RespondAsync(quotes[index - 1]);
                    }
                    else
                    {
                        await ctx.RespondAsync($"Could not find option {option}.");
                    }
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
