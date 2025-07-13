using DiscordBotTFT.Bots.Commands;
using DiscordBotTFT.Core.Services;
using DiscordBotTFT.Core.Services.APIService;
using DiscordBotTFT.Core.Services.MatchService;
using DiscordBotTFT.Core.Services.ProfileService;
using DiscordBotTFT.DAL;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DiscordBotTFT.Bots
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DiscordClient>(sp =>
            {
                var json = File.ReadAllText("configBot.json");
                var configJson = JsonConvert.DeserializeObject<ConfigJsonBot>(json);

                var discordConfig = new DiscordConfiguration
                {
                    Token = configJson.Token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    MinimumLogLevel = LogLevel.Information,
                    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.Guilds | DiscordIntents.GuildMembers | DiscordIntents.GuildMessages | DiscordIntents.MessageContents,
                };

                var client = new DiscordClient(discordConfig);

                client.UseInteractivity(new InteractivityConfiguration
                {
                    Timeout = TimeSpan.FromMinutes(2)
                });

                var slash = client.UseSlashCommands(new SlashCommandsConfiguration
                {
                    Services = sp
                });

                slash.RegisterCommands<ProfileCommands>();
                slash.RegisterCommands<InitCommands>();

                return client;
            });

            services.AddDbContext<RiotContext>(options =>
            {
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=RiotContext;Trusted_Connection=True;MultipleActiveResultSets=true",
                    x => x.MigrationsAssembly("DiscordBotTFT.DAL"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddHttpClient();

            services.AddScoped<IAPIService, APIService>();
            services.AddScoped<IMatchInfoService, MatchInfoService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IMatchStatusService, MatchStatusService>();

            services.AddHostedService<Bot>();
            services.AddHostedService<MatchStatusBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
        
        }
    }
}
