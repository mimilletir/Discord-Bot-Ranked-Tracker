using Azure;
using DiscordBotTFT.Core.Services.API;
using DiscordBotTFT.DAL;
using DiscordBotTFT.DAL.Models;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiscordBotTFT.Core.Services.Profiles
{
    public interface IProfileService
    {
        Task<string> CreateAccountAsync(string pseudo, string tag);
        Task<string> DeleteAccountAsync(string pseudo, string tag);
        Task<DiscordEmbedBuilder> ListAccountAsync();
        Task<DiscordEmbedBuilder> LeaderboardAsync(string queueType);
    }

    public class ProfileService : IProfileService
    {
        Dictionary<string, int> rankDictionary = new Dictionary<string, int>
        {
            { "IRON", 0000 },
            { "BRONZE", 1000 },
            { "SILVER", 2000 },
            { "GOLD", 3000 },
            { "PLATINUM", 4000 },
            { "EMERALD", 5000 },
            { "DIAMOND", 6000 },
            { "MASTER", 7000 },
            { "GRANDMASTER", 8000 },
            { "CHALLENGER", 9000 },
            { "I", 900 },
            { "II", 800 },
            { "III", 700 },
            { "IV", 600 },
        };

        private readonly DbContextOptions<RiotContext> _options;
        private readonly APIService _apiService;

        public ProfileService(DbContextOptions<RiotContext> options, APIService apiService)
        {
            _options = options;
            _apiService = apiService;
        }

        public async Task<string> CreateAccountAsync(string pseudo, string tag)
        {
            using var context = new RiotContext(_options);

            var profile = await context.Profiles
                .FirstOrDefaultAsync(x => x.gameName == pseudo && x.tagLine == tag).ConfigureAwait(false);

            if (profile != null) { return "Exist"; }

            var account = await _apiService.GetAccountByName(pseudo, tag);

            if (account == null) { return "Failed"; }

            profile = new Profile
            {
                gameName = account.gameName,
                tagLine = account.tagLine,
                puuid = account.puuid
            };

            context.Add(profile);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return "Success";
        }

        public async Task<string> DeleteAccountAsync(string pseudo, string tag)
        {
            using var context = new RiotContext(_options);

            var profile = await context.Profiles
                .FirstOrDefaultAsync(x => x.gameName == pseudo && x.tagLine == tag).ConfigureAwait(false);

            if (profile != null)
            {
                context.Remove(profile);

                await context.SaveChangesAsync().ConfigureAwait(false);

                return "Success"; 
            }

            return "Failed";
        }

        public async Task<DiscordEmbedBuilder> ListAccountAsync()
        {
            using var context = new RiotContext(_options);

            var profiles = await context.Profiles.ToListAsync();

            var profileEmbed = new DiscordEmbedBuilder
            { 
                Title = "Tracked Accounts"
            };

            if (profiles.Count > 0)
            {
                var pseudoList = string.Join("\n", profiles.Select(p => $"{p.gameName}#{p.tagLine}"));

                profileEmbed.AddField("\u200b", pseudoList);
            }

            else
            {
                profileEmbed.AddField("\u200b", "No Tracked Account");
            }

            return profileEmbed;
        }

        public async Task<DiscordEmbedBuilder> LeaderboardAsync(string queueType)
        {
            using var context = new RiotContext(_options);

            var profiles = await context.Profiles.ToListAsync();

            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = "Tracked Accounts"
            };

            if (profiles.Count > 0)
            {
                List<(string, int)> profileList = new List<(string, int)> { };

                foreach (Profile profile in profiles)
                {
                    var account = await _apiService.GetAccountRankByPuuid(profile.puuid);

                    if (account == null) { return null; }

                    int score = 0;
                    int tempScore = 0;

                    rankDictionary.TryGetValue(account.tier, out tempScore);
                    score += tempScore;

                    rankDictionary.TryGetValue(account.rank, out tempScore);
                    score += tempScore;

                    score += account.leaguePoints;

                    profile.ranks.Add(new Rank
                    {
                        queueType = account.queueType,
                        tier = account.tier,
                        rank = account.rank,
                        leaguePoints = account.leaguePoints
                    });

                    profileList.Add((profile.puuid, score));

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                profileList.Sort(delegate((string, int) x, (string, int) y)
                {
                    return y.Item2.CompareTo(x.Item2);
                });

                List<string> accountDataList = new List<string>();

                foreach ((string, int) profile in profileList)
                {
                    var p = await context.Profiles
                        .FirstOrDefaultAsync(x => x.puuid == profile.Item1).ConfigureAwait(false);

                    var rank = p.ranks.FirstOrDefault(r => r.queueType == "RANKED_SOLO_5x5");
                    accountDataList.Add($"{p.gameName}#{p.tagLine} is at {rank.tier} {rank.rank} {rank.leaguePoints}");
                };

                var leaderboardList = string.Join("\n", accountDataList);

                profileEmbed.AddField("\u200b", leaderboardList);
            }

            else
            {
                profileEmbed.AddField("\u200b", "No Tracked Account");
            }

            return profileEmbed;
        }
    }
}
