using Leaf.xNet;
using Newtonsoft.Json.Linq;
using Server.Game;
using System;
using System.Management;
using System.Text.RegularExpressions;

namespace Server
{
    public class LCU
    {
        public static long SummonerID = 0;
        public int port;
        public string auth;
        private static readonly string RegexPattern = "\"--remoting-auth-token=(?'token'.*?)\" | \"--app-port=(?'port'|.*?)\"";
        private static readonly RegexOptions RegexOption = RegexOptions.Multiline;
        HttpRequest request = new HttpRequest();

        public LCU()
        {
            this.ReadProcess();
            if (!SetSummonerID())
                Logger.Log("Couldn't set summoner ID.");
        }

        #region misc

        private void UpdateRequest()
        {
            this.request = new HttpRequest();
            this.request.AddHeader("Authorization", "Basic " + this.auth);
            this.request.AddHeader("Accept", "application/json");
            this.request.AddHeader("content-type", "application/json");
            this.request.IgnoreProtocolErrors = true;
        }


        #endregion
        public void StartQueue()
        {
            UpdateRequest();
            String url = "https://127.0.0.1:" + this.port + "/lol-lobby/v2/lobby/matchmaking/search";
            request.AddHeader("Authorization", "Basic " + this.auth);
            request.Post(url).ToString();
        }

