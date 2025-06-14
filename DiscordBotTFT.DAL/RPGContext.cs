using DiscordBotTFT.DAL.Models.Items;
using DiscordBotTFT.DAL.Models.Profiles;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTFT.DAL
{
    public class RPGContext : DbContext
    {
        public RPGContext(DbContextOptions<RPGContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ProfileItem> ProfileItems { get; set; }
    }
}
