using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("roll", "Roll a dice")]
        public async Task RollCommand(InteractionContext ctx, [Option("max", "Max value")] string max = "100")
        {
            if (Int64.TryParse(max, out long maxValue))
            {
                if (maxValue < 1)
                {
                    await ctx.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("Minimum amount must be 1 or higher")
                    );
                    return;
                }
                else
                {
                    long min = 1;
                    long number = LongRandom(min, maxValue, new Random());

                    await ctx.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource, 
                        new DiscordInteractionResponseBuilder().WithContent($"You rolled {number} (1-{maxValue})")
                    );
                }
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Value must be a valid integer")
                );
            }
        }

        // No idea why anyone would want to generate random long number, but it's possible anyway
        private long LongRandom(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}
