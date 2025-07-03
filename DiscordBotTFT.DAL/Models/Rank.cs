namespace DiscordBotTFT.DAL.Models
{
    public class Rank : Entity
    {
        public string queueType { get; set; }
        public string tier { get; set; }
        public string rank { get; set; }
        public int leaguePoints { get; set; }
    }
}
