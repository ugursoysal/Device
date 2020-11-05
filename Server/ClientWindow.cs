using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server
{
    public enum ClientStatus
    {
        OnLoginScreen,
        DialogVisible,
        Unknown
    }

    public class ClientWindow : Window
    {
        public bool IsMatch => Status != ClientStatus.Unknown;

        public ClientStatus Status { get; private set; }
        public Rectangle UsernameBox { get; private set; }
        public Rectangle PasswordBox { get; private set; }
        public Rectangle DialogBox { get; private set; }
        public Window InnerWindow { get; }
        public double Scale { get; set; }

        private ClientWindow(IntPtr handle, Window parent, string className, string windowName, Window innerWindow) : base(handle, parent, className, windowName)
        {
            InnerWindow = innerWindow;
        }

        public void RefreshStatus()
        {

            Status = ClientStatus.Unknown;
            UsernameBox = Rectangle.Empty;
            PasswordBox = Rectangle.Empty;
            DialogBox = Rectangle.Empty;

            Bitmap capture = Capture();

            List<Rectangle> rectangles = FindRectangles(capture);

            Scale = capture.Width / capture.Height;
            var rect1 = (rectangles.Count != 0) ? rectangles[0] : new Rectangle(12, 207, 173, 25); // 1024x768
            Status = ClientStatus.OnLoginScreen;
            DialogBox = new Rectangle((int)(Scale * 183), (int)(Scale * 525), (int)(Scale * 2), (int)(Scale * 2));
            UsernameBox = new Rectangle(rect1.X, rect1.Y, rect1.Width, rect1.Height);
            PasswordBox = new Rectangle(UsernameBox.X, UsernameBox.Y + (int)(65 * Scale), UsernameBox.Width, UsernameBox.Height);

            capture.Dispose();
        }
        public static List<Rectangle> FindRectangles(Bitmap source)
        {
            Bitmap canny = Grayscale.CommonAlgorithms.RMY.Apply(source);
            CannyEdgeDetector edgeDetector = new CannyEdgeDetector(5, 20);
            edgeDetector.ApplyInPlace(canny);

            BlobCounter blobCounter = new BlobCounter
            {
                FilterBlobs = true,
                MinWidth = 5,
                MinHeight = 5
            };

            blobCounter.ProcessImage(canny);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            List<Rectangle> rectangles = new List<Rectangle>();

            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            for (int i = 0; i < blobs.Length; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                if (shapeChecker.IsConvexPolygon(edgePoints, out List<IntPoint> corners))
                    if (blobs[i].Rectangle.Width > canny.Width / 5.3 && blobs[i].Rectangle.Width < canny.Width / 4.41 && canny.Height / 10.3 > blobs[i].Rectangle.Height && blobs[i].Rectangle.Height > canny.Height / 20.5)
                        rectangles.Add(blobs[i].Rectangle);
            }

            // order by descending area
            rectangles.Sort((a, b) =>
            {
                if (a.Width * a.Height > b.Width * b.Height)
                    return 1;
                else if (a.Width * a.Height < b.Width * b.Height)
                    return -1;
                return 0;
            });

            return rectangles;
        }

        public static ClientWindow FromWindow(Window window, Window innerWindow)
        {
            var result = new ClientWindow(window.Handle, window.Parent, window.ClassName, window.Name, innerWindow);

            try
            {
                result.RefreshStatus();
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to refresh client window status (w: {window.Name},{window.ClassName} iw: {innerWindow.Name},{innerWindow.ClassName}" + Environment.NewLine + ex.Message);
            }

            return result;
        }
    }
}
