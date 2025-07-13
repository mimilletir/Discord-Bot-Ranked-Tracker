using DiscordBotTFT.Bots;
using DiscordBotTFT.Bots.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace DiscordBotTFT.Core.Services
{
    public class Bot : BackgroundService
    {
        private readonly IServiceProvider _services;
        private DiscordClient _client;

        public Bot(DiscordClient client)
        {
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.Ready += OnClientReady;
            await _client.ConnectAsync();
            await Task.Delay(-1, stoppingToken);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            Console.WriteLine("Bot is ready!");
            return Task.CompletedTask;
        }
    }

}
