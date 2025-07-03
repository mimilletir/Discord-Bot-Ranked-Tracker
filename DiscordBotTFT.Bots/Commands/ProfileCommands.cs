using DiscordBotTFT.Core.Services.Profiles;
using DiscordBotTFT.DAL;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBotTFT.Bots.Commands
{
    public class ProfileCommands : ApplicationCommandModule
    {
        private readonly IProfileService _profileService;

        public ProfileCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [SlashCommand("addaccount","Ajoute un compte à suivre")]
        public async Task AddAccount(InteractionContext ctx, [Option("Pseudo", "Name")] string pseudo, [Option("Tag", "Tag")] string tag)
        {
            string result = await _profileService.CreateAccountAsync(ctx.Guild.Id, pseudo, tag);

            string message = result switch
            {
                "Exist" => "Account Already Tracked",
                "Success" => "Account Successfully Added",
                "Failed" => "Account Not Found"
            };

            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        [SlashCommand("deleteaccount", "Supprime un compte suivi")]
        public async Task DeleteAccount(InteractionContext ctx, [Option("Pseudo", "Name")] string pseudo, [Option("Tag", "Tag")] string tag)
        {
            string result = await _profileService.DeleteAccountAsync(ctx.Guild.Id, pseudo, tag);

            string message = result switch
            {
                "Success" => "Account Successfully Deleted",
                "Failed" => "Account Not In The Database"
            };

            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        [SlashCommand("list", "Liste de tout les comptes suivis")]
        public async Task ListAccount(InteractionContext ctx)
        {
            DiscordEmbedBuilder result = await _profileService.ListAccountAsync(ctx.Guild.Id);

            await ctx.Channel.SendMessageAsync(embed: result).ConfigureAwait(false);
        }

        [SlashCommand("leaderboard", "Classement des joueurs dans un certain type de queue")]
        public async Task LeaderboardAccount(InteractionContext ctx, [Option("QueueType", "Name")] string queueType)
        {
            DiscordEmbedBuilder result = await _profileService.LeaderboardAsync(ctx.Guild.Id, queueType);

            await ctx.Channel.SendMessageAsync(embed: result).ConfigureAwait(false);
        }
    }
}
