// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SilkPlatforms
{
    /// <summary>
    /// Manages a dynamic console window within a WinForms app. Note: If AllocConsole()
    /// and FreeConsole() are called multiple times, the Console object reports that
    /// the handle is invalid. Unless we can figure out how to property reset the
    /// Console object, we need to only create one console window during the application
    /// lifetime.
    /// </summary>
    public static class ConsoleWindow
    {

        #region Windows interop

        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        /// <summary>
        /// Retrieves the window handle used by the console associated with the
        /// calling process.
        /// </summary>
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Gets the console window title text.
        /// </summary>
        [DllImport("Kernel32.Dll", CharSet = CharSet.Unicode)]
        private static extern int GetConsoleTitle(StringBuilder titleBuilder, int titleSize);

        /// <summary>
        /// Sets the console window title text.
        /// </summary>
        [DllImport("Kernel32.Dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleTitle(string title);

        /// <summary>
        /// Shows or hides the console window.
        /// </summary>
        [DllImport("User32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int nShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        /// <summary>
        /// Gets if the console window is visible.
        /// </summary>
        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
            public int Width => Right - Left;
            public int Height => Bottom - Top;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;

        /// <summary>
        /// Gets a handle to a window's system menu.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;

        /// <summary>
        /// Deletes a menu item.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        //private delegate bool SetConsoleCtrlEventHandler(uint ctrlType);

        //[DllImport("Kernel32")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);

        //private const int CTRL_C_EVENT = 0;
        //private const int CTRL_BREAK_EVENT = 1;
        //private const int CTRL_CLOSE_EVENT = 2;
        //private const int CTRL_LOGOFF_EVENT = 5;
        //private const int CTRL_SHUTDOWN_EVENT = 6;

        #endregion

        static ConsoleWindow()
        {
            if (!AllocConsole())
                throw new Exception("Unable to create console window.");

            //if (!SetConsoleCtrlHandler(ConsoleCtrlHandler, true))
            //    throw new Exception("Unable to set console control handler.");
        }

        // static classes cannot have finalizers
        //~ConsoleWindow()
        //{
        //    if (Handle != (IntPtr)0)
        //    {
        //        if (!FreeConsole())
        //            throw new Exception("Unable to free console window.");
        //    }
        //}

        /// <summary>
        /// Gets or sets the console window title text.
        /// </summary>
        public static string Title
        {
            get
            {
                StringBuilder builder = new(256);
                int result = GetConsoleTitle(builder, builder.Capacity);
                if (result == 0)
                    throw new Exception("Unable to retrieve console window title.");
                return builder.ToString();
            }
            set => SetConsoleTitle(value);

        }

        /// <summary>
        /// Gets or sets whether the console window is visible.
        /// </summary>
        public static bool Visible
        {
            get => IsWindowVisible(GetConsoleWindow());
#pragma warning disable CA1806 // Do not ignore method results
            set => ShowWindow(GetConsoleWindow(), value ? SW_SHOW : SW_HIDE);
#pragma warning restore CA1806 // Do not ignore method results
        }

        /// <summary>
        /// Centers the console window on the screen.
        /// </summary>
        public static void CenterWindow()
        {
            IntPtr hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                Rectangle screen = Screen.PrimaryScreen.WorkingArea;
                if (GetWindowRect(hWnd, out RECT window))
                    SetWindowPos(hWnd, 0,
                        (screen.Width - window.Width) / 2,
                        (screen.Height - window.Height) / 2,
                        0, 0,
                        SWP_NOSIZE | SWP_NOZORDER);
            }
        }

        /// <summary>
        /// Prevents the user from closing the console window.
        /// </summary>
        public static void DisableCloseButton()
        {
            IntPtr hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                IntPtr hMenu = GetSystemMenu(hWnd, false);
                if (hMenu != IntPtr.Zero)
#pragma warning disable CA1806 // Do not ignore method results
                    DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
#pragma warning restore CA1806 // Do not ignore method results
            }
        }

        //private static bool ConsoleCtrlHandler(uint ctrlType)
        //{
        //    return ctrlType switch
        //    {
        //        CTRL_BREAK_EVENT or CTRL_C_EVENT or CTRL_LOGOFF_EVENT or CTRL_SHUTDOWN_EVENT => false,
        //        CTRL_CLOSE_EVENT => true,
        //        _ => false,
        //    };
        //}
    }
}
