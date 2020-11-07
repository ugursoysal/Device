using Newtonsoft.Json.Linq;
using Server.Models;
using System.Net;

namespace Server.GameApi
{
    public class MainPlayer
    {
        private string URL;

        public Player game = new Player();

        public MainPlayer(int port = 2999)
        {
            //Logger.Log("create main player p: " + port);
            URL = $"https://127.0.0.1:{port}/liveclientdata/activeplayer";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                new WebClient().DownloadString(URL);
            }
            catch
            {
                URL = $"https://127.0.0.1:2999/liveclientdata/activeplayer";
            }
            Update();
        }
        private void Update()
        {
            string json;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                json = new WebClient().DownloadString(URL);
                //Logger.Log("request: " + URL);
                JObject jo = JObject.Parse(json);
                //Logger.Log(json.ToString());
                game = jo.ToObject<Player>();
                game.CurrentHealth = (int)jo.SelectToken("championStats.currentHealth");
                game.MaxHealth = (int)jo.SelectToken("championStats.maxHealth");
                game.ResourceValue = (int)jo.SelectToken("championStats.resourceValue");
                game.ResourceMax = (int)jo.SelectToken("championStats.resourceMax");
            }
            catch (System.Exception x)
            {
                Logger.Log("LCU is not available: " + x.Message);

                if (GameSession.GameState.Started)
                {
                    GameSession.GameState.Started = false;
                }
                //Logger.Log("update err: " + x.Message);
                game.CurrentHealth = 1;
                game.MaxHealth = 1;
                game.ResourceMax = 1;
                game.ResourceValue = 1;
                game.CurrentGold = 0;
                game.Kills = 0;
                game.Deaths = 0;
                game.Assists = 0;
            }
        }

        public int GetGold()
        {
            Update();
            return (int)game.CurrentGold;
        }
        public bool Dead()
        {
            Update();
            return game.CurrentHealth == 0;
        }
        public int GetLevel()
        {
            return game.Level;
        }

        public int GetHealthPercent()
        {
            return (int)(100 * game.CurrentHealth / game.MaxHealth);
        }
        public int GetManaPercent()
        {
            return (int)(100 * game.ResourceValue / game.ResourceMax);
        }
    }
}
