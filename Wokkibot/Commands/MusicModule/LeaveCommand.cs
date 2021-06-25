using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("leave", "Leave channel")]
        public async Task LeaveCommand(InteractionContext ctx)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Lavalink connection is not esbalished")
                );
                return;
            }

            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder().WithContent("Lavalink is not connected")
                );
                return;
            }

            await conn.DisconnectAsync();
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Left channel")
            );
        }
    }
}
