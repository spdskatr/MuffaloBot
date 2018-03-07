using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MuffaloBot.DiscordComponent;
using MuffaloBot.InternalModules;
using MuffaloBot;

namespace MuffaloBot.CommandsModules
{
    [MuffaloBotCommandsModule]
    class ClientModuleCommands
    {
        [Command("mbhelp")]
        public async Task MuffaloBotHelp(CommandContext ctx, string command = null)
        {
            if (command == null)
            {
                await ctx.RespondAsync(embed: MuffaloBotProgram.createdInstances[ctx.Client].GetModule<HelpProvider>().GeneralHelp());
            }
            else
            {
                await ctx.RespondAsync(embed: MuffaloBotProgram.createdInstances[ctx.Client].GetModule<HelpProvider>().EmbedFromHelpCommand(command));
            }
        }
    }
}
