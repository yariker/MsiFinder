// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Windows;
using System.Windows.Interop;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace MsiFinder.Views
{
    /// <summary>
    /// Interaction logic for ProductDetailsWindow.xaml.
    /// </summary>
    public partial class ProductDetailsWindow : Window
    {
        public ProductDetailsWindow()
        {
            InitializeComponent();
        }

        private static void HideMaximizeMinimizeButtons(IntPtr handle)
        {
            var style = (WINDOW_STYLE)GetWindowLong((HWND)handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            style &= ~WINDOW_STYLE.WS_MAXIMIZEBOX & ~WINDOW_STYLE.WS_MINIMIZEBOX;
            SetWindowLong((HWND)handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)style);
        }

        private static void HideWindowIcon(IntPtr handle)
        {
            const SET_WINDOW_POS_FLAGS refreshFrameFlags = SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                                                           SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                                                           SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                                                           SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED;

            var style = (WINDOW_EX_STYLE)GetWindowLong((HWND)handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            style |= WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME;
            SetWindowLong((HWND)handle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)style);

            SetWindowPos((HWND)handle, (HWND)IntPtr.Zero, 0, 0, 0, 0, refreshFrameFlags);

            SendMessage((HWND)handle, WM_SETICON, 0, 0);
            SendMessage((HWND)handle, WM_SETICON, 1, 0);
        }

        private void OnWindowSourceInitialized(object sender, EventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            HideMaximizeMinimizeButtons(handle);
            HideWindowIcon(handle);
        }
    }
}
