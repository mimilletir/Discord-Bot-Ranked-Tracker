using DiscordBotTFT.Core.Services.Profiles;
using DiscordBotTFT.DAL;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBotTFT.Bots.Commands
{
    public class ProfileCommands : BaseCommandModule
    {
        private readonly IProfileService _profileService;

        public ProfileCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("addaccount")]
        public async Task AddAccount(CommandContext ctx, [Description("Pseudo")] string pseudo, [Description("Tag")] string tag)
        {
            string result = await _profileService.CreateAccountAsync(pseudo, tag);

            string message = "";

            switch (result)
            {
                case "Exist":
                    message = "Account Already Tracked";
                    return;
                case "Success":
                    message = "Account Successfully Added";
                    return;
                case "Failed":
                    message = "Account Not Found";
                    return;
            }

            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
