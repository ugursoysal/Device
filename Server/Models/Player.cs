using Newtonsoft.Json;
namespace Server.Models
{
    public class Player
    {
        public double CurrentHealth { get; set; }
        public double MaxHealth { get; set; }
        public double ResourceValue { get; set; }
        public double ResourceMax { get; set; }

        [JsonProperty("currentGold")]
        public double CurrentGold { get; set; }
        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonProperty("summonerName")]
        public string SummonerName { get; set; }
        public double Kills { get; set; }
        public double Deaths { get; set; }
        public double Assists { get; set; }
        public double CreepScore { get; set; }
    }
}
