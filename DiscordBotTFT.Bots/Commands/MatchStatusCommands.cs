using DiscordBotTFT.Core.Services.Profiles;
using DSharpPlus.SlashCommands;

namespace DiscordBotTFT.Bots.Commands
{
    public class MatchStatusCommands : ApplicationCommandModule
    {
        private readonly IProfileService _profileService;

        public MatchStatusCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }
    }
}
