using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace LuxuryClockSetup
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SetupWizard());
        }
    }

    public class SetupWizard : Form
    {
        private Panel panelWelcome, panelPath, panelFinish;
        private TextBox txtPath;
        private CheckBox chkDesktopShortcut, chkStartMenuShortcut, chkLaunch;

        public SetupWizard()
        {
            this.Text = "Luxury Clock V2 Setup";
            this.Size = new Size(500, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializePanels();
            ShowPanel(panelWelcome);
        }

        private void InitializePanels()
        {
            // --- Welcome Panel ---
            panelWelcome = new Panel { Dock = DockStyle.Fill };
            panelWelcome.Controls.Add(new Label { 
                Text = "Welcome to the Luxury Clock Setup Wizard\n\nThis will install Luxury Clock V2 on your computer.\n\nClick Next to continue.", 
                Location = new Point(30, 50), 
                Size = new Size(400, 150), 
                Font = new Font("Segoe UI", 12) 
            });
            Button btnNext1 = new Button { Text = "Next >", Location = new Point(380, 270) };
            btnNext1.Click += (s, e) => ShowPanel(panelPath);
            panelWelcome.Controls.Add(btnNext1);

            // --- Path Panel ---
            panelPath = new Panel { Dock = DockStyle.Fill };
            panelPath.Controls.Add(new Label { 
                Text = "Select Destination Location:\nWhere should Luxury Clock be installed?", 
                Location = new Point(30, 30), 
                Size = new Size(400, 40), 
                Font = new Font("Segoe UI", 10) 
            });
            
            txtPath = new TextBox { 
                Location = new Point(30, 80), 
                Size = new Size(330, 25), 
                Font = new Font("Segoe UI", 10) 
            };
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LuxuryClock");
            txtPath.Text = defaultPath;
            panelPath.Controls.Add(txtPath);

            Button btnBrowse = new Button { Text = "Browse...", Location = new Point(370, 79) };
            btnBrowse.Click += (s, e) => {
                using (var fbd = new FolderBrowserDialog()) {
                    fbd.SelectedPath = txtPath.Text;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        if (!fbd.SelectedPath.EndsWith("LuxuryClock"))
                            txtPath.Text = Path.Combine(fbd.SelectedPath, "LuxuryClock");
                        else
                            txtPath.Text = fbd.SelectedPath;
                    }
                }
            };
            panelPath.Controls.Add(btnBrowse);

            chkDesktopShortcut = new CheckBox { Text = "Create a desktop shortcut", Location = new Point(30, 130), Size = new Size(300, 25), Checked = true };
            chkStartMenuShortcut = new CheckBox { Text = "Create a start menu shortcut", Location = new Point(30, 160), Size = new Size(300, 25), Checked = true };
            panelPath.Controls.Add(chkDesktopShortcut);
            panelPath.Controls.Add(chkStartMenuShortcut);

            Button btnBack2 = new Button { Text = "< Back", Location = new Point(290, 270) };
            btnBack2.Click += (s, e) => ShowPanel(panelWelcome);
            Button btnInstall = new Button { Text = "Install", Location = new Point(380, 270) };
            btnInstall.Click += (s, e) => DoInstall();
            panelPath.Controls.Add(btnBack2);
            panelPath.Controls.Add(btnInstall);

            // --- Finish Panel ---
            panelFinish = new Panel { Dock = DockStyle.Fill };
            panelFinish.Controls.Add(new Label { 
                Text = "Setup has finished installing Luxury Clock on your computer.", 
                Location = new Point(30, 50), 
                Size = new Size(400, 80), 
                Font = new Font("Segoe UI", 12) 
            });
            chkLaunch = new CheckBox { 
                Text = "Launch Luxury Clock", 
                Location = new Point(30, 140), 
                Size = new Size(300, 25), 
                Checked = true, 
                Font = new Font("Segoe UI", 10) 
            };
            panelFinish.Controls.Add(chkLaunch);
            
            Button btnFinish = new Button { Text = "Finish", Location = new Point(380, 270) };
            btnFinish.Click += (s, e) => {
                if (chkLaunch.Checked)
                {
                    Process.Start(new ProcessStartInfo(Path.Combine(txtPath.Text, "LuxuryClock.exe")) { UseShellExecute = true });
                }
                Application.Exit();
            };
            panelFinish.Controls.Add(btnFinish);

            this.Controls.Add(panelWelcome);
            this.Controls.Add(panelPath);
            this.Controls.Add(panelFinish);
        }

        private void ShowPanel(Panel p)
        {
            panelWelcome.Visible = false;
            panelPath.Visible = false;
            panelFinish.Visible = false;
            p.Visible = true;
        }

        private void DoInstall()
        {
            try
            {
                string installDir = txtPath.Text;
                if (!Directory.Exists(installDir)) Directory.CreateDirectory(installDir);

                string exePath = Path.Combine(installDir, "LuxuryClock.exe");
                ExtractResource("Setup.LuxuryClock.exe", exePath);

                string iconPath = Path.Combine(installDir, "icon.ico");
                ExtractResource("Setup.icon.ico", iconPath);

                if (chkDesktopShortcut.Checked || chkStartMenuShortcut.Checked)
                {
                    CreateShortcuts(exePath, iconPath);
                }

                ShowPanel(panelFinish);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Installation failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtractResource(string resourceName, string outPath)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (s == null) throw new Exception("Could not find embedded resource: " + resourceName);
                using (FileStream fs = new FileStream(outPath, FileMode.Create))
                {
                    s.CopyTo(fs);
                }
            }
        }

        private void CreateShortcuts(string targetPath, string iconPath)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string startMenu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
            
            string vbsPath = Path.Combine(Path.GetTempPath(), "shortcut.vbs");
            string vbsCode = "Set oWS = WScript.CreateObject(\"WScript.Shell\")\r\n";
            
            if (chkDesktopShortcut.Checked)
            {
                string shortcutPath = Path.Combine(desktop, "Luxury Clock.lnk");
                vbsCode += string.Format("Set oLink = oWS.CreateShortcut(\"{0}\")\r\n", shortcutPath);
                vbsCode += string.Format("oLink.TargetPath = \"{0}\"\r\n", targetPath);
                vbsCode += string.Format("oLink.IconLocation = \"{0}\"\r\n", iconPath);
                vbsCode += string.Format("oLink.WorkingDirectory = \"{0}\"\r\n", Path.GetDirectoryName(targetPath));
                vbsCode += "oLink.Save()\r\n";
            }
            if (chkStartMenuShortcut.Checked)
            {
                string startMenuShortcut = Path.Combine(startMenu, "Luxury Clock.lnk");
                vbsCode += string.Format("Set oLink2 = oWS.CreateShortcut(\"{0}\")\r\n", startMenuShortcut);
                vbsCode += string.Format("oLink2.TargetPath = \"{0}\"\r\n", targetPath);
                vbsCode += string.Format("oLink2.IconLocation = \"{0}\"\r\n", iconPath);
                vbsCode += string.Format("oLink2.WorkingDirectory = \"{0}\"\r\n", Path.GetDirectoryName(targetPath));
                vbsCode += "oLink2.Save()\r\n";
            }
            
            File.WriteAllText(vbsPath, vbsCode);
            
            Process process = Process.Start(new ProcessStartInfo("cscript.exe", string.Format("//Nologo \"{0}\"", vbsPath))
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
            process.WaitForExit();
            if (File.Exists(vbsPath)) File.Delete(vbsPath);
        }
    }
}
