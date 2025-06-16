using DiscordBotTFT.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTFT.DAL
{
    public class RiotContext : DbContext
    {
        public RiotContext(DbContextOptions<RiotContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
    }
}
