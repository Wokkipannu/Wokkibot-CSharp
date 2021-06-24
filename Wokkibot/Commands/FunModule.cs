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
        var json = "";
        using (var fs = File.OpenRead("toppings.json"))
        using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = await sr.ReadToEndAsync();

        List<string> Toppings = JsonConvert.DeserializeObject<List<string>>(json);

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

    [SlashCommand("color", "Change your color")]
    [RequireGuildId(827090972172091392)]
    public async Task ColorCommand(InteractionContext ctx, [Option("color", "The color you want for your username")] string color)
    {
        Dictionary<string, ulong> colors = new Dictionary<string, ulong>()
        {
            { "darkblue", 857643057436426281 },
            { "lightblue", 857643192782028810 },
            { "cyan", 857643932016181268 },
            { "green", 857643539588317215 },
            { "orange", 857643964486909972 },
            { "red", 834482476113330236 },
            { "brown", 857644264070184961 },
            { "pink", 857644506449838090 },
            { "purple", 857643010631139389 },
            { "yellow", 857644716593905674 }
        };

        if (colors.Keys.Contains(color.ToLower()))
        {
            ulong roleId;
            colors.TryGetValue(color, out roleId);
            DiscordRole role = ctx.Guild.GetRole(roleId);

            var roles = ctx.Member.Roles;
            foreach (var r in roles)
            {
                if (colors.Keys.Contains(r.Name))
                {
                    await ctx.Member.RevokeRoleAsync(role, "Color change request");
                }
            }
            await ctx.Member.GrantRoleAsync(role);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"You've been given role {color}"));
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Invalid color. List of valid colors available at https://peepo.land/wokkibot/colors.html"));
        }
    }

    public class RequireGuildIdAttribute : SlashCheckBaseAttribute
    {
        public ulong Id;

        public RequireGuildIdAttribute(ulong id)
            => Id = id;

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
            => await Task.Run(SyncCheck(ctx.Guild.Id));
        
        private bool SyncCheck(bool GId)
            => GId == Id;
    }

    public class RequireUserIdAttribute : SlashCheckBaseAttribute
    {
        public ulong Id;

        public RequireUserIdAttribute(ulong id)
            => Id = id;

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
            => await Task.Run(SyncCheck(ctx.User.Id));
            
        private bool SyncCheck(bool UId)
            => UId == Id;
    }
}
