using Server.GameApi;
using Server.Image;
using Server.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public enum IndicatorStates
    {
        Off,
        Wait,
        On,
    }
    public partial class Server : Form
    {
        Communication Communication { get; set; }
        delegate void SafeCallDelegate(string text);
        static GameSession GameSession = null;
        public static Account LoadedAccount = null;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public static string LastInfo;
        public int PORT { get { return Convert.ToInt32(portTextBox.Text); } set { portTextBox.Text = value.ToString(); } }
        int CliID = 0;
        private System.Threading.Timer serverControl;

        public int ClientID { get { return CliID; } set { CliID = value; clientIDLabel.Text = CliID.ToString(); } }
        //public bool ServerIndicator { set { if (value) { pictureBox1.Image = global::Server.Properties.Resources.on; } else { pictureBox1.Image = global::Server.Properties.Resources.off; } } }
        public Server()
        {
            InitializeComponent();
        }

        private void Server_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        public void SetIndicator(IndicatorStates state)
        {
            switch (state)
            {
                case IndicatorStates.Off:
                    Indicator.Image = Properties.Resources.off;
                    break;
                case IndicatorStates.On:
                    Indicator.Image = Properties.Resources.on;
                    break;
                case IndicatorStates.Wait:
                    Indicator.Image = Properties.Resources.working;
                    break;
            }
        }
        public void LoadAccount(string username, string password)
        {
            LoadedAccount = new Account(username, password);
        }
        public void SetInfoText(string text)
        {
            if (InfoText.InvokeRequired)
            {
                var d = new SafeCallDelegate(SetInfoText);
                InfoText.Invoke(d, new object[] { text });
            }
            else
            {
                InfoText.Text = text;
            }
            LastInfo = text;
        }
        private void Indicator_Click(object sender, EventArgs e)
        {
            ClientApi.KillTimers();
            Communication.SendShutdownSignal();
            Communication.KillCloseThread();
            Close();
        }

        private void Server_Shown(object sender, EventArgs e)
        {
            Communication = new Communication(this);
            serverControl = new System.Threading.Timer(ServerControlCallback, null, 300000, Timeout.Infinite);
        }

        private void PortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            GameSession.Start = 1;
            LCU LCU = new LCU();
            GameSession = new GameSession(this, Convert.ToInt32(testBox1.Text));
        }

        private void Test2Button_Click(object sender, EventArgs e)
        {
            Thread thr = new System.Threading.Thread(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Logger.Log("log thread" + i);
                }
            });
            thr.Start();
            for (int i = 0; i < 100; i++)
            {
                Logger.Log("log" + i);
            }
            System.Threading.Thread.Sleep(3000);
            Stopwatch all = new Stopwatch();
            Stopwatch bll = new Stopwatch();
            Stopwatch cll = new Stopwatch();
            Stopwatch dll = new Stopwatch();
            Stopwatch ell = new Stopwatch();
            Stopwatch fll = new Stopwatch();
            Stopwatch gll = new Stopwatch();
            all.Start();
            var bmp = PixelCache.GetScreenshot();
            bll.Start();
            Point me = ImageValues.GetMyPosition(bmp); // 35ms
            bll.Stop();
            cll.Start();
            Point tower = ImageValues.EnemyTowerPosition(bmp); // 130ms
            cll.Stop();
            dll.Start();
            Point enemy = ImageValues.EnemyPosition(bmp); // 130ms
            dll.Stop();
            ell.Start();
            Point enemyCreep = ImageValues.EnemyCreepPosition(bmp); // 127ms
            ell.Stop();
            Point ally = ImageValues.AllyPosition(bmp); // 129ms

            Point allyCreep = ImageValues.AllyCreepPosition(bmp); // 129ms
            //int hpercent = Player.GetHealthPercent();
            fll.Start();
            int hpDiff = ImageValues.GetHPDifference(bmp);
            fll.Stop();
            gll.Start();
            int creepHpDiff = ImageValues.GetCreepHPDifference(bmp);
            gll.Stop();
            all.Stop();
            bmp.Dispose();
            SetInfoText(all.ElapsedMilliseconds + "ms total" + Environment.NewLine +
                bll.ElapsedMilliseconds + "ms" + Environment.NewLine +
                cll.ElapsedMilliseconds + "ms" + Environment.NewLine +
                dll.ElapsedMilliseconds + "ms" + Environment.NewLine +
                ell.ElapsedMilliseconds + "ms" + Environment.NewLine +
                fll.ElapsedMilliseconds + "ms" + Environment.NewLine +
                gll.ElapsedMilliseconds + "ms" + Environment.NewLine);

        }
        private void ServerControlCallback(object state)
        {
            try
            {
                TimeSpan timeSpan = new TimeSpan(Math.Abs(DateTime.Now.Ticks - GameSession.TimerTick));
                if (timeSpan.TotalMinutes > 10)
                {
                    ClientApi.LCU = new LCU();
                    if (LCU.SummonerID == 0)
                    {
                        try
                        {
                            var bmp = PixelCache.GetScreenshot();
                            bmp.Bitmap.Save("kill2.bmp");
                            bmp.Dispose();
                        }
                        catch (Exception x)
                        {
                            Logger.Log("kill (2) image error..." + x.Message);
                        }
                        Logger.Log("kill2 " + timeSpan.ToString());
                        ClientApi.Stop();
                        SetInfoText("Restarting bot...");
                        if (LoadedAccount != null)
                            ClientApi.Start();
                        else
                            Logger.Log("no loaded account");
                    }
                    else
                    {
                        try
                        {
                            var bmp = PixelCache.GetScreenshot();
                            bmp.Bitmap.Save("gs2.bmp");
                            bmp.Dispose();
                        }
                        catch (Exception x)
                        {
                            Logger.Log("gamesession (2) image error..." + x.Message);
                        }
                        Logger.Log("gamesession (2) restart..." + timeSpan.ToString());
                        GameSession.Start = 1;
                        GameSession = new GameSession(this, Game.Champions.RandomChamp());
                    }

                }
                else
                {
                    Logger.Log("check - OK: bot is working... (2)");
                }
            }
            catch (Exception x)
            {
                Logger.Log("check - FAIL: serverControlTimer error: " + x.Message);
            }
            finally
            {
                ClientApi.TimerChange(serverControl, 30000);
            }
        }
    }
}
