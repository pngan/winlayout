using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace winlayout_ui
{
    public partial class WinLayoutForm : Form
    {
        const int SW_SHOWNOACTIVATE = 4;
        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        private Dictionary<string, WINDOWPLACEMENT> WinLayouts { get; } = new Dictionary<string, WINDOWPLACEMENT>();
        private Process[] Processes { get; set; }
        private string ConfigFileName { get; set; }


        public WinLayoutForm()
        {
            InitializeComponent();

            restoreButton.Enabled = false;
            saveButton.Enabled = false;
            this.Load += WinLayoutForm_Load;
        }

        private void WinLayoutForm_Load(object sender, EventArgs e)
        {
            string folderName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "WinLayout");
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            ConfigFileName = Path.Combine(folderName, "winlayout.json");

            Processes = Process.GetProcesses();

            if (File.Exists(ConfigFileName))
                restoreButton.Enabled = true;

            saveButton.Enabled = true;
        }


        private void restoreButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ConfigFileName))
                return;

            var plJson = File.ReadAllText(ConfigFileName);
            var pl = JsonConvert.DeserializeObject<Dictionary<string, WINDOWPLACEMENT>>(plJson);

            foreach (var procName in pl.Keys)
            {
                var processes = Processes.Where(p => p.ProcessName == procName);
                foreach (var proc in processes)
                {
                    if (proc == null) continue;
                    var windowHandle = proc.MainWindowHandle;
                    if (windowHandle == (IntPtr)0) continue;

                    // When doing dock/undock, the windowplacement is not recognized as changed
                    // so force a small change by deactivating the windows
                    var winPlacement2 = pl[proc.ProcessName];
                    winPlacement2.showCmd = 1;
                    SetWindowPlacement(windowHandle, ref winPlacement2);

                    var winPlacement = pl[proc.ProcessName];
                    SetWindowPlacement(windowHandle, ref winPlacement);
                    break;
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            foreach (var process in Processes)
            {
                var name = process.ProcessName;
                var windowHandle = process.MainWindowHandle;

                Console.WriteLine($"Process: {process.ProcessName}, {windowHandle}");

                if (windowHandle == (IntPtr)0) continue;

                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                GetWindowPlacement(windowHandle, ref placement);
                WinLayouts[process.ProcessName] = placement;
            }

            var placementJson = JsonConvert.SerializeObject(WinLayouts);
            File.WriteAllText(ConfigFileName, placementJson);

            restoreButton.Enabled = true;
            this.AcceptButton = saveButton;
        }
    }
}
