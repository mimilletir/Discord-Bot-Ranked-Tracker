using DiscordBotTFT.Bots.Handlers;
using DiscordBotTFT.Core.Services;
using DiscordBotTFT.DAL;
using DiscordBotTFT.DAL.Models;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks.Dataflow;

namespace DiscordBotTFT.Bots.Commands
{
    public class MatchStatusCommands : ApplicationCommandModule
    {
        private readonly IProfileService _profileService;
        private readonly IAPIService _apiService;
        private readonly IMatchInfoService _matchInfoService;

        public MatchStatusCommands(IProfileService profileService, IAPIService apiService, IMatchInfoService matchInfoService)
        {
            _profileService = profileService;
            _apiService = apiService;
            _matchInfoService = matchInfoService;
        }

        [SlashCommand("init", "initialise le bot dans le channel où la commande est lancée")]
        [RequireRoles(RoleCheckMode.Any, "Admin")]
        public async Task Init(InteractionContext ctx)
        {
            string filePath = "configChannels.json";
            Dictionary<ulong, ulong> channelMap = new();

            string json = await File.ReadAllTextAsync(filePath);
            channelMap = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(json)
                            ?? new Dictionary<ulong, ulong>();

            if (channelMap.ContainsKey(ctx.Guild.Id))
            {
                await ctx.CreateResponseAsync("Ce serveur est déjà initialisé.");
                return;
            }

            channelMap[ctx.Guild.Id] = ctx.Channel.Id;

            string updatedJson = JsonConvert.SerializeObject(channelMap, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, updatedJson);

            await ctx.CreateResponseAsync("Initialisation réussi !");
        }

        public async Task CheckMatchStatus(InteractionContext ctx)
        {
            var channelHandler = new ChannelHandler();
            if (await channelHandler.CheckChannel(ctx.Guild.Id, ctx.Channel.Id))
                return;

            List<Profile> profiles = await _profileService.ListAccountAsync(ctx.Guild.Id);

            foreach (Profile profile in profiles)
            {
                MatchStatusResult matchStatusResult = await _apiService.GetMatchStatusByPuuidAsync(profile.puuid);

                if (matchStatusResult.IsInGame != profile.currentlyInGame && matchStatusResult.IsInGame != null)
                {
                    await _profileService.ChangeMatchStatusState(profile);

                    if (matchStatusResult.IsInGame == true)
                    {
                        MatchInfo matchInfo = await _matchInfoService.GetInfoInGameAsync(ctx.Guild.Id, profile.puuid, matchStatusResult.responseBody);

                        var message = new DiscordEmbedBuilder
                        {
                            Title = $"In Game ({matchInfo.gameMode})",
                            Description = $"{_profileService.GetPlayerNameAsync(ctx.Guild.Id, profile.puuid)}" +
                            $" is currently in game with {matchInfo.championId}"
                        };

                        await ctx.Channel.SendMessageAsync(embed: message);
                    }

                    else if (matchStatusResult.IsInGame == false)
                    {
                        profile.currentlyInGame = false;

                        var matchId = await _matchInfoService.GetMatchIdByPuuidAsync(ctx.Guild.Id, profile.puuid);

                        MatchInfo matchInfo = await _apiService.GetMatchResultByMatchIdAsync(profile.region, matchId);

                        var participant = matchInfo.info.participants.FirstOrDefault(p => p.puuid == profile.puuid);
                        string result = (bool)participant.win ? "Victory" : "Defeat";

                        var message = new DiscordEmbedBuilder
                        {
                            Title = $"{result} ({matchInfo.gameMode})",
                            Description = $"{_profileService.GetPlayerNameAsync(ctx.Guild.Id, profile.puuid)}" +
                            $"{((bool)participant.win ? "won" : "lost")} with {matchInfo.championId}"
                        };

                        message.AddField("Score", $"{participant.kills}/{participant.deaths}/{participant.assists}", true);

                        await ctx.Channel.SendMessageAsync(embed: message);

                        await _matchInfoService.DeleteMatchInfoAsync(ctx.Guild.Id, profile.puuid);
                    }
                }

                else continue;
            }
        }
    }
}
