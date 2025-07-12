namespace DiscordBotTFT.DAL.Models
{
    public class MatchInfo : Entity
    {
        public ulong server { get; set; }
        public string puuid { get; set; }
        public long gameId { get; set; }
        public string gameMode { get; set; }
        public long championId { get; set; }

        public List<Participant> participants { get; set; }

        public MatchInfoDetail? info { get; set; }
    }
    public class MatchInfoDetail : Entity
    {
        public List<Participant> participants { get; set; }
    }

    public class Participant : Entity
    {
        public string puuid { get; set; }
        public long championId { get; set; }
        public int? kills { get; set; }
        public int? deaths { get; set; }
        public int? assists { get; set; }
        public bool? win {  get; set; }
    }
}
