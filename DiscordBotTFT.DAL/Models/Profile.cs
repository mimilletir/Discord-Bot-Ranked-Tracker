namespace DiscordBotTFT.DAL.Models
{
    public class Profile : Entity
    {
        public ulong server { get; set; }
        public string gameName { get; set; }
        public string tagLine { get; set; }
        public string puuid { get; set; }

        public List<Rank> ranks { get; set; } = new List<Rank>();
    }
}
