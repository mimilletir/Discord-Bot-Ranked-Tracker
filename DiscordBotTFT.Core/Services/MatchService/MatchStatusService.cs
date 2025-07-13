using DiscordBotTFT.Core.Services.APIService;
using DiscordBotTFT.Core.Services.ProfileService;
using DiscordBotTFT.DAL.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace DiscordBotTFT.Core.Services.MatchService
{
    public interface IMatchStatusService
    {
        Task CheckMatchStatus(DiscordClient client);
    }

    public class MatchStatusService : IMatchStatusService
    {
        private readonly IProfileService _profileService;
        private readonly IAPIService _apiService;
        private readonly IMatchInfoService _matchInfoService;

        public MatchStatusService(IProfileService profileService, IAPIService apiService, IMatchInfoService matchInfoService)
        {
            _profileService = profileService;
            _apiService = apiService;
            _matchInfoService = matchInfoService;
        }

        public async Task CheckMatchStatus(DiscordClient client)
        {
            string filePath = "configChannels.json";
            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);
            var channelMap = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(json)
                             ?? new Dictionary<ulong, ulong>();

            foreach (var entry in channelMap)
            {
                ulong guildId = entry.Key;
                ulong channelId = entry.Value;

                DiscordGuild? guild = null;
                DiscordChannel? channel = null;

                foreach (var g in client.Guilds.Values)
                {
                    if (g.Id == guildId)
                    {
                        guild = g;
                        channel = guild.GetChannel(channelId);
                        break;
                    }
                }

                if (guild == null || channel == null)
                    continue;

                List<Profile> profiles = await _profileService.ListAccountAsync(guildId);

                foreach (Profile profile in profiles)
                {
                    MatchStatusResult matchStatusResult = await _apiService.GetMatchStatusByPuuidAsync(profile.puuid);

                    if (matchStatusResult.IsInGame != profile.currentlyInGame && matchStatusResult.IsInGame != null)
                    {
                        await _profileService.ChangeMatchStatusState(profile);

                        if (matchStatusResult.IsInGame == true)
                        {
                            MatchInfo matchInfo = await _matchInfoService.GetInfoInGameAsync(guildId, profile.puuid, matchStatusResult.responseBody);

                            var message = new DiscordEmbedBuilder
                            {
                                Title = $"In Game ({matchInfo.gameMode})",
                                Description = $"{_profileService.GetPlayerNameAsync(guildId, profile.puuid)}" +
                                $" is currently in game with {matchInfo.championId}"
                            };

                            await channel.SendMessageAsync(embed: message);
                        }

                        else if (matchStatusResult.IsInGame == false)
                        {
                            profile.currentlyInGame = false;

                            var matchId = await _matchInfoService.GetMatchIdByPuuidAsync(guildId, profile.puuid);

                            MatchInfo matchInfo = await _apiService.GetMatchResultByMatchIdAsync(profile.region, matchId);

                            var participant = matchInfo.info.participants.FirstOrDefault(p => p.puuid == profile.puuid);
                            string result = (bool)participant.win ? "Victory" : "Defeat";

                            var message = new DiscordEmbedBuilder
                            {
                                Title = $"{result} ({matchInfo.gameMode})",
                                Description = $"{_profileService.GetPlayerNameAsync(guildId, profile.puuid)}" +
                                $"{((bool)participant.win ? "won" : "lost")} with {matchInfo.championId}"
                            };

                            message.AddField("Score", $"{participant.kills}/{participant.deaths}/{participant.assists}", true);

                            await channel.SendMessageAsync(embed: message);

                            await _matchInfoService.DeleteMatchInfoAsync(guildId, profile.puuid);
                        }
                    }
                }
            }
        }
    }
}
