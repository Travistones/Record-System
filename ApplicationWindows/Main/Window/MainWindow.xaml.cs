using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;
using Windows.UI;
using Record_System.Pages.Container.Main.Page;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Record_System.ApplicationWindows.Main.Window
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
    {
        private AppWindow _appWindow;

        IntPtr hWnd = IntPtr.Zero;

        private SUBCLASSPROC SubClassDelegate;

        public static Frame RootNavigationFrameReference;

        public static Microsoft.UI.Xaml.Window MainWindowObject;

        public MainWindow()
        {
            this.Title = "Record System";

            MainWindowObject = this;

            this.InitializeComponent();

            RootNavigationFrameReference = RootNavigationFrame;

            hWnd = WindowNative.GetWindowHandle(this);

            _appWindow = GetAppWindowForCurrentWindow();

            InitializeUserInterface();

            InitializeTitleBar();

            SetWindowMinimumSize();
        }

        private void InitializeUserInterface()
        {
            this.RootNavigationFrame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }

        private void InitializeTitleBar()
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = _appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                ApplicationMainTitleBar.Loaded += ApplicationMainTitleBar_Loaded;
                ApplicationMainTitleBar.SizeChanged += ApplicationMainTitleBar_SizeChanged;
            }
            else
            {
                ApplicationMainTitleBar.Visibility = Visibility.Collapsed;
            }

            SetTitleBarTheme();
        }

        private void ApplicationMainTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported()
            && _appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                // Update drag region if the size of the title bar changes.
                SetDragRegionForCustomTitleBar(_appWindow);
            }
        }

        private void ApplicationMainTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                SetDragRegionForCustomTitleBar(_appWindow);
            }
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);

            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);

            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);

            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);

            if (result != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }
            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);

            return scaleFactorPercent / 100.0;
        }

        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            // Check to see if customization is supported.
            // The method returns true on Windows 10 since Windows App SDK 1.2, and on all versions of
            // Windows App SDK on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported()
            && appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {

                double scaleAdjustment = GetScaleAdjustment();

                RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);

                LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);
                
                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRectL;

                dragRectL.X = (int)(LeftPaddingColumn.ActualWidth * scaleAdjustment);

                dragRectL.Y = 0;

                dragRectL.Height = (int)(ApplicationMainTitleBar.ActualHeight * scaleAdjustment);

                dragRectL.Width = (int)(titleBarArea.ActualWidth * scaleAdjustment);

                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                appWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }

        private void SetTitleBarTheme()
        {
            _appWindow.TitleBar.ButtonForegroundColor = (Color)Application.Current.Resources["TitleBarButtonsForegroundColor"];

            _appWindow.TitleBar.ButtonBackgroundColor = (Color)Application.Current.Resources["TransparentColor"];

            _appWindow.TitleBar.ButtonInactiveBackgroundColor = (Color)Application.Current.Resources["TransparentColor"];

            _appWindow.TitleBar.ButtonInactiveForegroundColor = (Color)Application.Current.Resources["TitleBarButtonsInActiveForegroundColor"];

            _appWindow.TitleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["WhiteColor5"];

            _appWindow.TitleBar.ButtonHoverForegroundColor = (Color)Application.Current.Resources["WhiteColor50"];

            _appWindow.TitleBar.ButtonPressedBackgroundColor = (Color)Application.Current.Resources["WhiteColor5"];

            _appWindow.TitleBar.ButtonPressedForegroundColor = (Color)Application.Current.Resources["TitleBarButtonsForegroundColor"];
        }

        private void SetWindowMinimumSize()
        {
            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);

            bool bReturn = SetWindowSubclass(hWnd, SubClassDelegate, 0, 0);
        }

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case WM_GETMINMAXINFO:
                    {
                        MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
                        mmi.ptMinTrackSize.X = 700;
                        mmi.ptMinTrackSize.Y = 530;
                        Marshal.StructureToPtr(mmi, lParam, false);
                        return 0;
                    }
            }

            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }


        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public const int WM_GETMINMAXINFO = 0x0024;

        public struct MINMAXINFO
        {
            public System.Drawing.Point ptReserved;
            public System.Drawing.Point ptMaxSize;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Point ptMinTrackSize;
            public System.Drawing.Point ptMaxTrackSize;
        }

    }
}
