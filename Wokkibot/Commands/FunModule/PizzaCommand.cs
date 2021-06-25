using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace Wokkibot.Commands
{
    public partial class CommandModule : SlashCommandModule
    {
        [SlashCommand("pizza", "Get random pizza toppings")]
        public async Task PizzaCommand(InteractionContext ctx, [Option("amount", "Amount of toppings")] string amount = "4")
        {
            var json = "";
            using (var fs = File.OpenRead("toppings.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            List<string> Toppings = JsonConvert.DeserializeObject<List<string>>(json);

            if (Int32.TryParse(amount, out int toppingsCount))
            {
                if (toppingsCount > Toppings.Count())
                {
                    await ctx.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource, 
                        new DiscordInteractionResponseBuilder().WithContent($"Pizzassa ei voi olla enempää kun {Toppings.Count()} täytettä")
                    );
                    return;
                }

                List<string> SelectedToppings = new List<string>();
                Random rnd = new Random();
                for (int i = 0; i < toppingsCount; i++)
                {
                    int index = rnd.Next(Toppings.Count);
                    SelectedToppings.Add(Toppings[index]);
                    Toppings.RemoveAt(index);
                }

                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Pizzaasi tuli täytteet: {String.Join(", ", SelectedToppings.ToArray())}")
                );
            }
            else
            {
                await ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder().WithContent("Value must be a valid integer")
                );
            }
        }
    }
}
