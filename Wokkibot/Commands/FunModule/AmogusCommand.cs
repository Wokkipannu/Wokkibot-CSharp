using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("amogus", "Amogus")]
        public async Task AmogusCommand(InteractionContext ctx)
        {
            var emoji = DiscordEmoji.FromName(ctx.Client, ":Amogus:");
            if (emoji != null)
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"{emoji}")
                );
            }
        }
    }
}
