using InputManager;
using Server.Image;
using Server.Models;
using Server.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace Server.GameApi
{
    public class GameSession
    {
        static MainPlayer Player = null;
        static Shop Shop = null;
        public static int Start = 0;
        public static GameSessionModel GameState = null;
        static Timer GameTimer = null;
        static Timer MinuteTimer = null;
        static Timer HalfMinuteTimer = null;
        static Timer TenSecondsTimer = null;
        static Timer SecondsTimer = null;
        static Color LastScreenColor = Color.Black;
        const int MinuteTimerInterval = 60000;
        public static IntPtr LOLHandle = IntPtr.Zero;
        public double healthBefore = 0;
        int Port;
        int goB = -1;
        int secondHP = 0;
        Server Server = null;
        static Point me;
        static List<Item> items = null;
        static int GameTimerTry = 0;
        public static long TimerTick = DateTime.Now.Ticks;
        int Champ = 0;

        static Random rand = new Random();

        Point tone = new Point(111, 26); // enemy
        Point ttwo = new Point(24, 22); // top
        Point tthree = new Point(21, 107); // base

        Point one = new Point(51, 145); // base
        Point two = new Point(143, 51); // enemy
        Point three = new Point(142, 143); // bot
        public GameSession(Server server, int champ)
        {
            Champ = champ;
            Port = GetPort();
            Server = server;
            if (GameTimer != null)
                GameTimer.Dispose();
            GameTimerTry = 0;
            GameTimer = new Timer(GameTimerCallback, null, 1000, Timeout.Infinite);
            Logger.Log("Waiting for game to start...");
            Server.SetInfoText("Waiting for game to start...");
            Server.SetIndicator(IndicatorStates.Wait);
            if (champ != 0)
                items = Game.Champions.GetItems(champ);
            if (GameState == null || GameState.Started == false)
                GameState = new GameSessionModel();
        }

        public static int GetPort()
        {
            try
            {
                var process = ClientApi.GetProcessByName("League of Legends");
                if (process != null)
                {
                    LOLHandle = process.MainWindowHandle;
                    var x = NativeMethods.GetAllTcpConnections();
                    foreach (var a in x)
                    {
                        if (process.Id == a.owningPid)
                        {
                            return a.LocalPort;
                        }
                    }
                }
                else
                {
                    LOLHandle = IntPtr.Zero;
                }
            }
            catch (Exception x)
            {
                Logger.Log("getport error: " + x.Message);
            }
            return 0;
        }
        ~GameSession()
        {
            if (SecondsTimer != null)
                SecondsTimer.Dispose();
            SecondsTimer = null;
            if (TenSecondsTimer != null)
                TenSecondsTimer.Dispose();
            TenSecondsTimer = null;
            if (HalfMinuteTimer != null)
                HalfMinuteTimer.Dispose();
            HalfMinuteTimer = null;
            if (MinuteTimer != null)
                MinuteTimer.Dispose();
            MinuteTimer = null;
        }
        private void GameTimerCallback(object state)
        {
            try
            {
                //Logger.Log("game tim: " + GameState.Started);
                GameTimerTry++;
                TimerTick = DateTime.Now.Ticks;
                // waiting for game to start
                if (GameState.Started == false)
                {
                    //Logger.Log("ee");
                    try
                    {

                        //Logger.Log("k1");
                        Player = new MainPlayer(((Port = GetPort()) != 0) ? Port : 2999);
                        //Logger.Log("k3 p: " + Port);
                        /*if (Player != null)
                            Logger.Log("gts p: " + Port + " currenthealth: " + Player.game.CurrentHealth + " money: " + Player.GetGold());
                        */
                        if (Player != null && Player?.game?.CurrentHealth != 1)
                        {
                            GameState.Started = Player.GetGold() != 0;
                            //er.Log("k4 "+ GameState.Started);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Waiting for game to start... (err)");
                        Logger.Log("wait err: " + ex.Message);
                        Server.SetInfoText("Waiting for game to start... (err)");
                    }
                    if (GameTimerTry == 50)
                    {
                        Server.SetInfoText("Still waiting...");
                        Control();
                    }
                    //Logger.Log("gtfalse2");
                    if (GameTimerTry > 55)
                    {
                        Control();
                        if (LCU.SummonerID == 0)
                        {
                            if (Server.LoadedAccount != null)
                            {
                                GameTimerTry = 0;
                                ClientApi.Stop();
                                Logger.Log("Restarting bot...");
                                Server.SetInfoText("Restarting bot...");
                                Thread.Sleep(10000);
                                ClientApi.Start();
                            }
                            else
                            {
                                Logger.Log("Can't find the loaded account.");
                                Server.SetInfoText("Can't find the loaded account.");
                            }
                        }
                        else
                        {
                            var process = ClientApi.GetProcessByName("League of Legends");
                            if (process == null)
                            {
                                try
                                {
                                    Logger.Log("Trying to continue...");
                                    Server.SetInfoText("Trying to continue...");
                                    var bmp = PixelCache.GetScreenshot();
                                    bmp.Bitmap.Save("qr1.bmp");
                                    bmp.Dispose();
                                    ClientApi.TryToContinue();
                                    Logger.Log("Restarting queue...");
                                    Server.SetInfoText("Restarting queue...");
                                    GameTimerTry = 0;
                                    ClientApi.StartNewGame();
                                }
                                catch (Exception x)
                                {
                                    Logger.Log("queuerestart1 (1) image error..." + x.Message);
                                    Thread.Sleep(5000);
                                    ClientApi.Stop();
                                    Logger.Log("Can't take screenshots. Trying to continue...");
                                    Server.SetInfoText("Can't take screenshots. Trying to continue...");
                                    Control(false);
                                    ClientApi.Start();
                                }
                            }
                            else
                            {
                                bool flag = false;
                                try
                                {
                                    var bmp = PixelCache.GetScreenshot();
                                    bmp.Bitmap.Save("copen.bmp");
                                    if (bmp.GetPixel(495, 352) == Color.FromArgb(59, 45, 24)
                                        || bmp.GetPixel(507, 344) == Color.FromArgb(255, 255, 255))
                                        flag = true;
                                    bmp.Dispose();
                                    if (flag)
                                        Logger.Log("connection lost");
                                    else
                                        Logger.Log("copen ok");
                                }
                                catch (Exception x)
                                {
                                    Logger.Log("copen (1) image error..." + x.Message);
                                    flag = true;
                                }
                                if (flag)
                                {
                                    ClientApi.Stop();
                                    Logger.Log("Connection lost. Trying to continue...");
                                    Server.SetInfoText("Connection lost. Trying to continue...");
                                    Thread.Sleep(5000);
                                    ClientApi.Restart = true;
                                    ClientApi.Start();
                                }
                            }
                        }
                    }
                    else if (GameTimer != null)
                        GameTimer.Change(8000, Timeout.Infinite);

                    //Logger.Log("gtfalse3");
                }
                else
                {
                    //Logger.Log("gttrue1");
                    if (LOLHandle != null)
                    {
                        NativeMethods.Focus(LOLHandle);
                    }
                    else
                    {
                        Logger.Log("LOLHandle = null (?)");
                    }
                    ClientApi.SetState(ClientStates.InGame);
                    Server.SetIndicator(IndicatorStates.On);
                    TimerTick = DateTime.Now.Ticks;
                    //Logger.Log("gttrue2");

                    if (Port != 0 && Player == null)
                        Player = new MainPlayer(Port);
                    Shop = new Shop();
                    if (Start == 0)
                    {
                        Logger.Log("Game started.");
                        Server.SetInfoText("Game started.");
                        try
                        {
                            int gold = Player.GetGold();
                            foreach (var x in items)
                            {
                                if (x.got == false)
                                {
                                    if (gold > x.cost)
                                    {
                                        if (!Shop.Opened)
                                        {
                                            //Logger.Log("opening shop");
                                            Shop.Toogle();
                                            Thread.Sleep(RandomTimeGenerator(500));/*
                                MouseOperations.SetCursorPosition(420 + RandomTimeGenerator(5), 80 + RandomTimeGenerator(4));
                                Thread.Sleep(RandomTimeGenerator(200));
                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                                Thread.Sleep(RandomTimeGenerator(50));
                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);*/
                                            Keyboard.ShortcutKeys(new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.ControlKey, System.Windows.Forms.Keys.L }, 40);
                                            Thread.Sleep(RandomTimeGenerator(1000));
                                        }
                                        Thread.Sleep(RandomTimeGenerator());
                                        string str = x.phrase;

                                        //Logger.Log("buying: " + x.name);
                                        foreach (char c in str)
                                        {
                                            Thread.Sleep(RandomTimeGenerator(150));
                                            Keyboard.KeyPress((System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), c.ToString()), RandomTimeGenerator(50));
                                        }
                                        Thread.Sleep(RandomTimeGenerator(500));
                                        MouseOperations.SetCursorPosition(377 + RandomTimeGenerator(15), 146 + RandomTimeGenerator(8));

                                        Thread.Sleep(RandomTimeGenerator(150));
                                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                                        Thread.Sleep(RandomTimeGenerator(40));
                                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                                        Thread.Sleep(RandomTimeGenerator(500));
                                        Server.SetInfoText("Bought " + x.name);
                                        //Logger.Log("bought: " + x.name);

                                        for (int i = 0; i < str.Length; i++)
                                        {
                                            Thread.Sleep(RandomTimeGenerator(100));
                                            Keyboard.KeyPress(System.Windows.Forms.Keys.Back);
                                        }
                                        x.got = true;
                                        gold -= x.cost;
                                    }
                                    else
                                        break;
                                }
                            }
                            Thread.Sleep(RandomTimeGenerator());
                            /*if (Shop.Opened)
                                Logger.Log("closing shop");*/
                            if (Shop.Opened)
                                Shop.Toogle();
                            Thread.Sleep(RandomTimeGenerator());
                            Server.SetInfoText("Bought first items.");
                            Thread.Sleep(RandomTimeGenerator(35000) + 7500);
                        }
                        catch (Exception x)
                        {
                            Logger.Log("gg: " + x.Message);
                        }
                    }
                    else
                    {
                        Thread.Sleep(RandomTimeGenerator(10000));
                        Logger.Log("Game (re)started.");
                        Server.SetInfoText("Game (re)started.");
                        Start = 0;
                        items[0].got = true;
                    }
                    Logger.Log("gttrue3 - after start");
                    try
                    {
                        var bmp = PixelCache.GetScreenshot();
                        Color col = bmp.GetPixel(795, 756);
                        if (col.R != 0 && col.R != 255 && col.R != 165 && col.G != 142 && col.B != 99)
                        {
                            Keyboard.KeyPress(System.Windows.Forms.Keys.Y, RandomTimeGenerator(50));
                            string str = "Fixed camera."/* + Environment.NewLine + "R:" + col.R + " G:" + col.G + " B:" + col.B*/;
                            Server.SetInfoText(str);
                            Logger.Log(str);
                        }
                        if (bmp != null)
                            bmp.Dispose();
                    }
                    catch (Exception x)
                    {
                        Logger.Log("getscreen problem: " + x.Message);
                    }
                    Logger.Log("starting timers");
                    MinuteTimer = new Timer(MinuteTimerCallback, null, MinuteTimerInterval, Timeout.Infinite);
                    HalfMinuteTimer = new Timer(HalfMinuteTimerCallback, null, 30000, Timeout.Infinite);
                    TenSecondsTimer = new Timer(TenSecondsTimerCallback, null, 10000, Timeout.Infinite);
                    SecondsTimer = new Timer(SecondsTimerCallback, null, 1000, Timeout.Infinite);
                    Logger.Log("started");
                    SkillUp(2);
                    if (GameTimer != null)
                    {
                        GameTimer.Dispose();
                        GameTimer = null;
                    }
                    //Logger.Log("gtend");
                }
            }
            catch (Exception e)
            {
                try
                {
                    var bmp = PixelCache.GetScreenshot();
                    bmp.Bitmap.Save("fe1.bmp");
                    bmp.Dispose();
                }
                catch (Exception x)
                {
                    Logger.Log("fatal (1) image error..." + x.Message);
                }
                Logger.Log("fatal error: gametimer \r\n" + e.Message);
                /*
                MinuteTimer = new Timer(MinuteTimerCallback, null, MinuteTimerInterval, Timeout.Infinite);
                HalfMinuteTimer = new Timer(HalfMinuteTimerCallback, null, 30000, Timeout.Infinite);
                TenSecondsTimer = new Timer(TenSecondsTimerCallback, null, 10000, Timeout.Infinite);
                SecondsTimer = new Timer(SecondsTimerCallback, null, 1000, Timeout.Infinite);
                SkillUp(2);*/
                if (GameTimer != null)
                {
                    GameTimer.Dispose();
                    GameTimer = null;
                }
            }
        }

        private void TenSecondsTimerCallback(object state)
        {
            //Logger.Log("ten tim: " + GameState.Started);
            if (GameState.Started == true)
            {
                try
                {
                    //TimerTick = DateTime.Now.Ticks;
                    if (Player.Dead())
                        goB = 1;
                    NativeMethods.SetForegroundWindow(LOLHandle);//make the window top
                    if (goB < 1)
                    {
                        if (!Player.Dead() && Player.GetHealthPercent() < 30)
                        {
                            goB = 20;
                            secondHP = 2;
                            Server.SetInfoText("Going to base.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error in 10sec timer: " + ex.Message);
                }
                if (TenSecondsTimer != null)
                    TenSecondsTimer.Change(RandomTimeGenerator(10000), Timeout.Infinite);
                else
                {
                    Logger.Log("10sec timer stopped - restarting...");
                    TenSecondsTimer = new Timer(TenSecondsTimerCallback, null, RandomTimeGenerator(10000), Timeout.Infinite);
                }
            }
            else
            {
                Logger.Log("10sec timer finishing game...");
                FinishGame();
                LOLHandle = IntPtr.Zero;
            }
        }
        private void FinishGame()
        {
            Logger.Log("Game finished!");
            Server.SetInfoText("Game finished.");
            if (SecondsTimer != null)
                SecondsTimer.Dispose();
            SecondsTimer = null;
            if (TenSecondsTimer != null)
                TenSecondsTimer.Dispose();
            TenSecondsTimer = null;
            if (HalfMinuteTimer != null)
                HalfMinuteTimer.Dispose();
            HalfMinuteTimer = null;
            if (MinuteTimer != null)
                MinuteTimer.Dispose();
            MinuteTimer = null;
            Thread.Sleep(10000);
            ClientApi.TryToContinue();

            MouseOperations.SetCursorPosition(520 + RandomTimeGenerator(3), 380 + RandomTimeGenerator(3));

            Thread.Sleep(RandomTimeGenerator());
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            Thread.Sleep(RandomTimeGenerator(10));
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            Thread.Sleep(RandomTimeGenerator(6000));
            /*for (int a = 0; a < 5; a++)
            {
                MouseOperations.SetCursorPosition(934 + RandomTimeGenerator(5), 850 + RandomTimeGenerator(5));
                Thread.Sleep(RandomTimeGenerator());
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                Thread.Sleep(RandomTimeGenerator(10));
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            }*/
            Server.SetIndicator(IndicatorStates.Wait);
            //Server.GameSession = new GameSession(Server);
            ClientApi.StartNewGame();
        }
        private void SecondsTimerCallback(object state)
        {
            //Logger.Log("sec tim");
            if (GameState.Started == true && !Player.Dead())
            {
                /*Stopwatch all = new Stopwatch();
                all.Start();*/
                // var bmp = ImageUtils.TakeSmallCapture(0, 0, 1024, 768);
                try
                {
                    var bmp = PixelCache.GetScreenshot();
                    Color col = bmp.GetPixel(300, 300);
                    if (col != LastScreenColor)
                        TimerTick = DateTime.Now.Ticks;
                    LastScreenColor = col;
                    me = ImageValues.GetMyPosition(bmp);
                    Point tower = ImageValues.EnemyTowerPosition(bmp);
                    Point enemy = ImageValues.EnemyPosition(bmp);
                    Point enemyCreep = ImageValues.EnemyCreepPosition(bmp);
                    Point ally = ImageValues.AllyPosition(bmp);
                    Point allyCreep = ImageValues.AllyCreepPosition(bmp);

                    int hpercent = Player.GetHealthPercent();
                    int hpDiff = ImageValues.GetHPDifference(bmp);
                    int creepHpDiff = ImageValues.GetCreepHPDifference(bmp);
                    //int distanceEnemyCreep = ImageValues.GetDistanceFromOrigin(enemyCreep);
                    var pressNothing = false;
                    bool Attack = false;
                    int baseX = 757;
                    int baseY = 145;
                    if (tower.Y > 5 || enemy.Y > 15 || enemyCreep.Y > 15)
                    {
                        baseX = 566;//allyCreep.X;
                        baseY = 350;//allyCreep.Y;*/
                    }
                    //Server.SetInfoText("Calculating...");
                    /* FORWARD
                        baseX = 757;//allyCreep.X;
                        baseY = 145;//allyCreep.Y;*/
                    /* MIDDLE
                        baseX = 566;//allyCreep.X;
                        baseY = 357;//allyCreep.Y;*/
                    /* BACK
                        baseX = 46;
                        baseY = 695;*/
                    if (hpDiff < -50 || creepHpDiff < -40)
                    {/*
                    Server.SetInfoText("OK."
                        + Environment.NewLine + (hpDiff < -50).ToString()
                        + Environment.NewLine + (creepHpDiff < -40).ToString());*/
                        Attack = true;
                        if (goB > 0) goB = 20;
                    }
                    if (Player.game.CurrentHealth < healthBefore
                        || (goB < 1 && hpercent < 20)
                        || tower.Y > 100
                        || hpDiff + (hpercent / 2) < -245
                        || (creepHpDiff < -150 && allyCreep.X == 0)
                        || enemyCreep.Y > 380)
                    {
                        /*if (tower.Y > 110)
                            Logger.Log("tower.Y = " + tower.Y + " > 110");
                        else if (Player.game.Level < 13 && creepHpDiff < -150 && allyCreep.X == 0)
                            Logger.Log("creepHpDiff = " + creepHpDiff + " < -150 && allyCreep.X = 0");                    
                        else if (hpDiff + (hpercent / 2) < -260)
                            Logger.Log("hpDiff + (hpercent / 2) = " + (hpDiff + (hpercent / 2)).ToString() + " < -245");
                        else if (enemyCreep.Y > 380)
                            Logger.Log("enemyCreep.Y = " + enemyCreep.Y + " > 380");*/
                        //Server.SetInfoText("Need to get back.");
                        if (tower.Y > 110 || hpDiff < -700)
                            secondHP = 2;
                        else
                            secondHP = 1;
                        if (goB > 0) goB = 20;
                    }
                    if (secondHP > 0)
                    {
                        Attack = false;
                        baseX = 46;
                        baseY = 695;
                        secondHP--;
                    }
                    else if (goB > 0)
                    {
                        if (hpercent > 90)
                        {
                            goB = 1;
                        }
                        if (goB == 20)
                        {
                            if (enemy.X > 0 || enemyCreep.X > 0 || tower.X > 0)
                                secondHP = 2;
                            else
                            {
                                Keyboard.KeyPress(System.Windows.Forms.Keys.B);
                                goB--;
                                pressNothing = true;
                            }
                        }
                        else if (goB < 20)
                        {
                            goB--;
                            pressNothing = true;
                        }
                        if (goB == 0)
                        {
                            if (hpercent < 95)
                            {
                                goB = 5;
                            }
                            else if (hpercent < 50)
                            {
                                goB = 20;
                            }
                            else
                            {
                                //-----buyitems
                                Server.SetInfoText("Buying items...");
                                int gold = Player.GetGold();
                                foreach (var x in items)
                                {
                                    if (x.got == false)
                                    {
                                        if (gold > x.cost)
                                        {
                                            if (!Shop.Opened)
                                            {
                                                Shop.Toogle();
                                                Thread.Sleep(RandomTimeGenerator(500));
                                                /*MouseOperations.SetCursorPosition(420 + RandomTimeGenerator(5), 80 + RandomTimeGenerator(4));
                                                Thread.Sleep(RandomTimeGenerator(200));
                                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                                                Thread.Sleep(RandomTimeGenerator(50));
                                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);*/
                                                Keyboard.ShortcutKeys(new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.ControlKey, System.Windows.Forms.Keys.L }, RandomTimeGenerator(40));
                                                Thread.Sleep(RandomTimeGenerator(1000));
                                            }
                                            Thread.Sleep(RandomTimeGenerator());
                                            string str = x.phrase;
                                            foreach (char c in str)
                                            {
                                                Thread.Sleep(RandomTimeGenerator(150));
                                                Keyboard.KeyPress((System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), c.ToString()), RandomTimeGenerator(50));
                                            }
                                            Thread.Sleep(RandomTimeGenerator(500));
                                            MouseOperations.SetCursorPosition(377 + RandomTimeGenerator(15), 146 + RandomTimeGenerator(8));

                                            Thread.Sleep(RandomTimeGenerator(150));
                                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                                            Thread.Sleep(RandomTimeGenerator(40));
                                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                                            Thread.Sleep(RandomTimeGenerator(500));
                                            Server.SetInfoText("Bought " + x.name);

                                            for (int i = 0; i < str.Length; i++)
                                            {
                                                Thread.Sleep(RandomTimeGenerator(100));
                                                Keyboard.KeyPress(System.Windows.Forms.Keys.Back);
                                            }
                                            x.got = true;
                                            gold -= x.cost;
                                        }
                                        else
                                            break;
                                    }
                                }
                                Thread.Sleep(RandomTimeGenerator());
                                if (Shop.Opened)
                                    Shop.Toogle();
                                //-----buyitems
                                pressNothing = true;
                                /* baseX = 566;//allyCreep.X;
                                 baseY = 357;//allyCreep.Y;*/
                                goB = -1;
                            }
                        }
                    }
                    else if (allyCreep.X == 0)
                    {
                        //Server.SetInfoText("No ally in sight.");
                        if (enemyCreep.X != 0 || enemy.X != 0 || tower.X != 0)
                            secondHP = 2;
                    }
                    else
                    {
                        baseX = allyCreep.X;
                        baseY = allyCreep.Y;
                        if (enemyCreep.X != 0)
                        {
                            Attack = true;
                            Server.SetInfoText("Attacking creeps.");
                            baseX = 566;//allyCreep.X;
                            baseY = 350;//allyCreep.Y;
                        }
                        else if (tower.Y < 15 && enemy.X != 0 && (allyCreep.X != 0 || ally.X != 0) && hpDiff > -400)
                        {
                            Attack = true;
                            Server.SetInfoText("Attacking enemy.");
                            baseX = 566;//allyCreep.X;
                            baseY = 350;//allyCreep.Y;
                        }
                        else if (tower.X != 0)
                        {
                            Attack = true;
                            Server.SetInfoText("Attacking tower.");
                            baseX = 566;//allyCreep.X;
                            baseY = 350;//allyCreep.Y;
                        }
                    }
                    //Server.SetInfoText("Calculated...");
                    if (!pressNothing)
                    {
                        Random rand = new Random();
                        int r1 = rand.Next(-15, 15);
                        int r2 = rand.Next(-14, 14);

                        if (hpercent > 10 && Attack && (ally.X != 0 || r1 > 8) && (enemyCreep.X > 0 || enemy.X > 0) && hpercent > 30 && Player.GetManaPercent() > 30)
                        {
                            new Thread(() =>
                            {
                                if (enemy.X > 0)
                                    MouseOperations.SetCursorPosition(enemy.X, enemy.Y + RandomTimeGenerator(10));
                                else
                                    MouseOperations.SetCursorPosition(enemyCreep.X, enemyCreep.Y + RandomTimeGenerator(10));
                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                                Thread.Sleep(RandomTimeGenerator(30));
                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                                Thread.Sleep(RandomTimeGenerator(100));
                                int choice = rand.Next(1, 4);
                                if (enemy.X > 0)
                                    choice += 1;
                                DoRandomSkill(choice);
                                //Server.SetInfoText("doskill: " + choice);
                            }).Start();
                        }
                        else
                        {
                            MouseOperations.SetCursorPosition(baseX + r1, baseY + r2);
                            if (Attack && hpercent > 20)
                                Keyboard.KeyPress(System.Windows.Forms.Keys.A);
                            else if (!Attack)
                            {
                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                                Thread.Sleep(RandomTimeGenerator(12));
                                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                            }
                        }
                    }
                    bmp.Dispose();
                    //all.Stop();
                    healthBefore = Player.game.CurrentHealth;
                    /* Logger.Log("ms: " + all.ElapsedMilliseconds.ToString());
                     string ftr = "me:" + me.ToString() + Environment.NewLine
                         + "tower:" + tower.ToString() + Environment.NewLine
                         + "e:" + enemy.ToString() + Environment.NewLine
                         + "ec:" + enemyCreep.ToString() + Environment.NewLine
                         + "a:" + ally.ToString() + Environment.NewLine
                         + "a:" + allyCreep.ToString() + Environment.NewLine
                         + "hpDiff:" + hpDiff.ToString() + Environment.NewLine
                         + "creepHpDiff:" + creepHpDiff.ToString() + Environment.NewLine;
                     Logger.Log(ftr);*/
                    //Logger.Log("me:" + me.ToString());
                    //Server.SetInfoText(ftr);

                }
                catch (Exception x)
                {
                    Logger.Log("seconds timer error: " + x.Message);
                }
            }

            GameState.Started = (Player.GetGold() != 0);
            if (GameState.Started == false)
            {
                Logger.Log("seconds timer stopped - finish game...");
                FinishGame();
            }
            else
            {
                if (SecondsTimer != null)
                {
                    SecondsTimer.Change(RandomTimeGenerator(), Timeout.Infinite);
                }
                else
                {
                    Logger.Log("seconds timer stopped - restarting...");
                    SecondsTimer = new Timer(SecondsTimerCallback, null, RandomTimeGenerator(), Timeout.Infinite);
                    if (TenSecondsTimer == null)
                    {
                        Logger.Log("10sec timer stopped - restarting...");
                        TenSecondsTimer = new Timer(TenSecondsTimerCallback, null, RandomTimeGenerator(10000), Timeout.Infinite);
                    }
                }
            }
        }

        private void HalfMinuteTimerCallback(object state)
        {
            //Logger.Log("half tim: " + GameState.Started);
            if (GameState.Started == true)
            {
                try
                {
                    //TimerTick = DateTime.Now.Ticks;
                    if (goB < 1)
                    {
                        if (Player.GetLevel() >= 6)
                            SkillUp(4);
                        Random rand = new Random();
                        int choice = rand.Next(1, 4);
                        SkillUp(choice);
                        //Server.SetInfoText("skillup: " + choice);
                    }
                    //TODO
                }
                catch (Exception ex)
                {
                    Logger.Log("err: " + ex.Message);
                }
            }
            HalfMinuteTimer.Change(RandomTimeGenerator(30000), Timeout.Infinite);
        }

        private void MinuteTimerCallback(object state)
        {
            //Logger.Log("min tim: " + GameState.Started);
            if (GameState.Started == true)
            {
                try
                {
                    /*if (min < 1 && goB < 1)
                        secondHP += 1;*/

                    var bmp = ImageUtils.TakeCapture();
                    //bmp.Save("bmp.bmp");
                    if (!Shop.Opened)
                    {
                        Color col = bmp.GetPixel(795, 756);
                        if (col.R != 0 && col.R != 255 && col.R != 165 && col.G != 142 && col.B != 99)
                        {
                            Keyboard.KeyPress((System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), "Y"), RandomTimeGenerator(50));

                            string str = "Fixed camera." + Environment.NewLine + "R:" + col.R + " G:" + col.G + " B:" + col.B;
                            Server.SetInfoText(str);
                            Logger.Log(str);
                        }
                    }/*
                    int topTowerLevel = ImageValues.GetTowerStatus(bmp, Lanes.Top);
                    int midTowerLevel = ImageValues.GetTowerStatus(bmp, Lanes.Mid);
                    int botTowerLevel = ImageValues.GetTowerStatus(bmp, Lanes.Bot);*/
                    if (goB < 1)
                    {
                        bool topJung = ImageValues.IsPointInTriangle(one, two, three, me);
                        bool botJung = ImageValues.IsPointInTriangle(tone, ttwo, tthree, me);

                        if (topJung || botJung)
                        {
                            goB = 20;
                            Server.SetInfoText("Stuck in jungle, going to B" + me.ToString());
                            Logger.Log("Stuck in jungle, going to B" + me.ToString());
                        }
                    }
                    bmp.Dispose();

                    Server.SetInfoText(Player.game.SummonerName
                        //+ "(" + Player.game.Kills + "/" + Player.game.Deaths + "/" + Player.game.Assists + ")" 
                        + "(lvl: " + Player.game.Level + ") - $: " + Player.GetGold());
                }
                catch (Exception ex)
                {
                    Logger.Log("err: " + ex.Message);
                }
            }
            if (MinuteTimer != null)
                MinuteTimer.Change(RandomTimeGenerator(MinuteTimerInterval), Timeout.Infinite);
        }
        public static int RandomTimeGenerator(int avg = 1000) => rand.Next(avg - (avg / 3), avg + (avg / 3));
        public void SkillUp(int choice = 0)
        {
            switch (choice)
            {
                case 1:
                    Keyboard.ShortcutKeys(new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.ControlKey, System.Windows.Forms.Keys.Q }, 30);
                    break;
                case 2:
                    Keyboard.ShortcutKeys(new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.ControlKey, System.Windows.Forms.Keys.W }, 30);
                    break;
                case 3:
                    Keyboard.ShortcutKeys(new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.ControlKey, System.Windows.Forms.Keys.E }, 30);
                    break;
                case 4:
                    Keyboard.ShortcutKeys(new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.ControlKey, System.Windows.Forms.Keys.R }, 30);
                    break;
                default:
                    break;
            }
        }
        public void DoRandomSkill(int choice = 0)
        {
            switch (choice)
            {
                case 1:
                    Keyboard.KeyPress(System.Windows.Forms.Keys.Q);
                    break;
                case 2:
                    Keyboard.KeyPress(System.Windows.Forms.Keys.W);
                    break;
                case 3:
                    Keyboard.KeyPress(System.Windows.Forms.Keys.E);
                    break;
                case 4:
                    Keyboard.KeyPress(System.Windows.Forms.Keys.R);
                    break;
                default:
                    Keyboard.KeyPress(System.Windows.Forms.Keys.W);
                    break;
            }
            Thread.Sleep(RandomTimeGenerator(80));
            int i = new Random().Next(0, 15);
            if (i == 8)
            {
                Keyboard.KeyPress(System.Windows.Forms.Keys.D);
            }
            else if (i == 9)
            {
                Keyboard.KeyPress(System.Windows.Forms.Keys.F);
            }
        }
        public static void Control(bool a = true)
        {
            try
            {
                var processes = Process.GetProcessesByName("System Error");
                foreach (Process proc in processes)
                {
                    if (!proc.HasExited)
                    {
                        Logger.Log("System error. Reconnecting...");
                        proc.Kill();
                        if (!a)
                        {
                            Thread.Sleep(10000);
                            ClientApi.TryToContinue();
                            Thread.Sleep(500);
                            Mouse.Move(411, 363);
                            InputManager.Mouse.ButtonDown(Mouse.MouseKeys.Left);
                            Thread.Sleep(50);
                            Mouse.ButtonUp(Mouse.MouseKeys.Left);
                        }
                        break;
                    }
                }
                processes = Process.GetProcessesByName("Error");
                foreach (Process proc in processes)
                {
                    if (!proc.HasExited)
                    {
                        Logger.Log("Error. Reconnecting...");
                        proc.Kill();
                        if (!a)
                        {
                            Thread.Sleep(1000);
                            ClientApi.TryToContinue();
                            Thread.Sleep(500);
                            Mouse.Move(411, 363);
                            InputManager.Mouse.ButtonDown(Mouse.MouseKeys.Left);
                            Thread.Sleep(50);
                            Mouse.ButtonUp(Mouse.MouseKeys.Left);
                        }
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                Logger.Log("error control error: " + x.Message);
            }
        }
    }
}