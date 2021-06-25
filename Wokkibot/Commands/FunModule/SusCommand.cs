using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;


namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("sus", "Find out how sus you are")]
        public async Task SusCommand(InteractionContext ctx)
        {
            Random rnd = new Random();
            int susValue = rnd.Next(1, 100);
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"{ctx.Member.DisplayName} is {susValue}% sus!")
            );
        }
    }
}
