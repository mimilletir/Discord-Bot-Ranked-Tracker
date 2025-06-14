using DiscordBotTFT.DAL.Models.Profiles;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBotTFT.DAL.Models.Items
{
    public class ProfileItem : Entity
    {
        public int ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public Profile Profile { get; set; }
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }
    }
}
