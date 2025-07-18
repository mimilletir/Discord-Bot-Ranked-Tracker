﻿using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace DiscordBotTFT.Bots.Commands
{
    public class InitCommands : ApplicationCommandModule
    {
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
    }
}
