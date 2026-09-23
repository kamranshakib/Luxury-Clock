using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

namespace LuxuryClock
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            
            // Set up a timer to tick every 100 milliseconds to catch the second change precisely
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Initial update
            UpdateTime();

            this.Loaded += MainWindow_Loaded;
            this.Activated += MainWindow_Activated;

            SetStartup();
        }

        private void SetStartup()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string appName = "LuxuryClockV2";
                string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string runValue = "\"" + appPath + "\"";
                
                if (rk.GetValue(appName) == null || rk.GetValue(appName).ToString() != runValue)
                {
                    rk.SetValue(appName, runValue);
                }
            }
            catch { }
        }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOACTIVATE = 0x0010;

        private void SendToBottom()
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = 0;
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;

            SendToBottom();
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            SendToBottom();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            DateTime now = DateTime.Now;
            
            TimeText.Text = now.ToString("HH:mm");
            SecondsText.Text = now.ToString("ss");
        }

        // Allows dragging the window by clicking anywhere on it
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Show resize grip on hover
        private void RootGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ResizeGrip.Visibility = Visibility.Visible;
        }

        private void RootGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ResizeGrip.Visibility = Visibility.Collapsed;
        }

        // Handle custom resizing from the thumb
        private void ResizeGrip_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newWidth = this.Width + e.HorizontalChange;
            double newHeight = this.Height + e.VerticalChange;
            
            if (newWidth >= this.MinWidth)
                this.Width = newWidth;
                
            if (newHeight >= this.MinHeight)
                this.Height = newHeight;
        }
    }
}
