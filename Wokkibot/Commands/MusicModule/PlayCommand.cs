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
        [SlashCommand("play", "Play song or add it to queue")]
        public async Task PlayCommand(InteractionContext ctx, [Option("search", "Keyword to search with")] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("You must be connected to a voice channel")
                );
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            // If we are not connected, try connecting to the channel the user is in currenctly
            if (conn == null)
            {
                var channel = ctx.Member.VoiceState.Channel;

                if (channel.Type != ChannelType.Voice)
                {
                    await ctx.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("Channel must be a voice channel")
                    );
                    return;
                }

                await node.ConnectAsync(channel);
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder().WithContent($"No search results for {search}")
                );
                return;
            }

            var track = loadResult.Tracks.First();

            List<TrackItem> trackQueue;

            if (Wokkibot.Queue.GuildQueue.ContainsKey(ctx.Guild.Id.ToString()) == false)
            {
                Wokkibot.Queue.GuildQueue.Add(ctx.Guild.Id.ToString(), new List<TrackItem> { new TrackItem { Context = ctx, Track = track } });
                trackQueue = Wokkibot.Queue.GuildQueue[ctx.Guild.Id.ToString()];
            }
            else
            {
                trackQueue = Wokkibot.Queue.GuildQueue[ctx.Guild.Id.ToString()];
                trackQueue.Add(new TrackItem { Context = ctx, Track = track });
            }

            if (trackQueue.Count == 1)
            {
                await conn.PlayAsync(track);
                conn.PlaybackFinished += PlayNext;
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder().WithContent($"Now playing {track.Title} requested by {ctx.Member.DisplayName}")
                );
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder().WithContent($"Added {track.Title} to queue")
                );
            }
        }

        private async Task PlayNext(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs e)
        {
            List<TrackItem> trackQueue = Wokkibot.Queue.GuildQueue[sender.Guild.Id.ToString()];
            var ctx = trackQueue[0].Context;
            trackQueue.RemoveAt(0);

            if (trackQueue.Count == 0)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder().WithContent("No more songs in queue")
                );
                return;
            }

            var track = trackQueue.First();
            var lava = track.Context.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(track.Context.Member.VoiceState.Guild);

            await conn.PlayAsync(track.Track);
            //conn.PlaybackFinished += PlayNext;
            await track.Context.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"Now playing {track.Track.Title} requested by {track.Context.Member.DisplayName}")
            );
        }
    }
}
