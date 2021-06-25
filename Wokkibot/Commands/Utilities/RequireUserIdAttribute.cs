using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

namespace Wokkibot.Commands.Utilities
{
    class RequireUserIdAttribute : SlashCheckBaseAttribute
    {
        public ulong Id;

        public RequireUserIdAttribute(ulong id)
            => Id = id;

        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
            => await Task.Run(() => ctx.User.Id == Id);
    }
}
