using Newtonsoft.Json;

namespace DiscordBotTFT.Core.Services
{
    struct ConfigJson
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; private set; }
    }
}
