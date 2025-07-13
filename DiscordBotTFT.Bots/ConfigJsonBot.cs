using Newtonsoft.Json;

namespace DiscordBotTFT.Bots
{
    struct ConfigJsonBot
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; } 
    }
}
