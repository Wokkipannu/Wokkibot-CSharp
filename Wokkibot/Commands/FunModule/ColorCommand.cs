using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Wokkibot.Commands.Utilities;

namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("color", "Change your color")]
        [RequireGuildId(827090972172091392)]
        public async Task ColorCommand(InteractionContext ctx, [Option("color", "The color you want for your username")] string color)
        {
            Dictionary<string, ulong> colors = new Dictionary<string, ulong>()
        {
            { "darkblue", 857643057436426281 },
            { "lightblue", 857643192782028810 },
            { "cyan", 857643932016181268 },
            { "green", 857643539588317215 },
            { "orange", 857643964486909972 },
            { "red", 834482476113330236 },
            { "brown", 857644264070184961 },
            { "pink", 857644506449838090 },
            { "purple", 857643010631139389 },
            { "yellow", 857644716593905674 }
        };

            if (colors.Keys.Contains(color.ToLower()))
            {
                ulong roleId;
                colors.TryGetValue(color, out roleId);
                DiscordRole role = ctx.Guild.GetRole(roleId);

                var roles = ctx.Member.Roles;
                foreach (var r in roles)
                {
                    if (colors.Keys.Contains(r.Name))
                    {
                        await ctx.Member.RevokeRoleAsync(role, "Color change request");
                    }
                }
                await ctx.Member.GrantRoleAsync(role);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"You've been given role {color}"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Invalid color. List of valid colors available at https://peepo.land/wokkibot/colors.html"));
            }
        }
    }
}