        public int LeaverBuster()
        {
            //{ "errors":[],"lowPriorityData":{ "bustedLeaverAccessToken":"","penalizedSummonerIds":[2573812798891744],"penaltyTime":300.0,"penaltyTimeRemaining":225.0,"reason":"LEAVER_BUSTED"},"searchState":"Searching"}
            try
            {
                UpdateRequest();
                String url = "https://127.0.0.1:" + this.port + "/lol-lobby/v2/lobby/matchmaking/search-state";
                request.AddHeader("Authorization", "Basic " + this.auth);
                string response = request.Get(url).ToString();
                //Logger.Log("--- mmsearch state response: \n\r" + response);
                if (response.Contains("QUEUE_DODGER") || response.Contains("LEAVER_BUSTED"))
                {
                    var x = JObject.Parse(response);
                    if (x == null)
                        return 4;
                    var lpData = x["lowPriorityData"];
                    if (lpData == null || lpData["penaltyTimeRemaining"] == null)
                        return 4;
                    return Convert.ToInt32(lpData["penaltyTimeRemaining"].ToString());
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Logger.Log("leaverbuster() error: " + ex.Message);
                return 0;
            }
        }

        public bool InChampSelect()
        {
            try
            {
                string stringUrl = "https://127.0.0.1:" + this.port + "/lol-champ-select/v1/session";
                UpdateRequest();
                return request.Get(stringUrl).ToString().Contains("action");
            }
            catch
            {
                return false;
            }
        }

        public string CreateLobby(string type)
        {
            string id = (type == "intro") ? "830" : "850";
            UpdateRequest();
            string url = "https://127.0.0.1:" + this.port + "/lol-lobby/v2/lobby";
            string content = request.Post(url, "{\"queueId\": " + id + "}", "application/json").StatusCode.ToString();
            //Logger.Log(content);
            return content;
        }

        /*public void PickChampion(int ChampionID)
        {
            string statusCode = "";
            System.Threading.Thread.Sleep(2000);
            for (int i = 1; i < 11; i++)
            {
                try
                {
                    string url = "https://127.0.0.1:" + this.port + "/lol-champ-select/v1/session/actions/" + i;
                    UpdateRequest();

                    statusCode = request.Patch(url, "{\"actorCellId\": 0, \"championId\": " + ChampionID + ", \"completed\": true, \"id\": " + i + ", \"isAllyAction\": true, \"type\": \"string\"}", "application/json").ToString();

                    //Logger.Log("STATUSCODE i:");
                }
                catch (Exception x)
                {
                    Logger.Log($"Error picking champion in action {i}: {statusCode}");
                    Logger.Log($"ERROR: {x.Message}");
                }
            }
        }*/
        public string GetSessionJsonString()
        {
            try
            {
                //System.Threading.Thread.Sleep(2000);
                UpdateRequest();
                String url = "https://127.0.0.1:" + this.port + "/lol-champ-select/v1/session";
                request.AddHeader("Authorization", "Basic " + this.auth);
                string statusCode = request.Get(url).ToString();
                return statusCode;
            }
            catch
            {
                return null;
            }
        }

        public bool SetSummonerID()
        {
            try
            {
                //System.Threading.Thread.Sleep(2000);
                UpdateRequest();
                String url = "https://127.0.0.1:" + this.port + "/lol-login/v1/session";
                request.AddHeader("Authorization", "Basic " + this.auth);
                string json = request.Get(url).ToString();
                //Logger.Log("summoner response: " + Environment.NewLine + json);
                var x = JObject.Parse(json);

                if (!x.ContainsKey("state"))
                    return false;
                if (x.Value<string>("state") == "SUCCEEDED")
                {
                    if (!x.ContainsKey("summonerId"))
                        return false;
                    SummonerID = x.Value<long>("summonerId");
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public bool IsInLoginQueue()
        {
            try
            {
                //System.Threading.Thread.Sleep(2000);
                UpdateRequest();
                String url = "https://127.0.0.1:" + this.port + "/lol-login/v1/session";
                request.AddHeader("Authorization", "Basic " + this.auth);
                string json = request.Get(url).ToString();
                //Logger.Log("summoner response: " + Environment.NewLine + json);
                var x = JObject.Parse(json);

                if (!x.ContainsKey("state"))
                    return false;
                if (x.Value<string>("state") == "SUCCEEDED")
                {
                    if (x.ContainsKey("isInLoginQueue"))
                        return x.Value<bool>("isInLoginQueue");
                    return false;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public int SelectedChamp()
        {
            try
            {
                //System.Threading.Thread.Sleep(2000);
                UpdateRequest();
                String url = "https://127.0.0.1:" + this.port + "/lol-champ-select/v1/session";
                request.AddHeader("Authorization", "Basic " + this.auth);
                string json = request.Get(url).ToString();
                //Logger.Log(json);
                var x = JObject.Parse(json);
                if (x == null)
                    return 0;
                var myTeam = x["myTeam"];
                if (x["actions"] == null)
                    return 0;
                var actions = x["actions"][0];
                int actorCellId = -1;
                for (int i = 0; i < 5; i++)
                {
                    if (myTeam[i].Value<long>("summonerId") == SummonerID)
                        actorCellId = myTeam[i].Value<int>("cellId");
                }
                if (actorCellId == -1)
                    return 0;
                for (int i = 0; i < 10; i++)
                {
                    if (actions[i].Value<int>("actorCellId") == actorCellId)
                    {
                        if (actions[i].Value<bool>("completed"))
                        {
                            return actions[i].Value<int>("championId");
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("ischampselect error: " + ex.Message);
                return 0;
            }
            return 0;
        }

        public string PickChampion(int ChampionID = 22)
        {
            try
            {
                //System.Threading.Thread.Sleep(2000);
                UpdateRequest();
                String url = "https://127.0.0.1:" + this.port + "/lol-champ-select/v1/session";
                request.AddHeader("Authorization", "Basic " + this.auth);
                string json = request.Get(url).ToString();
                var x = JObject.Parse(json);
                /*       for action in cs['actions'][0]:
if action['actorCellId'] != actorCellId:
continue

if action['championId'] == 0:
championId = championsPrio[championIdx]
championIdx = championIdx + 1

url = '/lol-champ-select/v1/session/actions/%d' % action['id']
data = {'championId': championId}

championName = champions[str(championId)]
print('Picking', championName, '(%d)' % championId, '..')

# Pick champion
r = request('patch', url, '', data)
print(r.status_code, r.text)

# Lock champion
if championLock and action['completed'] == False:
r = request('post', url+'/complete', '', data)
print(r.status_code, r.text)*/
                if (x == null)
                    return "x = null";
                var myTeam = x["myTeam"];
                if (x["actions"] == null)
                    return "actions = null";
                var actions = x["actions"][0];
                int actorCellId = -1;
                for (int i = 0; i < 5; i++)
                {
                    if (myTeam[i].Value<long>("summonerId") == SummonerID)
                        actorCellId = myTeam[i].Value<int>("cellId");
                }
                if (actorCellId == -1)
                    return "actorCellId = -1";
                for (int i = 0; i < 10; i++)
                {
                    if (actions[i].Value<int>("actorCellId") == actorCellId)
                    {
                        if (actions[i].Value<bool>("completed") == false)
                        {
                            if (actions[i].Value<int>("championId") == 0)
                            {

                                int id = actions[i].Value<int>("id");
                                url = "https://127.0.0.1:" + this.port + "/lol-champ-select/v1/session/actions/" + id;
                                UpdateRequest();

                                string statusCode = request.Patch(url, "{\"championId\": " + ChampionID + "}", "application/json").ToString();
                                //Logger.Log("*i: " + i.ToString() + " - " + statusCode);
                                UpdateRequest();
                                url += "/complete";
                                statusCode = request.Post(url, "{\"championId\": " + ChampionID + "}", "application/json").ToString();
                                //Logger.Log("complete response: " + statusCode);
                                return statusCode;
                            }
                            else
                            {
                                return "completed = false1";
                            }
                        }
                        else
                        {
                            return "completed = false2";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("pickchampion error: " + ex.Message);
                return "pickchampion error";
            }
            return "pickchampion e-2";
        }
        public void PickChampionByName(string name)
        {
            this.PickChampion(Champions.GetIdByChamp(name));
        }

        public void AcceptQueue()
        {
            string url = "https://127.0.0.1:" + this.port + "/lol-matchmaking/v1/ready-check/accept";
            UpdateRequest();
            request.Post(url);
        }

        /*public void SingOut()
        {
            InputHelper.LeftClick(1736, 110, 150);
            InputHelper.LeftClick(1000, 587, 150);
            Logger.Write("Logged out");
        }

        public bool isLogged()
        {
            //Logger.Log(TextHelper.TextExists(447, 111, 505, 135, "HOME"));
            return TextHelper.TextExists(447, 111, 505, 135, "HOME");
        }

        public bool WrongCredentials()
        {

            //Console.Write(TextHelper.TextExists(586, 438, 100, 40, "match"));
            return TextHelper.TextExists(586, 438, 100, 40, "match");
        }


        public void GetLoginData()
        {
            Accounts acc = new Accounts();

            InputHelper.LeftClick(388, 464, 150);
            //BotHelper.Wait(2000);
            SendKeys.SendWait(acc.login);
            //BotHelper.Wait(2000);
            InputHelper.LeftClick(388, 527, 150);
            //BotHelper.Wait(2000);
            SendKeys.SendWait(acc.password);
            //BotHelper.Wait(2000);
            InputHelper.LeftClick(500, 680, 150);
        }*/

        public void ReadProcess()
        {
            ManagementClass managementClass = new ManagementClass("Win32_Process");
            foreach (ManagementBaseObject manageBaseobj in managementClass.GetInstances())
            {
                ManagementObject manageObj = (ManagementObject)manageBaseobj;
                if (manageObj["Name"].Equals("LeagueClientUx.exe"))
                {
                    foreach (object obj in Regex.Matches(manageObj["CommandLine"].ToString(), RegexPattern, RegexOption))
                    {
                        Match match = (Match)obj;
                        if (!string.IsNullOrEmpty(match.Groups["port"].ToString()))
                        {
                            this.port = int.Parse(match.Groups["port"].ToString());
                        }
                        else if (!string.IsNullOrEmpty(match.Groups["token"].ToString()))
                        {
                            string riot_pass = match.Groups["token"].ToString().Replace("=", "");
                            this.auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("riot:" + riot_pass));
                        }
                    }
                }
            }

            if (this.auth == null)
            {
                Logger.Log("Can't find LeagueClientUx.exe");
            }
        }
    }
}
