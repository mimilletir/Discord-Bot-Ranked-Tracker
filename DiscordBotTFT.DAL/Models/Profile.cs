namespace DiscordBotTFT.DAL.Models
{
    public class Profile : Entity
    {
        public ulong server { get; set; }
        public string gameName { get; set; }
        public string tagLine { get; set; }
        public string region { get; set; }
        public string puuid { get; set; }

        public bool? currentlyInGame { get; set; }

        public List<Rank>? ranks { get; set; } = new List<Rank>();
    }

    public class Rank : Entity
    {
        public string queueType { get; set; }
        public string tier { get; set; }
        public string rank { get; set; }
        public int leaguePoints { get; set; }
    }
}
