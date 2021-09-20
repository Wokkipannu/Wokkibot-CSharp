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
        [SlashCommand("skip", "Skips current song")]
        public async Task SkipCommand(InteractionContext ctx)
        {
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            var trackItem = Wokkibot.Queue.GetQueue(ctx.Guild.Id.ToString());

            if (trackItem.Item1)
            {
                if (trackItem.Item2.Count > 0)
                {
                    await ctx.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("Skipped")
                    );
                    await conn.StopAsync();
                }
                else
                {
                    await ctx.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("No more songs in queue")
                    );
                }

                return;
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Not in a voice channel")
                );
                return;
            }
        }
    }
}
