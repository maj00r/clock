using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Clock
{
    internal static class WindowPresenterService
    {
        public static Window PreventResizing(this Window window)
        {
            var presenter = window.AppWindow.Presenter as OverlappedPresenter;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;

            return window;
        }

        public static Window RemoveBorders(this Window window) {
            var presenter = window.AppWindow.Presenter as OverlappedPresenter;
            presenter.SetBorderAndTitleBar(true, false);
            var windowHandle = Win32Interop.GetWindowFromWindowId(window.AppWindow.Id);

            //DWM_WINDOW_CORNER_PREFERENCE documentation gives DWMWCP_ROUND as 2
            int cornerPreference = 2;
            int result = NativeApi.DwmSetWindowAttribute(windowHandle, 33, ref cornerPreference, Marshal.SizeOf<int>());
            Marshal.ThrowExceptionForHR(result);
            return window;
        }

        public static Window SetTransparency(this Window window, double percentage)
        {
            var windowHandle = Win32Interop.GetWindowFromWindowId(window.AppWindow.Id);
            // Get the current extended style
            var extendedStyle = NativeApi.GetWindowLong(windowHandle, NativeApi.GWL_EXSTYLE);

            extendedStyle = (extendedStyle | NativeApi.WS_EX_TOOLWINDOW) & ~NativeApi.WS_EX_APPWINDOW;

            // Add the WS_EX_LAYERED style to make the window layered
            NativeApi.SetWindowLong(windowHandle, NativeApi.GWL_EXSTYLE, extendedStyle | NativeApi.WS_EX_LAYERED | NativeApi.WS_EX_TRANSPARENT);

            // Set the window transparency to 80% (204 out of 255)
            byte transparency = (byte)(255 * percentage); // 80% transparency
            bool result = NativeApi.SetLayeredWindowAttributes(windowHandle, 0, transparency, NativeApi.LWA_ALPHA);
            return window;

        }

        public static Window SetTopMost(this Window window)
        {
            var windowHandle = Win32Interop.GetWindowFromWindowId(window.AppWindow.Id);
            NativeApi.SetWindowPos(windowHandle, NativeApi.HWND_TOPMOST, 0, 0, 0, 0,
            NativeApi.SWP_NOMOVE | NativeApi.SWP_NOSIZE | NativeApi.SWP_NOACTIVATE | NativeApi.SWP_SHOWWINDOW);
            return window;
        }
    }
    public partial class NativeApi
    {
        //Platform invoke definition for DwmSetWindowAttribute
        [LibraryImport("dwmapi.dll")]
        public static partial int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        // Constants for window styles and attributes
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        // Importing GetWindowLongPtr function to retrieve current window styles
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);


        // Importing SetLayeredWindowAttributes function to set transparency
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    }
}
