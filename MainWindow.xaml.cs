using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LuxuryClock
{
    public partial class MainWindow : Window
    {
        private const string CurrentVersion = "v2.1.0";
        private DispatcherTimer _timer;
        private int _lastSecond = -1;
        private int _lastHour = -1;
        
        private System.Windows.Media.MediaPlayer _tickPlayer;
        private System.Windows.Media.MediaPlayer _hourPlayer;

        public MainWindow()
        {
            InitializeComponent();
            
            // Set up a timer to tick every 100 milliseconds to catch the second change precisely
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Initialize MediaPlayers for sounds
            _tickPlayer = new System.Windows.Media.MediaPlayer();
            // A soft short click from Windows media
            _tickPlayer.Open(new Uri(@"C:\Windows\Media\Windows Navigation Start.wav", UriKind.Absolute));
            _tickPlayer.Volume = 0.5; 

            _hourPlayer = new System.Windows.Media.MediaPlayer();
            // A relaxing chime from Windows media
            _hourPlayer.Open(new Uri(@"C:\Windows\Media\Windows Notify Calendar.wav", UriKind.Absolute));
            _hourPlayer.Volume = 1.0; 

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

            Task t = CheckForUpdatesAsync();
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

            // Play tick sound every second
            if (now.Second != _lastSecond)
            {
                if (_lastSecond != -1 && SoundToggleBtn.IsChecked == true) // Don't play on immediate startup
                {
                    _tickPlayer.Position = TimeSpan.Zero;
                    _tickPlayer.Play();
                }
                _lastSecond = now.Second;
            }

            // Play hour sound when hour changes
            if (now.Hour != _lastHour)
            {
                if (_lastHour != -1 && SoundToggleBtn.IsChecked == true) // Don't play on immediate startup
                {
                    _hourPlayer.Position = TimeSpan.Zero;
                    _hourPlayer.Play();
                }
                _lastHour = now.Hour;
            }
        }

        // Allows dragging the window by clicking anywhere on it
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Show resize grip and sound toggle on hover
        private void RootGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ResizeGrip.Visibility = Visibility.Visible;
            SoundToggleBtn.Visibility = Visibility.Visible;
        }

        private void RootGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ResizeGrip.Visibility = Visibility.Collapsed;
            SoundToggleBtn.Visibility = Visibility.Collapsed;
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

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "LuxuryClock-Updater");
                    string url = "https://api.github.com/repos/kamranshakib/Luxury-Clock/releases/latest";
                    string json = await client.GetStringAsync(url);

                    Match tagMatch = Regex.Match(json, "\"tag_name\"\\s*:\\s*\"([^\"]+)\"");
                    if (tagMatch.Success)
                    {
                        string latestVersion = tagMatch.Groups[1].Value;
                        if (latestVersion != CurrentVersion)
                        {
                            Match assetMatch = Regex.Match(json, "\"browser_download_url\"\\s*:\\s*\"([^\"]+\\.exe)\"");
                            if (assetMatch.Success)
                            {
                                string downloadUrl = assetMatch.Groups[1].Value;
                                await DownloadAndApplyUpdate(downloadUrl);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private async Task DownloadAndApplyUpdate(string url)
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDir = Path.GetDirectoryName(exePath);
                string newExePath = Path.Combine(exeDir, "LuxuryClock_Update.exe");
                string batPath = Path.Combine(exeDir, "update.bat");

                using (var client = new HttpClient())
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(url);
                    File.WriteAllBytes(newExePath, fileBytes);
                }

                string exeName = Path.GetFileName(exePath);
                string batContent = 
                    "@echo off\r\n" +
                    "ping 127.0.0.1 -n 3 > nul\r\n" +
                    "del /q \"" + exeName + "\"\r\n" +
                    "ren \"LuxuryClock_Update.exe\" \"" + exeName + "\"\r\n" +
                    "start \"\" \"" + exeName + "\"\r\n" +
                    "del \"%~f0\"";

                File.WriteAllText(batPath, batContent);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = batPath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = exeDir
                };
                Process.Start(psi);

                Application.Current.Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
            }
            catch { }
        }
    }
}
