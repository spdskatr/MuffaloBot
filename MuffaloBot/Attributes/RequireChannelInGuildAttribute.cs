using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace MuffaloBot.Attributes
{
    sealed class RequireChannelInGuildAttribute : CheckBaseAttribute
    {
        // This is a positional argument
        public RequireChannelInGuildAttribute(string guildName, string channelName)
        {
            Guild = guildName;
            Channel = channelName;
        }

        public string Guild { get; set; }
        
        public string Channel { get; set; }

        public override Task<bool> CanExecute(CommandContext ctx, bool help)
        {
            return Task.FromResult(ctx.Guild.Name != Guild || ctx.Channel.Name == Channel);
        }
    }
}
