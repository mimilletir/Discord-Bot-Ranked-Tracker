using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBotTFT.Core.Services.MatchService
{
    public class MatchStatusBackgroundService : BackgroundService
    {
        private readonly DiscordClient _client;
        private readonly IServiceProvider _serviceProvider;

        public MatchStatusBackgroundService(DiscordClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var matchStatusService = scope.ServiceProvider.GetRequiredService<IMatchStatusService>();
                        await matchStatusService.CheckMatchStatus(_client);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'exécution de CheckMatchStatus : {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
