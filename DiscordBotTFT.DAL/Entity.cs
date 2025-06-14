using System.ComponentModel.DataAnnotations;

namespace DiscordBotTFT.DAL
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
