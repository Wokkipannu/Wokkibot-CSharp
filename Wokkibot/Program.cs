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
using Wokkibot.Commands;

namespace Wokkibot
{
    static class Queue
    {
        private static Dictionary<string, List<TrackItem>> GuildQueue;


        public static void Init()
        {
            GuildQueue = new Dictionary<string, List<TrackItem>> { };
        }

        public static (bool, List<TrackItem>) GetQueue(string id)
        {
            if (GuildQueue.ContainsKey(id))
            {
                return (true, GuildQueue[id]);
            } else
            {
                return (false, new List<TrackItem> { });
            }
        }

        public static List<TrackItem> AddToQueue(InteractionContext ctx, LavalinkTrack track)
        {
            var guildID = ctx.Guild.Id.ToString();
            var trackItem = new TrackItem { Context = ctx, Track = track };

            var trackQueue = GetQueue(guildID);
            if (!trackQueue.Item1)
            {
                GuildQueue.Add(guildID, new List<TrackItem> { trackItem });
            } else
            {
                trackQueue.Item2.Add(trackItem);
                GuildQueue[guildID] = trackQueue.Item2;
            }

            return GuildQueue[guildID];
        }

        public enum NextStatus
        {
            Found,
            Empty,
            NoGuild,
        }

        public static (NextStatus, TrackItem) GetNext(string id)
        {
            var trackQueue = GetQueue(id);

            if (!trackQueue.Item1)
            {
                return (NextStatus.Empty, new TrackItem { });
            }
            else
            {
                var ctx = trackQueue.Item2[0].Context;
                trackQueue.Item2.RemoveAt(0);
                GuildQueue[id] = trackQueue.Item2;
                if (GuildQueue[id].Count > 0)
                {
                    return (NextStatus.Found, GuildQueue[id][0]);
                }
                else
                {
                    return (NextStatus.Empty, new TrackItem { Context = ctx, Track = new LavalinkTrack() });
                }
            }
        }
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
            Queue.Init();
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
            slash.RegisterCommands<CommandModule>();

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
