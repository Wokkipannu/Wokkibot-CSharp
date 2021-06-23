using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using DSharpPlus.SlashCommands;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Wokkibot
{
    static class Queue
    {
        public static Dictionary<string, List<TrackItem>> GuildQueue { get; set; }
    }

    public class TrackItem
    {
        public InteractionContext Context { get; set; }
        public LavalinkTrack Track { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Queue.GuildQueue = new Dictionary<string, List<TrackItem>> { };
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var Config = JsonConvert.DeserializeObject<ConfigJson>(json);
            var cfg = new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };

            var Discord = new DiscordClient(cfg);

            // Lavalink configuration
            var endpoint = new ConnectionEndpoint
            {
                Hostname = Config.LavalinkHost,
                Port = Config.LavalinkPort
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = Config.LavalinkPassword,
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = Discord.UseLavalink();

            var slash = Discord.UseSlashCommands();
            slash.RegisterCommands<MusicModule>();
            slash.RegisterCommands<FunModule>();
            slash.RegisterCommands<GeneralModule>();

            await Discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }
    }

    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("lavalink_host")]
        public string LavalinkHost { get; private set; }

        [JsonProperty("lavalink_port")]
        public int LavalinkPort { get; private set; }

        [JsonProperty("lavalink_password")]
        public string LavalinkPassword { get; private set; }
    }
}
