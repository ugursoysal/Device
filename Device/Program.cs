using System;
using System.Windows.Forms;

namespace Device
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Manager());
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("dev.txt", ex.Message);
            }
        }
    }
}
