using InputManager;
using Server.Game;
using Server.GameApi;
using Server.Image;
using Server.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Server
{
    public enum ClientStates
    {
        None,
        Idle,
        Start,
        Login,
        Queue,
        InGame,
    }
    public class ClientApi
    {
        static GameSession GameSession { get; set; }
        static Process LOLProcess;
        static Server Server { get; set; }
        static ClientStates State { get; set; } = ClientStates.None;
        public static bool Restart = false;

        static Timer LoginTimer = null;
        static Timer gameControl = null;
        public static LCU LCU = null;

        static ClientWindow clientWindow = null;
        //static int hWnd = 0;
        //private const int SW_HIDE = 0;
        //private const int SW_SHOW = 5;
        const int LoginTimerInterval = 1000;
        const int QueueTimerInterval = 4000;
        const int Queue2TimerInterval = 2000;
        const int MAX_TRIES = 100;
        private const string ROOT_CLIENT_CLASS = "RCLIENT";
        private const string ROOT_CLIENT_NAME = null;
        private const string CHILD_CLIENT_CLASS = null;
        private const string CHILD_CLIENT_NAME = "Chrome Legacy Windo";
        //static bool newGame = false;
        static int Try = 0;
        static int sel = 0;
        public ClientApi(Server server)
        {
            //newGame = false;
            Server = server;
            State = ClientStates.None;
        }
        public static void StartNewGame()
        {
            sel = 0;
            KillTimers();
            Logger.Log("Killed timers (sng)");
            Try = 0;
            LoginTimer = new Timer(QueueTimerCallback, null, QueueTimerInterval, Timeout.Infinite);
        }
        public static void KillTimers()
        {
            //Logger.Log("kill");
            if (LoginTimer != null)
            {
                LoginTimer.Dispose();
                LoginTimer = null;
            }
        }
        public static void Start()
        {
            try
            {
                if (LOLProcess != null)
                {
                    if (!LOLProcess.HasExited)
                        LOLProcess.Kill();
                    LOLProcess.Dispose();
                    LOLProcess = null;
                }
                GameSession.Control();
                StopLOLClient();
                Thread.Sleep(1000);
                if (State != ClientStates.Start && State != ClientStates.Login)
                {
                    if (gameControl != null)
                        gameControl.Dispose();
                    gameControl = null;
                    LogAndInfo("Starting operation...");
                    Server.SetIndicator(IndicatorStates.Wait);
                    Thread.Sleep(1000);
                    State = ClientStates.Start;
                    LogAndInfo("Starting League of Legends...");
                    StartLOLClient();

                    LoginTimer = new Timer(LoginTimerCallback, null, LoginTimerInterval, Timeout.Infinite);
                    gameControl = new Timer(GameControlCallback, null, 30000, Timeout.Infinite);
                }
            }
            catch (Exception x)
            {
                Logger.Log("Start error: " + x.Message);
            }
        }

        private static void GameControlCallback(object state)
        {
            try
            {
                TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - GameSession.TimerTick);
                if (timeSpan.TotalSeconds > 260)
                {
                    LCU = new LCU();

                    if (GameSession.GetPort() != 0)
                    {
                        try
                        {
                            var bmp = PixelCache.GetScreenshot();
                            bmp.Bitmap.Save("gs1.bmp");
                            if (bmp.GetPixel(485, 352) == Color.FromArgb(59, 45, 24)) // afk
                            {
                                Mouse.Move(485, 352);
                                Thread.Sleep(600);
                                Mouse.ButtonDown(Mouse.MouseKeys.Left);
                                Thread.Sleep(50);
                                Mouse.ButtonUp(Mouse.MouseKeys.Left);
                                Thread.Sleep(300);
                                Mouse.Move(490, 352);
                                Thread.Sleep(100);
                                Mouse.ButtonDown(Mouse.MouseKeys.Left);
                                Thread.Sleep(50);
                                Mouse.ButtonUp(Mouse.MouseKeys.Left);
                                Thread.Sleep(1000);
                            }

                            if (bmp.GetPixel(798, 151) == Color.FromArgb(13, 26, 26)) // main menu
                            {
                                Mouse.Move(798, 151);
                                Thread.Sleep(600);
                                Mouse.ButtonDown(Mouse.MouseKeys.Left);
                                Thread.Sleep(50);
                                Mouse.ButtonUp(Mouse.MouseKeys.Left);
                                Thread.Sleep(300);
                                Mouse.Move(744, 596); // cancel button
                                Thread.Sleep(100);
                                Mouse.ButtonDown(Mouse.MouseKeys.Left);
                                Thread.Sleep(50);
                                Mouse.ButtonUp(Mouse.MouseKeys.Left);
                                Thread.Sleep(3000);
                            }

                            if (bmp.GetPixel(495, 352) == Color.FromArgb(59, 45, 24)
                                || bmp.GetPixel(507, 344) == Color.FromArgb(255, 255, 255))
                            {
                                ListProcesses();
                                GameSession.TimerTick = DateTime.Now.Ticks;
                                Logger.Log("gamesession (3) restart..." + timeSpan.ToString());
                                ClientApi.Stop();
                                Logger.Log("Connection lost. Trying to continue...");
                                Server.SetInfoText("Connection lost. Trying to continue...");
                                Thread.Sleep(5000);
                                ClientApi.Restart = true;
                                ClientApi.Start();
                            }
                            else if (bmp.GetPixel(532, 765) != Color.FromArgb(78, 62, 29))
                            {
                                ListProcesses();
                                GameSession.TimerTick = DateTime.Now.Ticks;
                                Logger.Log("gamesession (1) restart..." + timeSpan.ToString());
                                GameSession.Start = 1;
                                GameSession = null;
                                GameSession = new GameSession(Server, Champions.RandomChamp());
                            }
                            bmp.Dispose();
                        }
                        catch (Exception x)
                        {
                            Logger.Log("gamesession (1) image error..." + x.Message);
                        }
                    }
                    else //if(LCU != null && LCU.LeaverBuster() == 0)
                    {
                        bool flag = false;
                        try
                        {
                            var bmp = PixelCache.GetScreenshot();
                            bmp.Bitmap.Save("kill1.bmp");
                            if (bmp.GetPixel(425, 362) == Color.FromArgb(205, 190, 145))
                            {
                                GameSession.TimerTick = DateTime.Now.Ticks;
                                ListProcesses();
                                flag = true;
                                while (GameSession.GetPort() == 0)
                                {
                                    GameSession.Control();
                                    TryToContinue();
                                    LogAndInfo("waiting for reconnect...");
                                }
                                LogAndInfo("Reconnecting...");
                                State = ClientStates.InGame;
                                Communication.SendMessageWithoutReply(Server.ClientID, "state", "ingame");
                                StartGameSession(Champions.RandomChamp());
                                KillTimers();
                            }
                            else if (bmp.GetPixel(507, 344) == Color.FromArgb(255, 255, 255)
                               || bmp.GetPixel(525, 125) == Color.FromArgb(17, 17, 17))
                            {
                                ListProcesses();
                                GameSession.TimerTick = DateTime.Now.Ticks;
                                Logger.Log("kill (3) restart..." + timeSpan.ToString());
                                ClientApi.Stop();
                                Logger.Log("Connection lost. Trying to continue...");
                                Server.SetInfoText("Connection lost. Trying to continue...");
                                Thread.Sleep(5000);
                                ClientApi.Restart = true;
                                ClientApi.Start();
                            }
                            bmp.Dispose();
                        }
                        catch (Exception x)
                        {
                            Logger.Log("kill (1) image error..." + x.Message);
                        }
                        if (!flag)
                        {
                            //425,362 = 205, 190, 145
                            GameSession.TimerTick = DateTime.Now.Ticks;
                            Logger.Log("kill1 " + timeSpan.ToString());
                            Stop();
                            Logger.Log("Restarting bot...");
                            Server.SetInfoText("Restarting bot...");
                            Thread.Sleep(5000);
                            if (Server.LoadedAccount != null)
                                Start();
                            else
                                Logger.Log("no loaded account");
                        }
                    }
                }
                else
                {
                    Logger.Log("check - OK: bot is working... (1)");
                }
            }
            catch (Exception x)
            {
                Logger.Log("check - FAIL: gameControlTimer error: " + x.Message);
            }
            finally
            {
                TimerChange(gameControl, 60000);
            }
        }

        public static void Stop()
        {
            KillTimers();
            Logger.Log("Killed timers (so)");
            if (State != ClientStates.Idle)
            {
                LogAndInfo("Stopping operation...");
                Server.SetIndicator(IndicatorStates.Off);
                State = ClientStates.Idle;
                KillAllLeagueClientProcesses();
            }
        }

        private static Rectangle WaitForPasswordBox(ClientWindow clientWindow)
        {
            try
            {
                //Logger.Log("waitfor");
                while (NativeMethods.Exists(clientWindow.Handle) && clientWindow.Status != ClientStatus.OnLoginScreen)
                {
                    Thread.Sleep(1000);
                    clientWindow.RefreshStatus();
                    Logger.Log("waiting");
                }
                //Logger.Log("wok");
            }
            catch (Exception ex)
            {
                Logger.Log("wait fail: " + ex.Message);
            }

            return clientWindow.PasswordBox;
        }
        private static void LoginTimerCallback(object state)
        {
            //Logger.Log("login timer");
            try
            {
                clientWindow = GetClientWindow();

                if (clientWindow != null)
                {
                    //LogAndInfo("clientwindow = " + clientWindow?.Name);
                    State = ClientStates.Login;

                    LogAndInfo("Waiting for login screen...");
                    var passwordBox = WaitForPasswordBox(clientWindow);
                    NativeMethods.Focus(clientWindow.InnerWindow.Handle);

                    if (passwordBox != Rectangle.Empty)
                    {
                        LogAndInfo("Entering password...");
                        NativeMethods.Focus(clientWindow.InnerWindow.Handle);
                        if (!EnterPassword(clientWindow, new string[] { Server.LoadedAccount.Username, Server.LoadedAccount.Password }))
                        {
                            StopLOLClient();
                        }
                        else
                        {
                            if (CheckLockfile())
                            {
                                LogAndInfo("Login successful. Entering the queue...");
                                Try = 0;
                                sel = 0;
                                Communication.SendMessageWithoutReply(Server.ClientID, "state", "queue");
                                State = ClientStates.Queue;
                                if (LoginTimer != null)
                                    LoginTimer.Dispose();
                                LoginTimer = new Timer(QueueTimerCallback, null, QueueTimerInterval, Timeout.Infinite);
                            }
                            else
                                TimerChange(LoginTimer, LoginTimerInterval);

                        }
                    }
                }
                else
                {
                    try
                    {
                        Try++;
                        if (Try > MAX_TRIES)
                        {
                            State = ClientStates.Idle;

                            LogAndInfo("Can't log in: Reached to max. number of tries.");
                            KillTimers();
                        }
                    }
                    catch (Exception x)
                    {
                        LogAndInfo("ex: " + x.Message);
                    }
                    if (LoginTimer != null)
                        TimerChange(LoginTimer, LoginTimerInterval);
                    else
                        LoginTimer = new Timer(LoginTimerCallback, null, LoginTimerInterval, Timeout.Infinite);
                    //LogAndInfo("Trying again... " + Try.ToString());
                }
            }
            catch (Exception x)
            {
                Logger.Log("login timer error: " + x.Message);
                TimerChange(LoginTimer, LoginTimerInterval);
            }
        }
        private static void QueueTimerCallback(object state) // check main screen
        {
            if (LOLProcess != null && !LOLProcess.HasExited)
            {
                Try++;
                if (Try > MAX_TRIES)
                {
                    State = ClientStates.Idle;

                    LogAndInfo("Can't get in queue: Reached to max. number of tries.");
                    KillTimers();
                }
                else
                {
                    LogAndInfo("Waiting for the main screen..."); //);
                    TimerChange(LoginTimer, QueueTimerInterval);
                }
            }
            else
            {
                /*if (LOLProcess != null)
                {
                    LOLProcess.Dispose();
                    LOLProcess = null;
                }*/
                GameSession.TimerTick = DateTime.Now.Ticks;
                KillTimers();
                Logger.Log("Killed timers (q1)");
                LogAndInfo("Preparing...");
                Try = 0;
                sel = 0;

                Thread.Sleep(GameSession.RandomTimeGenerator(5000));
                try
                {
                    LCU = new LCU();
                    while (LCU.SummonerID == 0)
                    {
                        KillEdge();
                        Thread.Sleep(GameSession.RandomTimeGenerator(15000));
                        if (GameSession.GetPort() != 0)
                        {
                            GameSession.TimerTick = DateTime.Now.Ticks;
                            LogAndInfo("reconnecting");
                            Restart = true;
                            break;
                        }
                        LogAndInfo("Can't set summonerID");
                        Try = -1;
                        LCU = new LCU();
                    }
                    Thread.Sleep(GameSession.RandomTimeGenerator(15000));
                    while (LCU.IsInLoginQueue())
                    {
                        GameSession.TimerTick = DateTime.Now.Ticks;
                        KillEdge();
                        LogAndInfo("Still in login queue.");
                        Thread.Sleep(GameSession.RandomTimeGenerator(15000));
                    }
                }
                catch (Exception x)
                {
                    LogAndInfo("qu error: " + x.Message);
                }
                KillEdge();
                if (!Restart)
                {
                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            TryToContinue();
                        }
                        //ok-ok-ok
                        Thread.Sleep(GameSession.RandomTimeGenerator(15000));
                        LogAndInfo("Creating lobby...");
                        string re = LCU.CreateLobby("intro");
                        while (re != "OK" && re != "423")
                        {
                            LCU = new LCU();
                            Thread.Sleep(GameSession.RandomTimeGenerator(5000));
                            re = LCU.CreateLobby("intro");
                        }
                        Server.SetIndicator(IndicatorStates.Wait);

                        LoginTimer = new Timer(Queue2TimerCallback, null, Queue2TimerInterval, Timeout.Infinite);
                    }
                    catch (Exception x)
                    {
                        Logger.Log("!res err: " + x.Message);
                    }
                }
                else
                {
                    while (GameSession.GetPort() == 0)
                    {
                        TryToContinue();
                        LogAndInfo("waiting for reconnect...");
                    }
                    LogAndInfo("Reconnecting...");
                    State = ClientStates.InGame;
                    Communication.SendMessageWithoutReply(Server.ClientID, "state", "ingame");
                    StartGameSession(Champions.RandomChamp());
                    KillTimers();
                    Restart = false;
                }
            }
        }

        private static void Queue2TimerCallback(object state) // enter game queue
        {
            //Logger.Log("q2timer");
            try
            {
                /*var process = GetProcessByName("League of Legends");
                if (process == null)*/
                int port = GameSession.GetPort();
                if (port == 0)
                {
                    //Logger.Log("port");
                    if (LCU == null || LCU.SummonerID == 0)
                        LCU = new LCU();
                    //Logger.Log("sumo");
                    Try++;
                    int lb = LCU.LeaverBuster();
                    switch (Try)
                    {
                        case 1: LogAndInfo("Starting queue..."); LCU.StartQueue(); break;
                        case 2:
                            if (lb > 0)
                            {
                                GameSession.TimerTick = DateTime.Now.Ticks;
                                LogAndInfo("Leaverbuster detected. Trying again in " + lb.ToString() + " seconds...");
                                Try = 1;
                                if (lb > 4)
                                    Thread.Sleep(4000);
                                LCU = new LCU();
                            }
                            else
                            {
                                LCU.StartQueue();
                                Logger.Log("No Leaverbuster.");
                                Try = 6;
                            }
                            break;
                        default:
                            if (Try > 90 && !LCU.InChampSelect() && port == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    TryToContinue();
                                }
                                LogAndInfo("Can't find match: Reached to max. number of tries. Restarting queue...");
                                Try = 0;
                                sel = 0;
                            }
                            else if (port != 0)
                            {
                                LogAndInfo("Starting game (3) session... " + sel + " - p: " + port);
                                State = ClientStates.InGame;
                                Communication.SendMessageWithoutReply(Server.ClientID, "state", "ingame");
                                StartGameSession(sel);
                                KillTimers();
                            }
                            else if (Try > 5 && !LCU.InChampSelect())
                            {
                                if (Try > 30 && Try % 5 == 0)
                                {
                                    try
                                    {
                                        var bmp = PixelCache.GetScreenshot();
                                        bmp.Bitmap.Save("gs5.bmp");
                                        if (bmp.GetPixel(425, 362) == Color.FromArgb(205, 190, 145))
                                        {
                                            ListProcesses();
                                            GameSession.TimerTick = DateTime.Now.Ticks;
                                            Logger.Log("gamesession (5) restart...");
                                            Logger.Log("Game crashed. Trying to continue...");
                                            Server.SetInfoText("Game crashed. Trying to continue...");
                                            Thread.Sleep(5000);
                                            TryToContinue();
                                        }
                                    }
                                    catch (Exception x)
                                    {
                                        Logger.Log("gs5 error: " + x.Message);
                                        Logger.Log("Screenshot error. Trying to continue...");
                                        Server.SetInfoText("Screenshot error. Trying to continue...");
                                        TryToContinue();
                                    }
                                }
                                LogAndInfo("Accepting match.");
                                LCU.AcceptQueue();
                                //Logger.Log("acc macc: " + LCU?.auth?.ToString() + " kacc: " + port);
                            }
                            break;
                    }

                    if (LoginTimer != null)
                    {
                        GameSession.TimerTick = DateTime.Now.Ticks;
                        if (LCU != null && !LCU.InChampSelect())
                            TimerChange(LoginTimer, QueueTimerInterval);
                        else
                        {
                            sel = LCU.SelectedChamp();
                            int champ = Champions.RandomChamp();
                            try
                            {
                                LCU = new LCU();
                                if (sel == 0 && LCU.InChampSelect())
                                {
                                    LogAndInfo("Picking champion...");
                                    LCU.PickChampion(champ);
                                    Thread.Sleep(500);
                                    while (LCU.SelectedChamp() == 0)
                                    {
                                        LCU.AcceptQueue();
                                        LogAndInfo("Couldn't pick " + Champions.GetChampById(champ) + ". Trying again...");
                                        LCU = new LCU();
                                        Thread.Sleep(3000);
                                        champ = Champions.RandomChamp();
                                        Logger.Log(LCU.PickChampion(champ));
                                    }
                                    if (port == 0)
                                    {
                                        LogAndInfo("picked champion");
                                        TimerChange(LoginTimer, QueueTimerInterval);
                                    }
                                }
                                else
                                {
                                    sel = LCU.SelectedChamp();
                                    if (sel != 0)
                                    {
                                        //LogAndInfo("p: " + port);
                                        Try = 6;
                                        champ = sel;
                                        LogAndInfo("Picked champion: " + Champions.GetChampById(champ));
                                        Thread.Sleep(1000);
                                        if (port == 0)
                                        {
                                            TimerChange(LoginTimer, QueueTimerInterval);
                                        }
                                        else
                                        {
                                            LogAndInfo("Starting game (1) session...");
                                            State = ClientStates.InGame;
                                            Communication.SendMessageWithoutReply(Server.ClientID, "state", "ingame");
                                            StartGameSession(champ);
                                            KillTimers();
                                        }
                                    }
                                }
                            }
                            catch (Exception x)
                            {
                                Logger.Log("pick chmap exc: " + x.Message);
                            }
                        }
                    }
                }
                else
                {
                    LogAndInfo("Starting game (2) session...");
                    State = ClientStates.InGame;
                    Communication.SendMessageWithoutReply(Server.ClientID, "state", "ingame");
                    StartGameSession((sel != 0) ? sel : 22);
                    KillTimers();
                }
                /*
                else if(Try > 20)
                {
                    LogAndInfo("game is fucked - restarting");
                    var process = ClientApi.GetProcessByName("League of Legends");
                    if (process != null)
                    {
                        var x = NativeMethods.GetAllTcpConnections();
                        foreach (var a in x)
                        {                            
                            if (process.Id == a.owningPid)
                            {
                                process.Kill();
                            }
                        }
                    }
                    TryToContinue();
                    State = ClientStates.InGame;
                    Communication.SendMessageWithoutReply(Server.ClientID, "state", "ingame");
                    StartGameSession(Champions.RandomChamp());
                    KillTimers();
                }*/
            }
            catch (Exception x)
            {
                Logger.Log("exc");
                try
                {
                    var bmp = PixelCache.GetScreenshot();
                    bmp.Bitmap.Save("q2.bmp");
                    bmp.Dispose();
                }
                catch (Exception a)
                {
                    Logger.Log("q2 login timer (1) image error..." + a.Message);
                }
                if (Try > 0)
                    Try--;
                Server.SetInfoText("q2 Login timer error");
                if (LoginTimer != null)
                    TimerChange(LoginTimer, QueueTimerInterval);
                Logger.Log("restarting...");
                Logger.Log("error: " + x.Message);
                Logger.Log(x.StackTrace);
            }
        }
        static void StartGameSession(int champ)
        {
            GameSession.TimerTick = DateTime.Now.Ticks;
            Logger.Log("startgamesession: " + champ.ToString());
            if (GameSession != null && GameSession.GameState.Started != true)
            {
                Logger.Log("ok");
                GameSession = null;
            }
            GameSession = new GameSession(Server, champ);
        }

        private static bool EnterPassword(ClientWindow clientWindow, string[] par)
        {
            try
            {
                //hWnd = clientWindow.Handle.ToInt32();
                NativeMethods.Focus(clientWindow.InnerWindow.Handle);
                //Logger.Log("hWnd: " + hWnd.ToString());
                Thread.Sleep(1000);
                //NativeMethods.ShowWindow(hWnd, SW_HIDE);
                Thread.Sleep(5000);

                Rectangle userBox = clientWindow.UsernameBox;
                Rectangle passwordBox = clientWindow.PasswordBox;
                NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, userBox.Left + passwordBox.Width / 2, userBox.Top + userBox.Height / 2);
                NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, userBox.Left + passwordBox.Width / 2, userBox.Top + userBox.Height / 2);

                for (int i = 0; i < 20; i++)
                {
                    NativeMethods.SendKey(clientWindow.InnerWindow.Parent.Handle, VKey.BACK);
                    NativeMethods.SendKey(clientWindow.InnerWindow.Parent.Handle, VKey.DELETE);
                }
                NativeMethods.SendText(clientWindow.InnerWindow.Parent.Handle, par[0]);

                NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, passwordBox.Left + passwordBox.Width / 2, passwordBox.Top + passwordBox.Height / 2);
                NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, passwordBox.Left + passwordBox.Width / 2, passwordBox.Top + passwordBox.Height / 2);

                for (int i = 0; i < 20; i++)
                {
                    NativeMethods.SendKey(clientWindow.InnerWindow.Parent.Handle, VKey.BACK);
                    NativeMethods.SendKey(clientWindow.InnerWindow.Parent.Handle, VKey.DELETE);
                }
                NativeMethods.SendText(clientWindow.InnerWindow.Parent.Handle, par[1]);

                for (int i = 0; i < 20; i++)
                {
                    NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, clientWindow.DialogBox.Left + clientWindow.DialogBox.Width / 2, clientWindow.DialogBox.Top + clientWindow.DialogBox.Height / 2 - (int)(i * 2 * clientWindow.Scale));
                }
                Thread.Sleep(10000);

                for (int i = 0; i < 4; i++)
                {
                    MouseOperations.SetCursorPosition(500 + GameSession.RandomTimeGenerator(50), 350 + GameSession.RandomTimeGenerator(50));
                    Thread.Sleep(500);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    Thread.Sleep(GameSession.RandomTimeGenerator(24));
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                }
                Thread.Sleep(6000);
                Server.SetInfoText("Checking login status...");
                //NativeMethods.ShowWindow(hWnd, SW_HIDE);
                //NativeMethods.ShowWindow(clientWindow.InnerWindow.Handle.ToInt32(), SW_HIDE);
                try
                {
                    if (!CheckLockfile())
                    {
                        var newCapture = clientWindow.InnerWindow.Capture();
                        for (int i = 0; i < 2; i++)
                        {
                            NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, newCapture.Width / 2, newCapture.Height / 2);
                            NativeMethods.SendKey(clientWindow.InnerWindow.Parent.Handle, VKey.END);
                        }
                        Thread.Sleep(1000);
                        /*newCapture = clientWindow.InnerWindow.Capture();
                        Bitmap output = new Bitmap(newCapture);
                        using (var graphics = Graphics.FromImage(output)){*/
                        int baseCaptureWidth = newCapture.Width / 2 - newCapture.Width / 50;
                        int baseCaptureHeight = newCapture.Height / 2 + newCapture.Height / 40;
                        for (int i = 0; i < 10; i++)
                        {
                            /* Rectangle tang = new Rectangle(baseCaptureWidth - i * (newCapture.Width / 90), baseCaptureHeight + i * (newCapture.Height / 45), 7, 3);

                             graphics.DrawRectangle(new Pen(Color.Gold, 1), tang);
                             tang = new Rectangle(baseCaptureWidth - i * (newCapture.Width / 90), baseCaptureHeight + i * (newCapture.Height / 45) + (newCapture.Height / 6), 7, 3);

                             graphics.DrawRectangle(new Pen(Color.AliceBlue, 1), tang);*/

                            NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, baseCaptureWidth - i * (newCapture.Width / 90), baseCaptureHeight + i * (newCapture.Height / 45) + (newCapture.Height / 6));
                            NativeMethods.SendMouseClick(clientWindow.InnerWindow.Handle, baseCaptureWidth - i * (newCapture.Width / 90), baseCaptureHeight + i * (newCapture.Height / 45));
                        }/*
                            Util.SaveDebugImage(output, @"okk.png");*/
                        clientWindow.RefreshStatus();
                        // }
                    }
                }
                catch (Exception x)
                {
                    Server.SetInfoText("Can't enter password.");
                    Logger.Log("Can't enter password. Details: " + x.Message);
                }
            }
            catch (Exception x)
            {
                Server.SetInfoText("Couldn't log in with the provided credentials.");
                Logger.Log("Couldn't log in with the provided credentials. Details: " + x.Message);
                StopLOLClient();
            }

            Thread.Sleep(6000);
            if (!CheckLockfile())
            {
                return false;
            }
            return true;
            /*Thread.Sleep(30000);
            if (!Util.CheckLockfile())
            {
                Util.KillAllLeagueClientProcesses();
                Message.Error($"Hesap bilgileri programa aktarılamadı. (0x90)");
            }*/
            //clientWindow.InnerWindow.Parent.SendKey(VirtualKeyCode.RETURN);

            //Logger.Info("Successfully entered password (well, hopefully)");
        }

        public static void KillAllLeagueClientProcesses()
        {
            KillAllProcessesContaining("Riot");
            KillAllProcessesContaining("League");
            try
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if ((p.MainWindowTitle.Contains("Failed")
                        || p.MainWindowTitle.Contains("Error")
                        || p.MainWindowTitle.Contains("Hata")
                        || p.MainWindowTitle.Contains("League")
                        || p.MainWindowTitle.Contains("cmd.exe")
                        || p.MainWindowTitle.Contains("Edge")
                        || p.MainWindowTitle.Contains("Chrome"))
                            && !p.ProcessName.Contains("Device"))
                    {
                        Logger.Log("closing window: (pid: " + p.Id + ") " + p.MainWindowTitle + " - " + p.ProcessName);
                        p.Kill();
                    }
                }
            }
            catch (Exception x)
            {
                Logger.Log("error closing windows: " + x.Message);
            }
        }

        private static void KillAllProcessesContaining(string name)
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName.Contains(name))
                    p.Kill();
            }
        }

        public static bool CheckLeagueClientProcess()
        {
            return
                CheckProcessesContaining("LeagueClient")
                || CheckProcessesContaining("RiotClient");
        }

        public static Process GetProcessByName(string name)
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName.Contains(name)) return p;
            }
            return null;
        }
        public static bool CheckProcessesContaining(string name)
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName.Contains(name)) return true;
            }
            return false;
        }

        public static bool CheckLockfile()
        {
            if (File.Exists(Path.Combine(Folders.LOLPath, "lockfile"))) return true;
            return false;
        }

        public static List<Window> GetWindows(string className, string windowName)
        {
            //Logger.Debug($"Trying to find window handles for {{ClassName={className ?? "null"},WindowName={windowName ?? "null"}}}");

            var hwnd = IntPtr.Zero;
            var windows = new List<Window>();

            while ((hwnd = NativeMethods.FindWindowEx(IntPtr.Zero, hwnd, className, windowName)) != IntPtr.Zero)
            {
                Window window = new Window(hwnd, IntPtr.Zero, className, windowName);
                windows.Add(window);
            }

            //Logger.Debug($"Found {windows.Count} windows");

            return windows;
        }
        private static ClientWindow GetClientWindow()
        {
            List<Window> windows = GetWindows(ROOT_CLIENT_CLASS, ROOT_CLIENT_NAME);

            foreach (var window in windows)
            {
                Window child = window.FindChildRecursively(CHILD_CLIENT_CLASS, CHILD_CLIENT_NAME);

                if (child == null)
                {
                    continue;
                }

                ClientWindow clientWindow = ClientWindow.FromWindow(window, child);

                if (clientWindow.IsMatch)
                {
                    return clientWindow;
                }
            }

            return null;
        }
        public static ClientStates GetState()
        {
            return State;
        }
        public static void TryToContinue()
        {
            Random rn = new Random();
            if (GameSession.LOLHandle == IntPtr.Zero && LOLProcess != null && NativeMethods.IsWindow(LOLProcess.Handle) && !LOLProcess.HasExited)
                NativeMethods.Focus(LOLProcess.Handle);
            Thread.Sleep(GameSession.RandomTimeGenerator(6000));
            MouseOperations.SetCursorPosition(712 + rn.Next(-2, 2), 147 + rn.Next(-2, 2));
            Thread.Sleep(GameSession.RandomTimeGenerator(100));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            Thread.Sleep(GameSession.RandomTimeGenerator(25));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            Thread.Sleep(GameSession.RandomTimeGenerator(1000));
            MouseOperations.SetCursorPosition(511 + rn.Next(-3, 3), 488 + rn.Next(-3, 4));
            Thread.Sleep(GameSession.RandomTimeGenerator(100));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            Thread.Sleep(GameSession.RandomTimeGenerator(25));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            //reconnect
            Thread.Sleep(GameSession.RandomTimeGenerator(1000));
            MouseOperations.SetCursorPosition(418 + rn.Next(-3, 3), 364 + rn.Next(-3, 4));
            Thread.Sleep(GameSession.RandomTimeGenerator(100));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            Thread.Sleep(GameSession.RandomTimeGenerator(25));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            //reconnect
            Thread.Sleep(GameSession.RandomTimeGenerator(2000));
            MouseOperations.SetCursorPosition(514 + rn.Next(-5, 5), 588 + rn.Next(-5, 5));
            Thread.Sleep(GameSession.RandomTimeGenerator(100));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            Thread.Sleep(GameSession.RandomTimeGenerator(25));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            // HERO PICK
            Thread.Sleep(GameSession.RandomTimeGenerator(2000));
            MouseOperations.SetCursorPosition(510 + rn.Next(-2, 2), 386 + rn.Next(-2, 2));
            Thread.Sleep(GameSession.RandomTimeGenerator(100));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            Thread.Sleep(GameSession.RandomTimeGenerator(25));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            Thread.Sleep(GameSession.RandomTimeGenerator(1000));
            MouseOperations.SetCursorPosition(507 + rn.Next(-3, 3), 619 + rn.Next(-2, 2));
            Thread.Sleep(GameSession.RandomTimeGenerator(100));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            Thread.Sleep(GameSession.RandomTimeGenerator(25));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            // HERO PICK
        }
        public static void SetState(ClientStates state)
        {
            State = state;
        }
        static void LogAndInfo(string log)
        {
            Logger.Log(log);
            Server.SetInfoText(log);
        }

        public static void StartLOLClient()
        {
            if (LOLProcess == null && !CheckProcessesContaining("LeagueClient"))
            {
                var psi = new ProcessStartInfo(Folders.LeagueClientExe,
                    "--headless --disable-gpu --disable-patching --disable-self-update --locale=en_US")
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                LOLProcess = Process.Start(psi);
            }
        }

        static void StopLOLClient()
        {
            if (LOLProcess != null)
            {
                LOLProcess.Kill();
                LOLProcess = null;
            }
            KillAllLeagueClientProcesses();
        }
        static void KillEdge()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("MicrosoftEdge");
                foreach (Process p in processes)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception x)
                    {
                        LogAndInfo("killedge (1) error: " + x.Message);
                    }
                }
            }
            catch (Exception x)
            {
                LogAndInfo("killedge (2) error: " + x.Message);
            }
        }

        public static void TimerChange(Timer timer, int interval = 1000, int timeout = Timeout.Infinite)
        {
            try
            {
                timer.Change(interval, timeout);
            }
            catch (Exception x)
            {
                Logger.Log("Timer change error: " + x.Message);
            }
        }
        public static void ListProcesses()
        {
            try
            {
                foreach (var x in Process.GetProcesses())
                {
                    if (x.ProcessName.Length > 0)
                        Logger.Log(x.ProcessName + " (pid: " + x.Id + "): " + x.MainWindowTitle.ToString());
                }
            }
            catch (Exception x)
            {
                Logger.Log("list process error: " + x.Message);
            }
        }
    }
}
