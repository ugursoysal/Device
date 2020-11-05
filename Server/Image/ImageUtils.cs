using System;
using System.Drawing;
using System.Windows.Forms;

namespace Server.Image
{
    public class ImageUtils
    {
        static long Ts = 0;

        public static void UpdateTs() // update timstm
        {
            Ts = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static bool TsExpired() // imagetimestampexpired
        {

            if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond > Ts + 500)
            {

                return true;

            }

            return false;

        }

        /*public static Bitmap TakeScreenCapture()
        {

            //Create a new bitmap screen size
            Bitmap image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);
            //image.Save("target.bmp");
            //Create a new Graphics object
            var gfx = Graphics.FromImage(image);

            //Copy the current screen
            gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            //Dispose of gfx
            gfx.Dispose();

            return image;

        }*/
        public static DirectBitmap TakeCapture()
        {
            DirectBitmap dbm = new DirectBitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            try
            {
                using (var g = Graphics.FromImage(dbm.Bitmap))
                {
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, new Size(Screen.PrimaryScreen.Bounds.Size.Width, Screen.PrimaryScreen.Bounds.Size.Height), CopyPixelOperation.SourceCopy);
                    g.Dispose();
                }
            }
            catch (Exception x)
            {
                Logger.Log("takecapture error: " + x.Message);
                throw x;
            }
            return dbm;
        }/*
        public static Bitmap TakeTestCapture()
        {
            return ScreenCapture.CaptureScreen();
        }*/
    }
}
