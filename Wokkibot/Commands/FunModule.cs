using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

public class FunModule : SlashCommandModule
{
    DiscordInteractionResponseBuilder builder;
    DiscordEmbedBuilder Embed;

    [SlashCommand("roll", "Roll a dice")]
    public async Task RollCommand(InteractionContext ctx, [Option("max", "Max value")] string max = "100")
    {
        if (Int64.TryParse(max, out long maxValue))
        {
            if (maxValue < 1)
            {
                builder = new DiscordInteractionResponseBuilder().WithContent("Minimum amount must be 1 or higher");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }
            else
            {
                long min = 1;
                long number = LongRandom(min, maxValue, new Random());

                builder = new DiscordInteractionResponseBuilder().WithContent($"You rolled {number} (1-{maxValue})");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
            }
        }
        else
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("Value must be a valid integer");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
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

    [SlashCommand("sus", "Find out how sus you are")]
    public async Task SusCommand(InteractionContext ctx)
    {
        Random rnd = new Random();
        int susValue = rnd.Next(1, 100);
        builder = new DiscordInteractionResponseBuilder().WithContent($"{ctx.Member.DisplayName} is {susValue}% sus!");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
    }

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

        Embed = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor("#0077ea"))
            .WithTitle($"{user.Username}#{user.Discriminator}")
            .WithThumbnail(user.AvatarUrl)
            .AddField("Created at", user.CreationTimestamp.ToString());

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Embed));
    }

    [SlashCommand("amogus", "Amogus")]
    public async Task AmogusCommand(InteractionContext ctx)
    {
        var emoji = DiscordEmoji.FromName(ctx.Client, ":Amogus:");
        if (emoji != null)
        {
            builder = new DiscordInteractionResponseBuilder().WithContent($"{emoji}");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }

    [SlashCommand("pizza", "Get random pizza toppings")]
    public async Task PizzaCommand(InteractionContext ctx, [Option("amount", "Amount of toppings")] string amount = "4")
    {
        // This is FUCKING HORRIBLE way of doing this, should be moved to a file eventually...
        List<string> Toppings = new List<string>();
        Toppings.Add("Ananas");
        Toppings.Add("Kinkku");
        Toppings.Add("Kebab");
        Toppings.Add("Salami");
        Toppings.Add("Pepperoni");
        Toppings.Add("Herkkusieni");
        Toppings.Add("Persikka");
        Toppings.Add("Aurajuusto");
        Toppings.Add("Chili");
        Toppings.Add("Fetajuusto");
        Toppings.Add("Jalopeno");
        Toppings.Add("Jauheliha");
        Toppings.Add("Kana");
        Toppings.Add("Kananmuna");
        Toppings.Add("Kapris");
        Toppings.Add("Katkarapu");
        Toppings.Add("Mozzarellajuusto");
        Toppings.Add("Oliivi");
        Toppings.Add("Paprika");
        Toppings.Add("Pekoni");
        Toppings.Add("Pippuri");
        Toppings.Add("Rucola");
        Toppings.Add("Simpukka");
        Toppings.Add("Smetana");
        Toppings.Add("Tabasco");
        Toppings.Add("Tomaatti");
        Toppings.Add("Tonnikala");
        Toppings.Add("Tuplajuusto");
        Toppings.Add("Valkosipuli");
        Toppings.Add("BBQ-kastike");
        Toppings.Add("Currykastike");
        Toppings.Add("Häränliha");
        Toppings.Add("Pinaatti");
        Toppings.Add("Punasipuli");
        Toppings.Add("Tacokastike");
        Toppings.Add("Vuohenjuusto");
        Toppings.Add("Cheddar");
        Toppings.Add("Parsa");
        Toppings.Add("Suolakurkku");
        Toppings.Add("Kylmäsavulohi");
        Toppings.Add("Poronliha");
        Toppings.Add("Banaani");
        Toppings.Add("Anjovis");
        Toppings.Add("Avocado");
        Toppings.Add("Munakoiso");
        Toppings.Add("Kurkku");
        Toppings.Add("Parsakaali");
        Toppings.Add("Kirsikkatomaatti");
        Toppings.Add("Salaatti");
        Toppings.Add("Hunaja");

        if (Int32.TryParse(amount, out int toppingsCount))
        {
            if (toppingsCount > Toppings.Count())
            {
                builder = new DiscordInteractionResponseBuilder().WithContent($"Pizzassa ei voi olla enempää kun {Toppings.Count()} täytettä");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
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

            builder = new DiscordInteractionResponseBuilder().WithContent($"Pizzaasi tuli täytteet: {String.Join(", ", SelectedToppings.ToArray())}");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
        else
        {
            builder = new DiscordInteractionResponseBuilder().WithContent("Value must be a valid integer");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }
}