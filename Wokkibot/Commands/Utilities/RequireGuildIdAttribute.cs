using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

namespace Wokkibot.Commands.Utilities
{
    public class RequireGuildIdAttribute : SlashCheckBaseAttribute
    {
        public ulong Id;

        public RequireGuildIdAttribute(ulong id)
            => Id = id;

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
            => await Task.Run(() => ctx.Guild.Id == Id);
    }
}
