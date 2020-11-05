using Server.Native;
using System;
using System.Drawing;
using System.Text;

namespace Server
{
    public class Window
    {
        public IntPtr Handle { get; }
        public Window Parent { get; }
        public string ClassName { get; }
        public string Name { get; }

        public Window(IntPtr handle, IntPtr parentHandle, string className = null, string name = null)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentOutOfRangeException(nameof(handle) + " cannot be zero");

            if (parentHandle != IntPtr.Zero)
            {
                Parent = new Window(parentHandle, NativeMethods.GetParent(parentHandle));
            }
            else
            {
                Parent = null;
            }

            Handle = handle;
            ClassName = !string.IsNullOrEmpty(className) ? className : GetWindowClassName(handle);
            Name = !string.IsNullOrEmpty(name) ? name : GetWindowName(handle);
        }

        public Window(IntPtr handle, Window parent, string className = null, string name = null)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentOutOfRangeException(nameof(handle) + " cannot be zero");

            Handle = handle;
            Parent = parent;
            ClassName = !string.IsNullOrEmpty(className) ? className : GetWindowClassName(handle);
            Name = !string.IsNullOrEmpty(name) ? name : GetWindowName(handle);
        }

        public Bitmap Capture()
        {
            return CaptureWindow(Handle);
        }
        public static Bitmap CaptureWindow(IntPtr handle)
        {
            //Logger.Debug("Capturing window with handle " + handle);

            // get te hDC of the target window
            var hdcSrc = NativeMethods.GetWindowDC(handle);

            // get the size
            NativeMethods.GetWindowRect(handle, out RECT windowRect);

            var width = windowRect.Right - windowRect.Left;
            var height = windowRect.Bottom - windowRect.Top;

            // create a device context we can copy to
            var hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);

            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            var hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, width, height);

            // select the bitmap object
            var hOld = NativeMethods.SelectObject(hdcDest, hBitmap);

            // bitblt over
            NativeMethods.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, NativeMethods.SRCCOPY);

            // restore selection
            NativeMethods.SelectObject(hdcDest, hOld);

            // clean up 
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Bitmap img = System.Drawing.Image.FromHbitmap(hBitmap);

            // free up the Bitmap object
            NativeMethods.DeleteObject(hBitmap);

            return img;
        }

        public override string ToString()
        {
            return $"{{Handle={Handle}, ClassName={ClassName}, Name={Name}}}";
        }

        /*
        private int GetDigitAtPosition(int input, int position)
        {
            return (int)Math.Floor((input - Math.Floor(input / Math.Pow(10, position + 1)) * Math.Pow(10, position + 1)) / Math.Pow(10, position));
        }*/

        public Window FindChildRecursively(string className, string windowName)
        {
            return FindChildRecursively(Handle, className, windowName);
        }

        private Window FindChildRecursively(IntPtr parent, string className, string windowName)
        {
            IntPtr child = NativeMethods.FindWindowEx(parent, IntPtr.Zero, null, null);

            //Logger.Log(className + " " + windowName);

            //Logger.Log("FindChildRecursively -> parent: " + parent.ToString("X"));

            while (child != IntPtr.Zero)
            {
                //Logger.Log("FindChildRecursively -> child: " + child.ToString("X"));
                Window found;

                if ((found = FindChildRecursively(child, className, windowName)) != null)
                {
                    return found;
                }

                if ((string.IsNullOrEmpty(className) || GetWindowClassName(child) == className) && (string.IsNullOrEmpty(windowName) || GetWindowName(child) == windowName || GetWindowName(child) == "Chrome Legacy Window"))
                {
                    return new Window(child, parent, className, windowName);
                }

                child = NativeMethods.FindWindowEx(parent, child, null, null);
            }

            return null;
        }

        private string GetWindowClassName(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentOutOfRangeException(nameof(handle) + " cannot be zero");

            StringBuilder sb = new StringBuilder(256);
            NativeMethods.GetClassName(handle, sb, sb.Capacity);
            return sb.ToString();
        }

        private string GetWindowName(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentOutOfRangeException(nameof(handle) + " cannot be zero");

            StringBuilder sb = new StringBuilder(256);
            NativeMethods.GetWindowText(handle, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
