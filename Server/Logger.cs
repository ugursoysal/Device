using System;

namespace Server
{
    public class Logger
    {
        private static readonly NLog.Logger nLog = NLog.LogManager.GetCurrentClassLogger();
        public static void Log(string log)
        {
            try
            {
                nLog.Info(log);
            }
            catch (Exception ex)
            {
                nLog.Error(ex, "Goodbye cruel world", ex.Message);
            }
        }
    }
}
