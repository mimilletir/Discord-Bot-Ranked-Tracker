using DiscordBotTFT.DAL;
using DiscordBotTFT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace DiscordBotTFT.Core.Services.API
{
    public class APIService
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

        public async Task<Profile> GetAccountByName(string pseudo, string tag)
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

        public async Task<Rank> GetAccountRankByPuuid(string puuid)
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
    }
}
