using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("invite", "Get invite link to Wokkibot")]
        public async Task InviteCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);


            await ctx.EditResponseAsync(
                new DiscordWebhookBuilder()
                .AddEmbed(
                    new DiscordEmbedBuilder()
                        .WithColor(new DiscordColor("#0077ea"))
                        .WithTitle("Click here to invite")
                        .WithUrl($"https://discord.com/api/oauth2/authorize?client_id={ctx.Client.CurrentApplication.Id}&permissions=0&scope=bot%20applications.commands"))
            );
        }
    }
}
