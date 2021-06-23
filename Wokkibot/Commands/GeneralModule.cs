using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

public class GeneralModule : SlashCommandModule
{
    DiscordWebhookBuilder builder;

    [SlashCommand("invite", "Get invite link to Wokkibot")]
    public async Task InviteCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        DiscordEmbedBuilder Embed = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor("#0077ea"))
            .WithTitle("Click here to invite")
            .WithUrl("https://discord.com/api/oauth2/authorize?client_id=226891447054368769&permissions=0&scope=bot%20applications.commands");

        builder = new DiscordWebhookBuilder().AddEmbed(Embed);
        await ctx.EditResponseAsync(builder);
    }

    [SlashCommand("status", "Get current bot status")]
    public async Task StatusCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var currentProcessName = Process.GetCurrentProcess().ProcessName;
        var cpuCounter = new PerformanceCounter("Process", "% Processor Time", currentProcessName);
        var memory = GC.GetTotalMemory(true) / 1024 / 1024;
        var runtime = DateTime.Now - Process.GetCurrentProcess().StartTime;

        cpuCounter.NextValue();
        await Task.Delay(500);

        DiscordEmbedBuilder Embed = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor("#0077ea"))
            .WithTitle("Wokkibot Status")
            .AddField("CPU", $"{(int)cpuCounter.NextValue()}%", true)
            .AddField("RAM", $"{(int)memory}MB", true)
            .AddField("Guilds", ctx.Client.Guilds.Count.ToString(), true)
            .AddField("Uptime", $"{getValueFormat(runtime.Days, "day")}, {getValueFormat(runtime.Hours, "hour")}, {getValueFormat(runtime.Minutes, "minute")}, {getValueFormat(runtime.Seconds, "second")}", true);

        builder = new DiscordWebhookBuilder().AddEmbed(Embed);
        await ctx.EditResponseAsync(builder);
    }

    public string getValueFormat(int value, string type)
    {
        if (value == 1)
        {
            return $"{value.ToString()} {type}";
        }
        else
        {
            return $"{value.ToString()} {type}s";
        }
    }
}
