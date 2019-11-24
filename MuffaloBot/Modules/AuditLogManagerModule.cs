using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBot.Modules
{
#if DEBUG
    class AuditLogManagerModule : BaseModule
    {
        protected override void Setup(DiscordClient client)
        {
            Client = client;
        }

        private async Task UpdateAuditLogs(AuditLogActionType type, DiscordGuild guild)
        {
            DiscordAuditLogEntry entryLatest = (await guild.GetAuditLogsAsync(limit: 1, action_type: type))[0];
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            DiscordUser responsible = entryLatest.UserResponsible;
            builder.WithAuthor(responsible.Username, null, responsible.AvatarUrl);

            switch (entryLatest.ActionCategory)
            {
                case AuditLogActionCategory.Create:
                    builder.Color = DiscordColor.Green;
                    break;
                case AuditLogActionCategory.Update:
                    builder.Color = DiscordColor.Yellow;
                    break;
                case AuditLogActionCategory.Delete:
                    builder.Color = DiscordColor.Red;
                    break;
                case AuditLogActionCategory.Other:
                    builder.Color = DiscordColor.White;
                    break;
                default:
                    break;
            }

            switch (entryLatest.ActionType)
            {
                case AuditLogActionType.GuildUpdate:
                    builder.WithTitle("Guild updated");
                    break;
                case AuditLogActionType.ChannelCreate:
                    builder.WithTitle("Channel created");
                    break;
                case AuditLogActionType.ChannelUpdate:
                    builder.WithTitle("Channel updated");
                    break;
                case AuditLogActionType.ChannelDelete:
                    builder.WithTitle("Channel deleted");
                    break;
                case AuditLogActionType.OverwriteCreate:
                    builder.WithTitle("Permission override for channel created");
                    break;
                case AuditLogActionType.OverwriteUpdate:
                    builder.WithTitle("Permission override for channel updated");
                    break;
                case AuditLogActionType.OverwriteDelete:
                    builder.WithTitle("Permission override for channel deleted");
                    break;
                case AuditLogActionType.Kick:
                    builder.WithTitle("User kicked");
                    break;
                case AuditLogActionType.Prune:
                    builder.WithTitle("Users pruned");
                    break;
                case AuditLogActionType.Ban:
                    builder.WithTitle("User banned");
                    break;
                case AuditLogActionType.Unban:
                    builder.WithTitle("User unbanned");
                    break;
                case AuditLogActionType.MemberUpdate:
                    builder.WithTitle("Member updated");
                    break;
                case AuditLogActionType.MemberRoleUpdate:
                    builder.WithTitle("Member role updated");
                    break;
                case AuditLogActionType.RoleCreate:
                    builder.WithTitle("Role created");
                    break;
                case AuditLogActionType.RoleUpdate:
                    builder.WithTitle("Role updated");
                    break;
                case AuditLogActionType.RoleDelete:
                    builder.WithTitle("Role deleted");
                    break;
                case AuditLogActionType.InviteCreate:
                    builder.WithTitle("Invite created");
                    break;
                case AuditLogActionType.InviteUpdate:
                    builder.WithTitle("Invite updated");
                    break;
                case AuditLogActionType.InviteDelete:
                    builder.WithTitle("Invite deleted");
                    break;
                case AuditLogActionType.WebhookCreate:
                    builder.WithTitle("Webhook created");
                    break;
                case AuditLogActionType.WebhookUpdate:
                    builder.WithTitle("Webhook updated");
                    break;
                case AuditLogActionType.WebhookDelete:
                    builder.WithTitle("Webhook deleted");
                    break;
                case AuditLogActionType.EmojiCreate:
                    builder.WithTitle("Emoji created");
                    break;
                case AuditLogActionType.EmojiUpdate:
                    builder.WithTitle("Emoji updated");
                    break;
                case AuditLogActionType.EmojiDelete:
                    builder.WithTitle("Emoji deleted");
                    break;
                case AuditLogActionType.MessageDelete:
                    builder.WithTitle("Message deleted (See logs channel for more info)");
                    break;
                default:
                    builder.WithTitle("Unknown event");
                    break;
            }

            Type entryType = entryLatest.GetType();
            PropertyInfo[] properties = entryType.GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                if (typeof(PropertyChange<>).IsAssignableFrom(prop.PropertyType))
                {
                    Type changeType = prop.PropertyType.GetGenericArguments()[0];
                }
                else
                {

                }
            }
        }

        private async Task<string> ToStringHumanReadable(object obj, DiscordGuild guild)
        {
            if (obj is string s)
            {
                return s;
            }
            else if (obj is int i)
            {
                return i.ToString();
            }
            else if (obj is DiscordGuild g)
            {
                return g.ToString();
            }
            else if (obj is DiscordChannel c)
            {
                if (c.Type == ChannelType.Text)
                {
                    return $"<#{c.Id}>";
                }
                return c.ToString();
            }
            else if (obj is DiscordOverwrite ov)
            {
                string result = "";
                if (ov.Type == "role")
                {
                    result += $"Role {ToStringHumanReadable(guild.GetRole(ov.Id), guild)}:\n";
                }
                else if (ov.Type == "member")
                {
                    result += $"Member {ToStringHumanReadable(await guild.GetMemberAsync(ov.Id).ConfigureAwait(false), guild)}";
                }
                return $"Allow: {ov.Allow.ToPermissionString()}\nDeny: {ov.Deny.ToPermissionString()}";
            }
            else if (obj is Permissions perms)
            {
                return perms.ToPermissionString();
            }
            else if (obj is DiscordMember m)
            {
                return m.Mention;
            }
            else if (obj is DiscordRole r)
            {
                return r.Mention;
            }
            else if (obj is DiscordInvite inv)
            {
                return inv.ToString();
            }
            else if (obj is DiscordWebhook w)
            {
                return w.ToString();
            }
            else if (obj is DiscordEmoji e)
            {
                return e.ToString();
            }
            else if (obj is DiscordMessage msg)
            {
                return $"https://discordapp.com/channels/{msg.Channel.GuildId}/{msg.ChannelId}/{msg.Id}";
            }
            else if (obj is IReadOnlyList<DiscordRole> roles)
            {
                return string.Join(" ", roles.Select(rl => ToStringHumanReadable(rl, guild)));
            }
            return null;
        }

        private async Task ProcessDiscordAuditLogGuildEntry(DiscordAuditLogGuildEntry entry, DiscordEmbedBuilder builder)
        {
            var nameChange = entry.NameChange;
            var ownerChange = entry.OwnerChange;
            var iconChange = entry.IconChange;
            var verificationLevelChange = entry.VerificationLevelChange;
            var afkChannelChange = entry.AfkChannelChange;
        }

        private async Task ProcessDiscordAuditLogChannelEntry(DiscordAuditLogChannelEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogOverwriteEntry(DiscordAuditLogOverwriteEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogKickEntry(DiscordAuditLogKickEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogPruneEntry(DiscordAuditLogPruneEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogBanEntry(DiscordAuditLogBanEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogMemberUpdateEntry(DiscordAuditLogMemberUpdateEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogRoleUpdateEntry(DiscordAuditLogRoleUpdateEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogInviteEntry(DiscordAuditLogInviteEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogWebhookEntry(DiscordAuditLogWebhookEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogEmojiEntry(DiscordAuditLogEmojiEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task ProcessDiscordAuditLogMessageEntry(DiscordAuditLogMessageEntry entry, DiscordEmbedBuilder builder)
        {

        }

        private async Task NotifyLogAsync(DiscordEmbed embed, DiscordGuild guild)
        {
            DiscordChannel channel = (await guild.GetChannelsAsync()).First(c => c.Name == "logs");
            if (channel != null)
            {
                Permissions permissions = channel.PermissionsFor(guild.CurrentMember);
                if ((permissions & Permissions.SendMessages) != 0)
                {
                    await channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                }
            }
        }
    }
#endif
}
