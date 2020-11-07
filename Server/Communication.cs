using Server.Models;
using SimpleTCP;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public class Communication
    {
        const int TIMER_INTERVAL = 15000;
        static System.Threading.Timer MainTimer = null;
        static Thread CloseThread = null;
        static SimpleTcpClient Client = null;
        static Server Server { get; set; }
        public Communication(Server server)
        {
            string ip = System.IO.File.ReadAllText("local.txt").TrimEnd('\n');
            Server = server;
            Server.ClientID = 0;
            StartServer(ip, Server.PORT);
            int id = GetAvailableClientID();
            if (id == 0)
            {
                CloseInSeconds("Can't find available ID in manager.");
            }
            else
            {
                Server.ClientID = id;
                Server.SetInfoText($"Registering ID {id}...");
                if (SendMessage(id, "register") == "true")
                    Server.SetInfoText($"Success.");
                else
                    CloseInSeconds("Can't register ID.");
            }
            if (Server.ClientID != 0)
            { // id set
                new ClientApi(server);
                ClientApi.SetState(ClientStates.Idle);
                KillMainTimer();
                MainTimer = new System.Threading.Timer(TimerCallback, null, TIMER_INTERVAL, Timeout.Infinite);
            }
        }
        public void TimerCallback(object state)
        {
            try
            {
                GetMainCommand();
                MainTimer.Change(TIMER_INTERVAL, Timeout.Infinite);
            }
            catch (Exception x)
            {
                Logger.Log("error in TimerCallback: " + x.Message);
            }
        }
        public static void SendShutdownSignal()
        {
            if (Client != null)
            {
                try
                {
                    Client.WriteLine(Server.ClientID.ToString() + "|shutdown|0");
                }
                catch { }
            }
        }
        public static void KillCloseThread()
        {
            if (CloseThread != null)
            {
                CloseThread.Abort();
                CloseThread = null;
            }
        }
        public static void KillMainTimer()
        {
            if (MainTimer != null)
            {
                MainTimer.Dispose();
                MainTimer = null;
            }
        }
        public static void CloseInSeconds(string msg, int time = 5)
        {
            KillCloseThread();
            CloseThread = new Thread(() =>
            {
                for (int t = time; t > 0; t--)
                {
                    Server.SetInfoText($"{msg} Closing the application in {t}...");
                    Thread.Sleep(999);
                }
                Application.Exit();
            });
            CloseThread.Start();
        }
        public void StartServer(string ip = "127.0.0.1", int port = 53399)
        {
            try
            {
                if (Client == null)
                    Client = new SimpleTcpClient().Connect(ip, port);
                else
                {
                    Client.Disconnect();
                    Client.Dispose();
                    Client = new SimpleTcpClient().Connect(ip, port);
                }
            }
            catch (Exception e)
            {
                Server.SetInfoText($"ServerERR: {e.Message}");
            }
        }

        public static string SendMessage(int clientID, string msg, string arg = "0")
        {
            if (Client == null)
                Client = new SimpleTcpClient().Connect("127.0.0.1", Server.PORT);
            try
            {
                var replyMsg = Client.WriteLineAndGetReply(clientID.ToString() + "|" + msg + "|" + arg, TimeSpan.FromSeconds(3));
                if (replyMsg != null)
                    return replyMsg.MessageString.TrimEnd('\u0013');
            }
            catch
            {
                Client = new SimpleTcpClient().Connect("127.0.0.1", Server.PORT);
            }

            return "0|0|0";
        }
        public static void SendMessageWithoutReply(int clientID, string msg, string arg = "0")
        {
            if (Client == null)
                Client = new SimpleTcpClient().Connect("127.0.0.1", Server.PORT);
            try
            {
                Client.WriteLine(clientID.ToString() + "|" + msg + "|" + arg);
            }
            catch
            {
                Client = new SimpleTcpClient().Connect("127.0.0.1", Server.PORT);
            }
        }

        public int GetAvailableClientID()
        {
            if (Client == null)
                return 0;
            var replyMsg = Client.WriteLineAndGetReply("checkID", TimeSpan.FromSeconds(3));
            if (replyMsg != null)
                return Convert.ToInt32(replyMsg.MessageString.TrimEnd('\u0013'));
            return 0;
        }
        public Account GetAccount()
        {
            try
            {
                string reply = SendMessage(Server.ClientID, "account");
                if (reply != null && reply == "no_account")
                    return null;

                string[] split = reply.Split(':');

                Logger.Log("Account information loaded: " + split[0]);
                return new Account(split[0], split[1]);
            }
            catch (Exception x)
            {
                Logger.Log("Can't get account. Details: " + x.Message);
            }
            return null;
        }
        public void GetMainCommand()
        {
            try
            {
                string reply = SendMessage(Server.ClientID, "main", Server.LastInfo);

                switch (reply)
                {
                    case "Start":
                        if (ClientApi.GetState() != ClientStates.Start)
                        {
                            Server.LoadedAccount = GetAccount();
                            if (Server.LoadedAccount == null)
                            {
                                Server.SetInfoText("Can't get account data.");
                            }
                            else
                            {
                                ClientApi.Start();
                            }
                        }
                        break;
                    case "Login": ClientApi.SetState(ClientStates.Login); Logger.Log("Manager said: " + reply); break;
                    case "Stop": ClientApi.Stop(); break;
                    case "shutdown": break;
                }
            }
            catch (Exception x)
            {
                Logger.Log("Can't get commands. Details: " + x.Message);
            }
        }
    }
}
