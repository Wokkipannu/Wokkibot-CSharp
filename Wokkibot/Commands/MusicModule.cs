using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Wokkibot;

public class MusicModule : SlashCommandModule
{
    DiscordInteractionResponseBuilder builder;

    [SlashCommand("play", "Play song or add it to queue")]
    public async Task PlayCommand(InteractionContext ctx, [Option("search", "Keyword to search with")] string search)
    {
        if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("You must be connected to a voice channel");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
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
                builder = new DiscordInteractionResponseBuilder().WithContent("Channel must be a voice channel");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            await node.ConnectAsync(channel);
        }

        var loadResult = await node.Rest.GetTracksAsync(search);

        if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            builder = new DiscordInteractionResponseBuilder().WithContent($"No search results for {search}");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
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
            builder = new DiscordInteractionResponseBuilder().WithContent($"Now playing {track.Title} requested by {ctx.Member.DisplayName}");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
        else
        {
            builder = new DiscordInteractionResponseBuilder().WithContent($"Added {track.Title} to queue");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }

    private async Task PlayNext(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs e)
    {
        List<TrackItem> trackQueue = Wokkibot.Queue.GuildQueue[sender.Guild.Id.ToString()];
        trackQueue.RemoveAt(0);

        if (trackQueue.Count == 0)
        {
            //builder = new DiscordInteractionResponseBuilder().WithContent("No more songs in queue");
            //await track.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            Console.WriteLine("No more songs in queue");
            return;
        }

        var track = trackQueue.First();
        var lava = track.Context.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(track.Context.Member.VoiceState.Guild);

        await conn.PlayAsync(track.Track);
        //conn.PlaybackFinished += PlayNext;
        builder = new DiscordInteractionResponseBuilder().WithContent($"Now playing {track.Track.Title} requested by {track.Context.Member.DisplayName}");
        await track.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
    }
    
    [SlashCommand("skip", "Skips current song")]
    public async Task SkipCommand(InteractionContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null)
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("Lavalink is not connected");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            return;
        }

        Console.WriteLine(conn.CurrentState);

        await conn.StopAsync();

        builder = new DiscordInteractionResponseBuilder().WithContent("Skipped");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
    }

    [SlashCommand("join", "Join your current channel")]
    public async Task JoinCommand(InteractionContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("Lavalink connection is not esbalished");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            return;
        }

        var node = lava.ConnectedNodes.Values.First();

        var channel = ctx.Member.VoiceState.Channel;

        if (channel.Type != ChannelType.Voice)
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("Channel must be a voice channel");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            return;
        }

        await node.ConnectAsync(channel);
        builder = new DiscordInteractionResponseBuilder().WithContent("Joined channel");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
    }

    [SlashCommand("leave", "Leave channel")]
    public async Task LeaveCommand(InteractionContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("Lavalink connection is not esbalished");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            return;
        }

        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Guild);

        if (conn == null)
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("Lavalink is not connected");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            return;
        }

        await conn.DisconnectAsync();
        builder = new DiscordInteractionResponseBuilder().WithContent("Left channel");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
    }
}