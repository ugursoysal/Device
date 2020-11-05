namespace Server.Image
{
    public class PixelCache
    {
        public const string SCREENSHOT_IMAGE_NAME = "screenshot";

        //static int[] ImagePixels;

        private static DirectBitmap CurrentScreenshot;
        public static DirectBitmap GetScreenshot()
        {
            TakeScreenshot();
            //CurrentScreenshot.Save("screen.bmp");
            return CurrentScreenshot;
        }

        public static void TakeScreenshot()
        {

            if (ImageUtils.TsExpired())
            {

                //Clear image from memory
                if (CurrentScreenshot != null) CurrentScreenshot.Dispose();

                //Get a screen capture
                CurrentScreenshot = ImageUtils.TakeCapture();
                //CurrentScreenshot = ImageUtils.TakeSmallCapture(448, 156, 1024, 768);
                //Save the screenshot pixels
                //ImagePixels = ConvertImage(CurrentScreenshot);
                //ImagePixels = GetRGB(CurrentScreenshot);
                //CurrentScreenshot.Bitmap.Save("bmp.bmp");


                //Set new image screenshot time
                ImageUtils.UpdateTs();
            }

        }
    }
}
