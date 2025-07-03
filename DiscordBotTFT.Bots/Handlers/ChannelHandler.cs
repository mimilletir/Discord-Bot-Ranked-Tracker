using Newtonsoft.Json;

namespace DiscordBotTFT.Bots.Handlers
{
    public class ChannelHandler
    {
        public async Task<bool> CheckChannel(ulong server, ulong channel)
        {
            string filePath = "configChannels.json";
            Dictionary<ulong, ulong> channelMap = new();

            string json = await File.ReadAllTextAsync(filePath);
            channelMap = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(json);

            if (channelMap.TryGetValue(server, out var savedChannel) && channel == savedChannel)
            {
                return false;
            }
            
            return true;
        }
    }
}