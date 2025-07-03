using DiscordBotTFT.Core.Services.API;
using DiscordBotTFT.Core.Services.Profiles;
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
            services.AddScoped<APIService>();
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
