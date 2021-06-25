using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("age", "Get account age")]
        public async Task AgeCommand(InteractionContext ctx, [Option("user", "The user to look up for")] DiscordUser user = null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            user ??= ctx.Member;

            // Try assigning variable username value of user.Username to check if user exists in cache
            // Most likely better ways exist but this works for now
            try
            {
                var username = user.Username;
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("The given key '"))
                {
                    var userId = e.Message
                        .Replace("The given key '", "")
                        .Replace("' was not present in the dictionary.", "");

                    user = await ctx.Client.GetUserAsync(Convert.ToUInt64(userId));
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to fetch user data: {e.Message}"));
                }
            }

            await ctx.EditResponseAsync(
                new DiscordWebhookBuilder()
                .AddEmbed(
                    new DiscordEmbedBuilder()
                        .WithColor(new DiscordColor("#0077ea"))
                        .WithTitle($"{user.Username}#{user.Discriminator}")
                        .WithThumbnail(user.AvatarUrl)
                        .AddField("Created at", user.CreationTimestamp.ToString())
            ));
        }
    }
}
