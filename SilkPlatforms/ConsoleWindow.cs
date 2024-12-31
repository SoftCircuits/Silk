// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Drawing;
using System.Runtime.InteropServices;
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
    public static partial class ConsoleWindow
    {

        #region Windows interop

        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AllocConsole();

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        //[LibraryImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static partial bool FreeConsole();

        /// <summary>
        /// Retrieves the window handle used by the console associated with the
        /// calling process.
        /// </summary>
        [LibraryImport("Kernel32.dll")]
        private static partial IntPtr GetConsoleWindow();

        /// <summary>
        /// Gets the console window title text.
        /// </summary>
        [LibraryImport("Kernel32.Dll", EntryPoint = "GetConsoleTitleW", StringMarshalling = StringMarshalling.Utf16)]
        private static partial int GetConsoleTitle([Out] char[] ptszTitle, int titleSize);

        /// <summary>
        /// Sets the console window title text.
        /// </summary>
        [LibraryImport("Kernel32.Dll", EntryPoint = "SetConsoleTitleW", StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetConsoleTitle(string title);

        /// <summary>
        /// Shows or hides the console window.
        /// </summary>
        [LibraryImport("User32.dll")]
        private static partial int ShowWindow(IntPtr hwnd, int nShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        /// <summary>
        /// Gets if the console window is visible.
        /// </summary>
        [LibraryImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsWindowVisible(IntPtr hWnd);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
            public readonly int Width => Right - Left;
            public readonly int Height => Bottom - Top;
        }

        [LibraryImport("user32.dll")]
        private static partial IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;

        /// <summary>
        /// Gets a handle to a window's system menu.
        /// </summary>
        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;

        /// <summary>
        /// Deletes a menu item.
        /// </summary>
        [LibraryImport("user32.dll")]
        private static partial int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        //private delegate bool SetConsoleCtrlEventHandler(uint ctrlType);

        //[LibraryImport("Kernel32")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static partial bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);

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

        //// static classes cannot have finalizers
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
                char[] buffer = new char[255];
                int length = GetConsoleTitle(buffer, buffer.Length);
                return new(buffer, 0, length);



                //StringBuilder builder = new(256);
                //int result = GetConsoleTitle(builder, builder.Capacity);
                //if (result == 0)
                //    throw new Exception("Unable to retrieve console window title.");
                //return builder.ToString();
            }
            set => SetConsoleTitle(value);

        }

        /// <summary>
        /// Gets or sets whether the console window is visible.
        /// </summary>
        public static bool Visible
        {
            get => IsWindowVisible(GetConsoleWindow());
            set => _ = ShowWindow(GetConsoleWindow(), value ? SW_SHOW : SW_HIDE);
        }

        /// <summary>
        /// Centers the console window on the screen.
        /// </summary>
        public static void CenterWindow()
        {
            IntPtr hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero && Screen.PrimaryScreen != null)
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
                    _ = DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
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
