using Azure;
using DiscordBotTFT.DAL;
using DiscordBotTFT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DiscordBotTFT.Core.Services
{
    public interface IAPIService
    {
        Task<Profile> GetAccountByNameAsync(string pseudo, string tag);
        Task<Profile> GetAccountRegionByPuuidAsync(string puuid);
        Task<Rank> GetAccountRankByPuuidAsync(string puuid);
        Task<MatchStatusResult> GetMatchStatusByPuuidAsync(string puuid);
        Task<MatchInfo> GetMatchResultByMatchIdAsync(string region, long matchId);
    }

    public class APIService : IAPIService
    {
        public string GetApiKey()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("../DiscordBotTFT.Core/Services/config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            return configJson.ApiKey;
        }

        public async Task<Profile> GetAccountByNameAsync(string pseudo, string tag)
        {
            using HttpClient client = new HttpClient();

            try
            {
                string url = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{pseudo}/{tag}?api_key={GetApiKey()}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Profile>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erreur requête : {e.Message}");
                return null;
            }
        }

        public async Task<Profile> GetAccountRegionByPuuidAsync(string puuid)
        {
            using HttpClient client = new HttpClient();

            try
            {
                string url = $"https://europe.api.riotgames.com/riot/account/v1/region/by-game/lol/by-puuid/{puuid}?api_key={GetApiKey()}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Profile>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erreur requête : {e.Message}");
                return null;
            }
        }

        public async Task<Rank> GetAccountRankByPuuidAsync(string puuid)
        {
            using HttpClient client = new HttpClient();

            try
            {
                string url = $"https://euw1.api.riotgames.com/lol/league/v4/entries/by-summoner/{puuid}?api_key={GetApiKey()}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Rank>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erreur requête : {e.Message}");
                return null;
            }
        }

        public async Task<MatchStatusResult> GetMatchStatusByPuuidAsync(string puuid)
        {
            using HttpClient client = new HttpClient();

            try
            {
                string url = $"https://euw1.api.riotgames.com/lol/spectator/v5/active-games/by-summoner/{puuid}?api_key={GetApiKey()}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new MatchStatusResult { IsInGame = false };
                }

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                return new MatchStatusResult
                {
                    IsInGame = true,
                    responseBody = responseBody
                };
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erreur requête : {e.Message}");
                return null;
            }
        }

        public async Task<MatchInfo> GetMatchResultByMatchIdAsync(string region, long matchId)
        {
            using HttpClient client = new HttpClient();

            try
            {
                string url = $"https://europe.api.riotgames.com/lol/match/v5/matches/{region.ToUpper()}_{matchId}?api_key={GetApiKey()}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MatchInfo>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erreur requête : {e.Message}");
                return null;
            }
        }
    }

    public class MatchStatusResult
    {
        public bool IsInGame { get; set; }
        public string responseBody { get; set; }
    }

}
