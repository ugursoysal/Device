namespace Server.Models
{
    public class GameSessionModel
    {
        public int Level { get; set; }
        public string SummonerName { get; set; }
        public bool Started { get; set; }
        public GameSessionModel()
        {
            Started = false;
        }
    }
}
