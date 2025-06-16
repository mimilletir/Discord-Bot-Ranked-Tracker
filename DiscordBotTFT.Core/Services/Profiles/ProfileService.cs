using DiscordBotTFT.Core.Services.API;
using DiscordBotTFT.DAL;
using DiscordBotTFT.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTFT.Core.Services.Profiles
{
    public interface IProfileService
    {
        Task<string> CreateAccountAsync(string pseudo, string tag);
    }

    public class ProfileService : IProfileService
    {
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
    }
}
