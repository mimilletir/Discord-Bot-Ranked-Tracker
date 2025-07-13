using Newtonsoft.Json;

namespace DiscordBotTFT.Core.Services.APIService
{
    struct ConfigJsonAPIKey
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; private set; }
    }
}
