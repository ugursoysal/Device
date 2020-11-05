using System;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// www
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                var config = new NLog.Config.LoggingConfiguration();

                // Targets where to log to: File and Console
                var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "logs.txt" };

                // Rules for mapping loggers to targets           
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);

                // Apply config           
                NLog.LogManager.Configuration = config;
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Server());
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("server.txt", ex.Message);
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            ClientApi.KillTimers();
            Communication.KillCloseThread();
            Communication.SendShutdownSignal();
        }
    }
}
