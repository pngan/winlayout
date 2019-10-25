using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

using System.Collections.Generic;

namespace winLayout
{
    

    class Program
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

        private const string USAGE = "Usage: winLayout [save | restore] [filename (optional)]";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(USAGE);
                return;
            }

            string folderName = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "WinLayout");
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            string filename = "winlayout.json";
            if (args.Length > 1)
            {
                filename = args[1];
                if (!filename.EndsWith(".json"))
                {
                    filename += ".json";
                }
            }
            string configFileName = Path.Combine(folderName, filename);

            var processes = Process.GetProcesses();
            if (string.Equals(args[0], "save", StringComparison.OrdinalIgnoreCase))
            {
                Dictionary<string, WINDOWPLACEMENT> winLayouts = new Dictionary<string, WINDOWPLACEMENT>();

                foreach (var process in processes)
                {
                    var name = process.ProcessName;
                    var windowHandle = process.MainWindowHandle;
                    if (windowHandle == (IntPtr)0) continue;

                    WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                    placement.length = Marshal.SizeOf(placement);
                    GetWindowPlacement(windowHandle, ref placement);
                    winLayouts[process.ProcessName] = placement;
                }

                var placementJson = JsonConvert.SerializeObject(winLayouts);
                File.WriteAllText(configFileName, placementJson);
            }
            else if (string.Equals(args[0], "restore", StringComparison.OrdinalIgnoreCase))
            {
                var plJson = File.ReadAllText(configFileName);
                var pl = JsonConvert.DeserializeObject<Dictionary<string, WINDOWPLACEMENT>>(plJson);

                foreach (var procName in pl.Keys)
                {
                    var procs = processes.Where(p => p.ProcessName == procName);
                    foreach (var proc in procs)
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
            else
            {
                Console.WriteLine(USAGE);
            }
        }
    }
}
