using DiscordBotTFT.Core.Services;
using DiscordBotTFT.DAL;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTFT.Bots
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
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

            var serviceProvider = services.BuildServiceProvider();

            var bot = new Bot(serviceProvider);
            services.AddSingleton(bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
        
        }
    }
}
