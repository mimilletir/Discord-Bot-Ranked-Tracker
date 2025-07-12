using Azure;
using DiscordBotTFT.DAL;
using DiscordBotTFT.DAL.Models;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiscordBotTFT.Core.Services
{
    public interface IMatchInfoService
    {
        Task<MatchInfo> GetInfoInGameAsync(ulong server,string puuid, string RawJson);
        Task<long> GetMatchIdByPuuidAsync(ulong server, string puuid);
        Task DeleteMatchInfoAsync(ulong server, string puuid);
    }

    public class MatchInfoService : IMatchInfoService
    {
        private readonly DbContextOptions<RiotContext> _options;
        private readonly IAPIService _apiService;

        public MatchInfoService(DbContextOptions<RiotContext> options, IAPIService apiService)
        {
            _options = options;
            _apiService = apiService;
        }

        public async Task<MatchInfo> GetInfoInGameAsync(ulong server, string puuid, string RawJson)
        {
            using var context = new RiotContext(_options);

            MatchInfo info = JsonConvert.DeserializeObject<MatchInfo>(RawJson);
            var participant = info.participants.FirstOrDefault(p => p.puuid == puuid);

            var matchInfo = new MatchInfo 
            {
                server = server,
                puuid = puuid,
                gameId = info.gameId,
                gameMode = info.gameMode,
                championId = participant.championId,
            };

            await context.SaveChangesAsync().ConfigureAwait(false);

            return matchInfo;
        }

        public async Task<long> GetMatchIdByPuuidAsync(ulong server, string puuid)
        {
            using var context = new RiotContext(_options);

            var match = await context.MatchInfo.FirstOrDefaultAsync(p => p.server == server && p.puuid == puuid).ConfigureAwait(false);

            return match.gameId;
        }

        public async Task DeleteMatchInfoAsync(ulong server, string puuid)
        {
            using var context = new RiotContext(_options);

            var match = await context.MatchInfo.FirstOrDefaultAsync(p => p.server == server && p.puuid == puuid).ConfigureAwait(false);

            context.Remove(match);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return;
        }
    }
}
