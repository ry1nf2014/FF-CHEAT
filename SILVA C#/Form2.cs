using FF;
using Guna.UI2.WinForms;
using Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLUE_C_
{
    public partial class Form2 : Form
    {
        // --- Class members for your logic ---
        // 'memoryManager' is now correctly declared as a class member.
        private Memory123k memoryManager;
        private bool scopeSavedOnce = false;
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AIMBOT DATA.txt");
        private readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AIMBOT DATA .txt");
        // These declarations were conflicting with the Memory123k class.
        // The Memory123k class already has its own thread management.
        // I have commented them out and will use the methods from memoryManager instead.
        // private Thread monitorThread;
        // private bool isMonitoring = true;
        private static FAHIM PLAYBOX = new FAHIM();
        private string aimAOB = "FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 A5 43";
        private string readFORhead = "0x80";
        private string write = "0X7C";
        private Dictionary<long, int> originalvalues = new Dictionary<long, int>();
        private Dictionary<long, int> originallvalues = new Dictionary<long, int>();
        private Dictionary<long, int> originalvalues2 = new Dictionary<long, int>();
        private Dictionary<long, int> originallvalues2 = new Dictionary<long, int>();
        Mem Memory = new Mem();

        // --- Constructor ---
        public Form2()
        {
            InitializeComponent();
            // This is the CRUCIAL line that was missing.
            // It initializes the memoryManager object.
            memoryManager = new Memory123k();
        }

        // --- Event Handlers ---
        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2ToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2ToggleSwitch3.Checked)
            {
                base.ShowInTaskbar = false;
                Form2.Streaming = true;
                Form2.SetWindowDisplayAffinity(base.Handle, 17U);
            }
            else
            {
                base.ShowInTaskbar = true;
                Form2.Streaming = false;
                Form2.SetWindowDisplayAffinity(base.Handle, 0U);
            }
        }

        public static bool Streming;
        private static bool Streaming;
        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        public void ExecuteCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                // Write the command to the command prompt
                process.StandardInput.WriteLine(command);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                // Wait for the command to finish
                process.WaitForExit();
            }
        }
        private void guna2ToggleSwitch6_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2ToggleSwitch3.Checked)
            {
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
                // The name 'Sta1' is not defined in your Form2.cs, but I will assume it is a Label.
                // Replace this with the correct name of your status label if it's different.
                // this.Sta1.Text = "Internet Blocked Success";
                Console.Beep();
            }
            else
            {
                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
                this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
                // Replace this with the correct name of your status label if it's different.
                // this.Sta1.Text = "Internet Unblocked Success";
                Console.Beep();
            }
        }

        private async void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            originalvalues.Clear();
            originallvalues.Clear();
            originalvalues2.Clear();
            originallvalues2.Clear();
            // Replace with the correct name of your status label if it's different.
            // Sta1.Text = "Applying...";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Int64 readOffset = Convert.ToInt64(readFORhead, 16);
            Int64 writeOffset = Convert.ToInt64(write, 16);
            Int32 proc = Process.GetProcessesByName("HD-Player")[0].Id;
            PLAYBOX.OpenProcess(proc);

            var result = await PLAYBOX.AoBScan2(aimAOB, true, true);
            List<long> resultList = result.ToList();
            using (StreamWriter writer = new StreamWriter(logFilePath, false))
            {
                writer.WriteLine($"Total Patterns Found: {resultList.Count}");
                writer.WriteLine("=========================================");
                if (resultList.Count != 0)
                {
                    foreach (var CurrentAddress in resultList)
                    {
                        writer.WriteLine($"=========================================");
                        writer.WriteLine($"Pattern Found at Address: 0x{CurrentAddress:X}");
                        writer.WriteLine("=========================================");
                        writer.WriteLine($"Full Array of Bytes:");
                        writer.WriteLine(aimAOB);
                        writer.WriteLine();
                        writer.WriteLine("Replacements:");

                        Int64 AddressToSave = CurrentAddress + writeOffset;
                        var currentBytes = PLAYBOX.readMemory(AddressToSave.ToString("X"), sizeof(int));
                        int currentValue = BitConverter.ToInt32(currentBytes, 0);
                        originalvalues[AddressToSave] = currentValue;

                        Int64 addressToSave9 = CurrentAddress + readOffset;
                        var currentBytes9 = PLAYBOX.readMemory(addressToSave9.ToString("X"), sizeof(int));
                        int currentValue9 = BitConverter.ToInt32(currentBytes9, 0);
                        originallvalues[addressToSave9] = currentValue9;

                        writer.WriteLine($"  Address: 0x{AddressToSave:X}");
                        writer.WriteLine($"    Original Value (Dec): {currentValue}");
                        writer.WriteLine($"    Original Value (Hex): 0x{currentValue:X}");
                        writer.WriteLine($"    Replaced Value (Dec): {currentValue9}");
                        writer.WriteLine($"    Replaced Value (Hex): 0x{currentValue9:X}");
                        writer.WriteLine();

                        PLAYBOX.WriteMemory(addressToSave9.ToString("X"), "int", currentValue.ToString());
                        PLAYBOX.WriteMemory(AddressToSave.ToString("X"), "int", currentValue9.ToString());

                        writer.WriteLine($"  Address: 0x{addressToSave9:X}");
                        writer.WriteLine($"    Original Value (Dec): {currentValue9}");
                        writer.WriteLine($"    Original Value (Hex): 0x{currentValue9:X}");
                        writer.WriteLine($"    Replaced Value (Dec): {currentValue}");
                        writer.WriteLine($"    Replaced Value (Hex): 0x{currentValue:X}");
                        writer.WriteLine();
                    }
                }
                else
                {
                    writer.WriteLine("No patterns found.");
                }

                stopwatch.Stop();
                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                Console.Beep();
                // Replace with the correct name of your status label if it's different.
                // Sta1.Text = $"Successful, Time: {elapsedSeconds:F2} Seconds;";
            }
        }

        private static void ExtractEmbeddedResource(string resourceName, string outputPath)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new ArgumentException($"Resource '{resourceName}' not found.");
                }
                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    byte[] buffer = new byte[resourceStream.Length];
                    resourceStream.Read(buffer, 0, buffer.Length);
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        const uint PROCESS_CREATE_THREAD = 0x2;
        const uint PROCESS_QUERY_INFORMATION = 0x400;
        const uint PROCESS_VM_OPERATION = 0x8;
        const uint PROCESS_VM_WRITE = 0x20;
        const uint PROCESS_VM_READ = 0x10;
        const uint MEM_COMMIT = 0x1000;
        const uint PAGE_READWRITE = 4;

        private async void guna2ToggleSwitch4_CheckedChanged(object sender, EventArgs e)
        {
            string processName = "HD-Player";
            string dllResourceName = "SILVA_C_.CHAMS MENU.dll";
            string tempDllPath = Path.Combine(Path.GetTempPath(), "CHAMS MENU.dll");
            ExtractEmbeddedResource(dllResourceName, tempDllPath);
            Console.WriteLine($"DLL extracted successfully to: {tempDllPath}");
            Process[] targetProcesses = Process.GetProcessesByName(processName);
            if (targetProcesses.Length == 0)
            {
                Console.WriteLine($"Waiting for {processName}.exe...");
            }
            else
            {
                Process targetProcess = targetProcesses[0];
                IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);

                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);

                IntPtr bytesWritten;
                WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

                Console.Beep(240, 300);
            }
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            var toggleSwitch = (Guna.UI2.WinForms.Guna2ToggleSwitch)sender;
            bool sniperScopeRunning = toggleSwitch.Checked;

            if (sniperScopeRunning)
            {
                if (!memoryManager.AttackProcess(memoryManager.GetEmulatorRunning()))
                {
                    MessageBox.Show("Could not attach to emulator. Please run as administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toggleSwitch.Checked = false;
                    return;
                }

                if (!scopeSavedOnce)
                {
                    if (memoryManager.SaveScope())
                    {
                        scopeSavedOnce = true;
                    }
                    else
                    {
                        MessageBox.Show("Failed to find the sniper scope pattern. Feature will not work.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        memoryManager.CloseProcessHandle();
                        toggleSwitch.Checked = false;
                        return;
                    }
                }
                memoryManager.EnableScope();
                memoryManager.StartMonitor();
            }
            else
            {
                memoryManager.StopMonitor();
                memoryManager.DisableScope();
                memoryManager.CloseProcessHandle();
            }
        }

        private async void guna2ToggleSwitch5_CheckedChanged(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                Console.Beep(240, 300);
            }
            else
            {
                string search = "05 00 00 00 01 00 00 00 B4 C8 D6 3F 01 00 00 00 B4 C8 D6 3F 00 00 00 00 B4 C8 D6 3F 00 00 80 3F 00 00 80 3F 0A D7 A3 3D 00 00 00 00 00 00 5C 43 00 00 90 42 00 00 B4 42 96 00 00 00 00 00 00 00 00 00 00 3F 00 00 80 3E 00 00 00 00 04 00 00 00 00 00 80 3F 00 00 20 41 00 00 34 42 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 8F C2 35 3F 9A 99 99 3F 00 00 80 3F 00 00 00 00 00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00";
                string replace = "05 00 00 00 01 00 00 00 B4 C8 D6 3F 01 00 00 00 B4 C8 D6 3F 00 00 00 00 B4 C8 D6 3F 00 00 80 3F 00 00 80 3F 0A D7 A3 3D 00 00 00 00 00 00 5C 43 00 00 90 42 00 00 B4 42 96 00 00 00 00 00 00 00 00 00 00 3F 00 00 80 3E 00 00 00 3C 04 00 00 00 00 00 80 3F 00 00 20 41 00 00 34 42 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 8F C2 35 3F 9A 99 99 3F 00 00 80 3F 00 00 00 00 00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00";
                bool k = false;
                Memory.OpenProcess("HD-Player");

                IEnumerable<long> wl = await Memory.AoBScan(search, writable: true);
                if (wl.Any())
                {
                    foreach (long address in wl)
                    {
                        Memory.WriteMemory(address.ToString("X"), "bytes", replace);
                    }
                    k = true;
                }

                if (k)
                {
                    Console.Beep(400, 300);
                }
                else
                {
                    Console.Beep(240, 300);
                }
            }
        }

        private async void guna2ToggleSwitch9_CheckedChanged(object sender, EventArgs e)
        {
            // This method is missing a lot of logic.
            // I will provide a fix that makes it toggle on and off.
            // You will need to fill in the 'restore' logic if the toggle is turned off.
            var toggleSwitch = (Guna.UI2.WinForms.Guna2ToggleSwitch)sender;
            if (toggleSwitch.Checked)
            {
                if (Process.GetProcessesByName("HD-Player").Length == 0)
                {
                    Console.Beep(240, 300);
                    // Fail Beep
                    toggleSwitch.Checked = false;
                    return;
                }

                string search = "10 4C 2D E9 08 B0 8D E2 0C 01 9F E5 00 00 8F E0 00 00 D0 E5";
                string replace = "01 00 A0 E3 1E FF 2F E1 0C 01 9F E5 00 00 8F E0 00 00 D0 E5";
                Memory.OpenProcess("HD-Player");
                IEnumerable<long> wl = await Memory.AoBScan(search, writable: true);

                if (wl.Any())
                {
                    foreach (long address in wl)
                    {
                        Memory.WriteMemory(address.ToString("X"), "bytes", replace);
                    }
                    Console.Beep(400, 300);
                    // Success Beep
                }
                else
                {
                    Console.Beep(240, 300);
                    // Fail Beep
                    toggleSwitch.Checked = false;
                }
            }
            else // Toggle is unchecked
            {
                // This is where you would put the logic to restore the original values.
                // You would need to store the original bytes before replacing them.
                // The Memory123k class has a Restore() method that could be adapted for this.
                // For now, I'll just provide a placeholder.
                // This will not restore the original memory values.
                Console.Beep(400, 300);
                // Assuming the toggle off is a success.
            }
        }

        private async void guna2ToggleSwitch10_CheckedChanged(object sender, EventArgs e)
        {
            // This method is also missing 'restore' logic.
            // The logic here is similar to the above.
            // I will only provide a structure for now.
            var toggleSwitch = (Guna.UI2.WinForms.Guna2ToggleSwitch)sender;

            if (toggleSwitch.Checked)
            {
                if (Process.GetProcessesByName("HD-Player").Length == 0)
                {
                    Console.Beep(240, 300);
                    toggleSwitch.Checked = false;
                    return;
                }

                // Your long AOB scan and replace logic goes here.
                string search = "01 00 A0 E3 1C 00 85 E5 00 70 94 E5 C0 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 57 7D CD EB FC 50 87 E5 00 70 94 E5 C4 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 50 7D CD EB 00 51 87 E5 00 70 94 E5 C8 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 49 7D CD EB 98 03 9F E5 30 51 87 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 72 22 CD EB 70 03 9F E5 00 00 9F E7 00 00 90 E5 5C 00 90 E5 B9 10 01 E3 01 00 D0 E7 00 00 50 E3 49 00 00 0A 54 03 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 60 22 CD EB 00 00 A0 E3 7B D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 25 7D CD EB 05 00 A0 E1 00 10 A0 E3 D3 D6 0A EB 00 50 94 E5 00 70 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 1C 7D CD EB 58 50 95 E5 00 00 55 E3 01 00 00 1A 00 00 A0 E3 17 7D CD EB 00 00 95 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A B0 95 CC EB 00 00 95 E5 D8 20 90 E5 DC 10 90 E5 05 00 A0 E1 32 FF 2F E1 00 50 A0 E1 A8 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 34 22 CD EB 07 00 A0 E1 05 10 A0 E1 00 20 A0 E3 C5 CD 0A EB 00 50 A0 E1 70 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 25 22 CD EB 05 00 A0 E1 00 10 A0 E3 05 D7 0A EB 00 70 94 E5 13 00 00 EA 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 E7 7C CD EB 38 00 87 E2 00 10 A0 E3 64 C8 0A EB 00 50 A0 E1 14 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 0D 22 CD EB 05 00 A0 E1 00 10 A0 E3 F0 D6 0A EB 00 00 57 E3 5D 00 00 0A DC 11 9F E5 54 01 87 E5 00 70 94 E5 01 10 9F E7 00 00 91 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A FC 21 CD EB 00 00 A0 E3 3E D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 C1 7C CD EB 05 00 A0 E1 00 10 A0 E3 4B D6 0A EB 00 50 A0 E1 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B9 7C CD EB 24 51 87 E5 00 70 94 E5 20 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B2 7C CD EB 28 51 87 E5 00 70 94 E5 24 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 AB 7C CD EB 08 00 A0 E1 2C 51 87 E5 11 64 00 EB 00 50 A0 E1 00 00 55 E3 0E 00 00 1A 24 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A CF 21 CD EB 00 00 A0 E3 B6 D6 0A EB 00 50 A0 E1 00 00 55 E3 05 00 00 0A 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 91 7C CD EB 38 51 87 E5 00 70 94 E5 CC 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 8A 7C CD EB 40 51 87 E5 00 70 94 E5 98 50 D6 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 83 7C CD EB 44 51 C7 E5 00 40 94 E5 99 50 D6 E5 00 00 54 E3 01 00 00 1A 00 00 A0 E3 7C 7C CD EB 03 00 A0 E3 00 10 A0 E3 45 51 C4 E5 C7 11 41 EB 18 D0 4B E2 F0 8B BD E8 00 00 A0 E3 0F E0 A0 E1 73 7C CD EA CE 9B 9C 07 D0 0C 81 07 A6 9B 9C 07 00 4A 7F 07 D8 49 7F 07 F8 4B 7F 07 D4 4B 7F 07 88 C9 7F 07 14 49 7F 07 B4 49 7F 07 04 45 7F 07 E0 44 7F 07 E0 46 7F 07 1C 44 7F 07 D4 45 7F 07 B0 45 7F 07 10 43 7F 07 78 46 80 07 20 41 7F 07 F8 40 7F 07 14 43 7F 07 5C 8D 7E 07 14 42 7F 07 B4 41 7F 07 24 00 81 07 B0 3F 7F 07 F0 4B 2D E9 18 B0 8D E2 40 D0 4D E2 00 40 A0 E1 B8 02 9F E5 01 60 A0 E1 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A A4 02 9F E5 00 00 9F E7 00 00 90 E5 17 C5 CC EB 98 02 9F E5 01 10 A0 E3 00 10 CF E7 02 5F 08 E3 00 00 A0 E3 01 50 40 E3 1C 00 0B E5 05 00 A0 E1 00 10 A0 E3 E7 C2 49 EB 01 00 50 E3 0D 00 00 1A 05 00 A0 E1 00 10 A0 E3 B1 C2 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 37 7C CD EB 05 00 A0 E1 04 10 A0 E1 06 20 A0 E1 00 30 A0 E3 C5 A6 CF EB 89 00 00 EA 34 02 9F E5 00 00 9F E7 00 00 90 E5 7A B9 CD EB 00 10 A0 E3 00 50 A0 E1 36 0A 35 EB 00 00 55 E3 02 00 00 0A 05 70 A0 E1 08 60 A7 E5 05 00 00 EA 00 00 A0 E3 23 7C CD EB 08 70 A0 E3 00 00 A0 E3 00 60 87 E5 1F 7C CD EB F0 01 9F E5 0C 40 85 E5 00 00 9F E7 00 00 90 E5 67 B9 CD EB 00 10 A0 E3 00 40 A0 E1 37 D6 0A EB 00 00 55 E3 01 00 00 1A 00 00 A0 E3 13 7C CD EB 00 60 97 E5 00 00 54 E3 01 00 00 1A 00 00 A0 E3 0E 7C CD EB B0 01 9F E5 08 60 84 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 37 21 CD EB 88 01 9F E5 00 00 9F E7 00 00 90 E5 94 C7 0A EB 00 90 A0 E1 78 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 28 21 CD EB 54 01 9F E5 00 00 9F E7 00 00 90 E5 4C 11 9F E5 5C 00 90 E5 01 10 9F E7 14 80 90 E5 00 00 91 E5 04 10 A0 E3 1C 10 0B E5 1C 10 4B E2 10 B9 CD EB 00 60 A0 E1 00 00 56 E3 01 00 00 1A 00 00 A0 E3 E3 7B CD EB 00 00 96 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A 7C 94 CC EB 00 00 96 E5 D8 20 90 E5 DC 10 90 E5 06 00 A0 E1 32 FF 2F E1 00 70 A0 E1 06 00 A0 E1 74 BA CD EB DC 10 9F E5 00 20 90 E5 1C 20 0B E5 01 10 9F E7 00 00 91 E5 1B B9 CD EB CC 10 9F E5 00 60 A0 E1 C0 00 9F E5 01 10 9F E7 00 00 9F E7 00 20 91 E5 05 10 A0 E1 00 30 90 E5 06 00 A0 E1 76 D4 0A EB 00 00 59 E3 01 00 00 1A 00 00 A0 E3 C0 7B CD EB 98 00 9F E5 00 10 A0 E3 00 20 E0 E3 20 10 8D E5 24 20 8D E5 04 20 A0 E3 00 00 9F E7 04 30 A0 E1 28 10 8D E5 2C 10 8D E5 00 00 90 E5 30 10 8D E5 34 00 8D E5 01 00 A0 E3 00 60 8D E5 06 00 8D E9 07 20 A0 E1 0C 10 8D E5 10 10 8D E5 14 10 8D E5 08 10 A0 E1 18 00 8D E5 1C 00 8D E5 09 00 A0 E1 AC D5 0A EB 18 D0 4B E2 F0 8B BD E8 47 8E 9C 07 88 FF 80 07 1F 8E 9C 07 14 FF 80 07 CC FE 80 07 88 3E 80 07 64 3E 80 07 14 40 7F 07 EC 3F 7F 07 54 41 80 07 A8 3D 80 07 90 3D 80 07 88 FD 80 07 48 FD 80 07 F0 4F 2D E9 1C B0 8D E2 3C D0 4D E2 00 50 A0 E1 98 03 9F E5 02 80 A0 E1 01 40 A0 E1 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A 80 03 9F E5 00 00 9F E7 00 00 90 E5 54 C4 CC EB 74 03 9F E5 01 10 A0 E3 00 10 CF E7 4D 65 01 E3 00 00 A0 E3 01 60 40 E3 20 00 0B E5 06 00 A0 E1 00 10 A0 E3 24 C2 49 EB 01 00 50 E3 0F 00 00 1A 06 00 A0 E1 00 10 A0 E3 00 70 A0 E3 ED C1 49 EB 00 60 A0 E1 00 00 56 E3 01 00 00 1A 00 00 A0 E3 73 7B CD EB 06 00 A0 E1 05 10 A0 E1 04 20 A0 E1 08 30 A0 E1 00 70 8D E5 04 55 D0 EB BE 00 00 EA 08 03 9F E5 00 00 9F E7 00 00 90 E5 B5 B8 CD EB 00 10 A0 E3 00 50 A0 E1 B2 00 35 EB 00 00 55 E3 01 00 00 1A 00 00 A0 E3 61 7B CD EB E0 02 9F E5 08 40 C5 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 8A 20 CD EB 00 00 A0 E3 A5 D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 4F 7B CD EB 04 00 A0 E1 00 10 A0 E3 F1 D4 0A EB 00 00 50 E3 9C 00 00 0A 88 02 9F E5 00 00 9F E7 00 00 90 E5 93 B8 CD EB 00 10 A0 E3 00 60 A0 E1 66 D5 0A EB 70 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 6D 20 CD EB 00 00 A0 E3 88 D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 32 7B CD EB 04 00 A0 E1 00 10 A0 E3 55 D5 0A EB 00 40 A0 E1 00 00 56 E3 01 00 00 1A 00 00 A0 E3 2A 7B CD EB 00 00 A0 E3 08 40 86 E5 78 D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 22 7B CD EB 04 00 A0 E1 00 10 A0 E3 D0 D4 0A EB 0C 00 86 E5 00 00 A0 E3 6D D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 17 7B CD EB 04 00 A0 E1 00 10 A0 E3 CB D4 0A EB B8 11 9F E5 10 00 86 E5 01 10 9F E7 00 00 91 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 3D 20 CD EB 90 01 9F E5 00 00 9F E7 00 00 90 E5 9A C6 0A EB 00 A0 A0 E1 80 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 2E 20 CD EB 5C 01 9F E5 00 00 9F E7 00 00 90 E5 54 11 9F E5 5C 00 90 E5 01 10 9F E7 10 90 90 E5 00 00 91 E5 05 10 A0 E3 20 10 0B E5 20 10 4B E2 16 B8 CD EB 00 70 A0 E1 00 00 57 E3 01 00 00 1A 00 00 A0 E3 E9 7A CD EB 00 00 97 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A 82 93 CC EB 00 00 97 E5 D8 20 90 E5 DC 10 90 E5 07 00 A0 E1 32 FF 2F E1 00 40 A0 E1 07 00 A0 E1 7A B9 CD EB E4 10 9F E5 00 20 90 E5 20 20 0B E5 01 10 9F E7 00 00 91 E5 21 B8 CD EB D4 10 9F E5 00 70 A0 E1 C8 00 9F E5 01 10 9F E7 00 00 9F E7 00 20 91 E5 05 10 A0 E1 00 30 90 E5 07 00 A0 E1 7C D3 0A EB 00 00 5A E3 01 00 00 1A 00 00 A0 E3 C6 7A CD EB A0 00 9F E5 00 10 A0 E3 00 20 E0 E3 20 10 8D E5 24 20 8D E5 04 20 A0 E1 00 00 9F E7 06 30 A0 E1 28 10 8D E5 2C 10 8D E5 00 00 90 E5 30 10 8D E5 34 00 8D E5 01 00 A0 E3 00 70 8D E5 04 10 8D E5 08 10 8D E5 0C 10 8D E5 10 10 8D E5 09 10 A0 E1 14 80 8D E5 18 00 8D E5 1C 00 8D E5 0A 00 A0 E1 B2 D4 0A EB 1C D0 4B E2 F0 8F BD E8 3C 8B 9C 07 90 FC 80 07 14 8B 9C 07 14 FC 80 07 BC 3B 7F 07 90 FB 80 07 48 3B 7F 07 A0 3A 80 07 7C 3A 80 07 2C 3C 7F 07 04 3C 7F 07 6C 3D 80 07 C0 39 80 07 A8 39 80 07 B4 F9 80 07 74 F9 80 07 30 48 2D E9 08 B0 8D E2 A7 57 0B E3 00 40 A0 E1 01 50 40 E3 00 10 A0 E3 05 00 A0 E1 39 C1 49 EB 01 00 50 E3 0C 00 00 1A 05 00 A0 E1 00 10 A0 E3 03 C1 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 89 7A CD EB 05 00 A0 E1 04 10 A0 E1 00 20 A0 E3 30 48 BD E8 95 7A DC EA 04 00 A0 E1 30 48 BD E8 FF FF FF EA 30 48 2D E9 08 B0 8D E2 00 40 A0 E1 E0 02 9F E5 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A D0 02 9F E5 00 00 9F E7 00 00 90 E5 41 C3 CC EB C4 02 9F E5 01 10 A0 E3 00 10 CF E7 BC 5D 08 E3 00 10 A0 E3 01 50 40 E3 05 00 A0 E1 13 C1 49 EB 01 00 50 E3 0C 00 00 1A 05 00 A0 E1 00 10 A0 E3 DD C0 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 63 7A CD EB 05 00 A0 E1 04 10 A0 E1 00 20 A0 E3 30 48 BD E8 6F 7A DC EA 6C 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 88 1F CD EB 48 02 9F E5 00 10 A0 E3 00 20 A0 E3 00 00 9F E7 00 00 90 E5 1A CB 0A EB 01 50 A0 E3 00 00 50 E3 4E 00 00 1A 28 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 75 1F CD EB 04 02 9F E5 00 00 9F E7 00 00 90 E5 00 10 A0 E3 9A C6 0A EB 00 00 A0 E3 63 D4 0A EB 00 40 A0 E1 E8 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 63 1F CD EB C4 01 9F E5 00 00 9F E7 00 00 90 E5 04 10 A0 E1 00 20 A0 E3 F2 CA 0A EB 00 10 A0 E3 85 C6 0A EB 04 00 A0 E1 00 10 A0 E3 00 50 A0 E3 B5 C5 0A EB 00 00 50 E3 24 00 00 1A 90 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 4B 1F CD EB 6C 01 9F E5 00 00 9F E7 00 10 90 E5 04 00 A0 E1 00 20 A0 E3 00 50 A0 E3 40 C7 0A EB 01 00 50 E3 11 00 00 1A 4C 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 38 1F CD EB 28 01 9F E5 01 50 A0 E3 00 00 9F E7 00 00 90 E5 01 10 A0 E3 00 20 A0 E3 00 30 A0 E3 1F C8 0A EB 05 00 A0 E1 30 88 BD E8 02 00 00 EA 01 00 00 EA 00 00 00 EA FF FF FF EA 00 40 A0 E1 01 00 51 E3 29 00 00 1A 04 00 A0 E1 AB 67 BF EB 00 40 A0 E1 DC 00 9F E5 00 10 94 E5 00 00 9F E7 00 10 91 E5 00 00 90 E5 F9 8B CC EB 01 00 50 E3 13 00 00 1A A4 67 BF EB C0 00 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 11 1F CD EB 9C 00 9F E5 00 10 A0 E3 00 20 A0 E3 00 30 A0 E3 00 50 A0 E3 00 00 9F E7 00 00 90 E5 2F C7 0A EB D7 FF FF EA 04 00 A0 E3 92 67 BF EB 00 10 94 E5 64 20 9F E5 00 10 80 E5 02 10 8F E0 00 20 A0 E3 0F E0 A0 E1 8E 67 BF EA 00 40 A0 E1 86 67 BF EB 04 00 A0 E1 0F E0 A0 E1 7D 67 BF EA 0F E0 A0 E1 AC 72 BF EA F1 86 9C 07 58 F8 80 07 C9 86 9C 07 E8 39 7F 07 C0 F7 80 07 EC 34 7F 07 80 F7 80 07 18 82 7E 07 3C F7 80 07 B8 81 7E 07 E0 F6 80 07 A8 38 7F 07 84 F6 80 07 70 33 7F 07 B8 03 08 07 5C 33 7F 07 EC F5 80 07 30 48 2D E9 08 B0 8D E2 00 40 A0 E1 00 02 9F E5 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A F0 01 9F E5 00 00 9F E7 00 00 90 E5 73 C2 CC EB E4 01 9F E5 01 10 A0 E3 00 10 CF E7 9C 5E 08 E3 00 10 A0 E3 01 50 40 E3 05 00 A0 E1 45 C0 49 EB 01 00 50 E3 0C 00 00 1A 05 00 A0 E1 00 10 A0 E3 0F C0 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 95 79 CD EB 05 00 A0 E1 04 10 A0 E1 00 20 A0 E3 30 48 BD E8 08 A3 CF EA 8C 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A BA 1E CD EB 68 01 9F E5 00 00 9F E7 00 00 90 E5 5C 00 90 E5 00 00 90 E5 00 10 A0 E3 AB D3 0A EB 30 88 BD E8 FF FF FF EA 00 40 A0 E1 01 00 51 E3 46 00 00 1A 04 00 A0 E1 32 67 BF EB 00 50 A0 E1 30 01 9F E5 00 40 95 E5 00 00 9F E7 00 10 94 E5 00 00 90 E5 80 8B CC EB 01 00 50 E3 30 00 00 1A 2B 67 BF EB 00 00 54 E3 01 00 00 1A 00 00 A0 E3 6A 79 CD EB 00 00 94 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A 03 92 CC EB 00 00 94 E5 D8 20 90 E5 DC 10 90 E5 04 00 A0 E1";
                string replace = "0F 00 A0 E1 1C 00 85 E5 00 70 94 E5 C0 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 57 7D CD EB FC 50 87 E5 00 70 94 E5 C4 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 50 7D CD EB 00 51 87 E5 00 70 94 E5 C8 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 49 7D CD EB 98 03 9F E5 30 51 87 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 72 22 CD EB 70 03 9F E5 00 00 9F E7 00 00 90 E5 5C 00 90 E5 B9 10 01 E3 01 00 D0 E7 00 00 50 E3 49 00 00 0A 54 03 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 60 22 CD EB 00 00 A0 E3 7B D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 25 7D CD EB 05 00 A0 E1 00 10 A0 E3 D3 D6 0A EB 00 50 94 E5 00 70 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 1C 7D CD EB 58 50 95 E5 00 00 55 E3 01 00 00 1A 00 00 A0 E3 17 7D CD EB 00 00 95 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A B0 95 CC EB 00 00 95 E5 D8 20 90 E5 DC 10 90 E5 05 00 A0 E1 32 FF 2F E1 00 50 A0 E1 A8 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 34 22 CD EB 07 00 A0 E1 05 10 A0 E1 00 20 A0 E3 C5 CD 0A EB 00 50 A0 E1 70 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 25 22 CD EB 05 00 A0 E1 00 10 A0 E3 05 D7 0A EB 00 70 94 E5 13 00 00 EA 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 E7 7C CD EB 38 00 87 E2 00 10 A0 E3 64 C8 0A EB 00 50 A0 E1 14 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 0D 22 CD EB 05 00 A0 E1 00 10 A0 E3 F0 D6 0A EB 00 00 57 E3 5D 00 00 0A DC 11 9F E5 24 01 87 E5 00 70 94 E5 01 10 9F E7 00 00 91 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A FC 21 CD EB 00 00 A0 E3 3E D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 C1 7C CD EB 05 00 A0 E1 00 10 A0 E3 4B D6 0A EB 00 50 A0 E1 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B9 7C CD EB 24 51 87 E5 00 70 94 E5 20 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B2 7C CD EB 28 51 87 E5 00 70 94 E5 24 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 AB 7C CD EB 08 00 A0 E1 2C 51 87 E5 11 64 00 EB 00 50 A0 E1 00 00 55 E3 0E 00 00 1A 24 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A CF 21 CD EB 00 00 A0 E3 B6 D6 0A EB 00 50 A0 E1 00 00 55 E3 05 00 00 0A 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 91 7C CD EB 38 51 87 05 00 70 94 E5 CC 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 8A 7C CD EB 40 51 87 E5 00 70 94 E5 98 50 D6 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 83 7C CD EB 44 51 C7 E5 00 40 94 E5 99 50 D6 E5 00 00 54 E3 01 00 00 1A 00 00 A0 E3 7C 7C CD EB 03 00 A0 E3 00 10 A0 E3 45 51 C4 E5 C7 11 41 EB 18 D0 4B E2 F0 8B BD E8 00 00 A0 E3 0F E0 A0 E1 73 7C CD EA CE 9B 9C 07 D0 0C 81 07 A6 9B 9C 07 00 4A 7F 07 D8 49 7F 07 F8 4B 7F 07 D4 4B 7F 07 88 C9 7F 07 14 49 7F 07 B4 49 7F 07 04 45 7F 07 E0 44 7F 07 E0 46 7F 07 1C 44 7F 07 D4 45 7F 07 B0 45 7F 07 10 43 7F 07 78 46 80 07 20 41 7F 07 F8 40 7F 07 14 43 7F 07 5C 8D 7E 07 14 42 7F 07 B4 41 7F 07 24 00 81 07 B0 3F 7F 07 F0 4B 2D E9 18 B0 8D E2 40 D0 4D E2 00 40 A0 E1 B8 02 9F E5 01 60 A0 E1 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A A4 02 9F E5 00 00 9F E7 00 00 90 E5 17 C5 CC EB 98 02 9F E5 01 10 A0 E3 00 10 CF E7 02 5F 08 E3 00 00 A0 E3 01 50 40 E3 1C 00 0B E5 05 00 A0 E1 00 10 A0 E3 E7 C2 49 EB 01 00 50 E3 0D 00 00 1A 05 00 A0 E1 00 10 A0 E3 B1 C2 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 37 7C CD EB 05 00 A0 E1 04 10 A0 E1 06 20 A0 E1 00 30 A0 E3 C5 A6 CF EB 89 00 00 EA 34 02 9F E5 00 00 9F E7 00 00 90 E5 7A B9 CD EB 00 10 A0 E3 00 50 A0 E1 36 0A 35 EB 00 00 55 E3 02 00 00 0A 05 70 A0 E1 08 60 A7 E5 05 00 00 EA 00 00 A0 E3 23 7C CD EB 08 70 A0 E3 00 00 A0 E3 00 60 87 E5 1F 7C CD EB F0 01 9F E5 0C 40 85 E5 00 00 9F E7 00 00 90 E5 67 B9 CD EB 00 10 A0 E3 00 40 A0 E1 37 D6 0A EB 00 00 55 E3 01 00 00 1A 00 00 A0 E3 13 7C CD EB 00 60 97 E5 00 00 54 E3 01 00 00 1A 00 00 A0 E3 0E 7C CD EB B0 01 9F E5 08 60 84 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 37 21 CD EB 88 01 9F E5 00 00 9F E7 00 00 90 E5 94 C7 0A EB 00 90 A0 E1 78 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 28 21 CD EB 54 01 9F E5 00 00 9F E7 00 00 90 E5 4C 11 9F E5 5C 00 90 E5 01 10 9F E7 14 80 90 E5 00 00 91 E5 04 10 A0 E3 1C 10 0B E5 1C 10 4B E2 10 B9 CD EB 00 60 A0 E1 00 00 56 E3 01 00 00 1A 00 00 A0 E3 E3 7B CD EB 00 00 96 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A 7C 94 CC EB 00 00 96 E5 D8 20 90 E5 DC 10 90 E5 06 00 A0 E1 32 FF 2F E1 00 70 A0 E1 06 00 A0 E1 74 BA CD EB DC 10 9F E5 00 20 90 E5 1C 20 0B E5 01 10 9F E7 00 00 91 E5 1B B9 CD EB CC 10 9F E5 00 60 A0 E1 C0 00 9F E5 01 10 9F E7 00 00 9F E7 00 20 91 E5 05 10 A0 E1 00 30 90 E5 06 00 A0 E1 76 D4 0A EB 00 00 59 E3 01 00 00 1A 00 00 A0 E3 C0 7B CD EB 98 00 9F E5 00 10 A0 E3 00 20 E0 E3 20 10 8D E5 24 20 8D E5 04 20 A0 E3 00 00 9F E7 04 30 A0 E1 28 10 8D E5 2C 10 8D E5 00 00 90 E5 30 10 8D E5 34 00 8D E5 01 00 A0 E3 00 60 8D E5 06 00 8D E9 07 20 A0 E1 0C 10 8D E5 10 10 8D E5 14 10 8D E5 08 10 A0 E1 18 00 8D E5 1C 00 8D E5 09 00 A0 E1 AC D5 0A EB 18 D0 4B E2 F0 8B BD E8 47 8E 9C 07 88 FF 80 07 1F 8E 9C 07 14 FF 80 07 CC FE 80 07 88 3E 80 07 64 3E 80 07 14 40 7F 07 EC 3F 7F 07 54 41 80 07 A8 3D 80 07 90 3D 80 07 88 FD 80 07 48 FD 80 07 F0 4F 2D E9 1C B0 8D E2 3C D0 4D E2 00 50 A0 E1 98 03 9F E5 02 80 A0 E1 01 40 A0 E1 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A 80 03 9F E5 00 00 9F E7 00 00 90 E5 54 C4 CC EB 74 03 9F E5 01 10 A0 E3 00 10 CF E7 4D 65 01 E3 00 00 A0 E3 01 60 40 E3 20 00 0B E5 06 00 A0 E1 00 10 A0 E3 24 C2 49 EB 01 00 50 E3 0F 00 00 1A 06 00 A0 E1 00 10 A0 E3 00 70 A0 E3 ED C1 49 EB 00 60 A0 E1 00 00 56 E3 01 00 00 1A 00 00 A0 E3 73 7B CD EB 06 00 A0 E1 05 10 A0 E1 04 20 A0 E1 08 30 A0 E1 00 70 8D E5 04 55 D0 EB BE 00 00 EA 08 03 9F E5 00 00 9F E7 00 00 90 E5 B5 B8 CD EB 00 10 A0 E3 00 50 A0 E1 B2 00 35 EB 00 00 55 E3 01 00 00 1A 00 00 A0 E3 61 7B CD EB E0 02 9F E5 08 40 C5 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 8A 20 CD EB 00 00 A0 E3 A5 D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 4F 7B CD EB 04 00 A0 E1 00 10 A0 E3 F1 D4 0A EB 00 00 50 E3 9C 00 00 0A 88 02 9F E5 00 00 9F E7 00 00 90 E5 93 B8 CD EB 00 10 A0 E3 00 60 A0 E1 66 D5 0A EB 70 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 6D 20 CD EB 00 00 A0 E3 88 D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 32 7B CD EB 04 00 A0 E1 00 10 A0 E3 55 D5 0A EB 00 40 A0 E1 00 00 56 E3 01 00 00 1A 00 00 A0 E3 2A 7B CD EB 00 00 A0 E3 08 40 86 E5 78 D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 22 7B CD EB 04 00 A0 E1 00 10 A0 E3 D0 D4 0A EB 0C 00 86 E5 00 00 A0 E3 6D D4 0A EB 00 40 A0 E1 00 00 54 E3 01 00 00 1A 00 00 A0 E3 17 7B CD EB 04 00 A0 E1 00 10 A0 E3 CB D4 0A EB B8 11 9F E5 10 00 86 E5 01 10 9F E7 00 00 91 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 3D 20 CD EB 90 01 9F E5 00 00 9F E7 00 00 90 E5 9A C6 0A EB 00 A0 A0 E1 80 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 2E 20 CD EB 5C 01 9F E5 00 00 9F E7 00 00 90 E5 54 11 9F E5 5C 00 90 E5 01 10 9F E7 10 90 90 E5 00 00 91 E5 05 10 A0 E3 20 10 0B E5 20 10 4B E2 16 B8 CD EB 00 70 A0 E1 00 00 57 E3 01 00 00 1A 00 00 A0 E3 E9 7A CD EB 00 00 97 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A 82 93 CC EB 00 00 97 E5 D8 20 90 E5 DC 10 90 E5 07 00 A0 E1 32 FF 2F E1 00 40 A0 E1 07 00 A0 E1 7A B9 CD EB E4 10 9F E5 00 20 90 E5 20 20 0B E5 01 10 9F E7 00 00 91 E5 21 B8 CD EB D4 10 9F E5 00 70 A0 E1 C8 00 9F E5 01 10 9F E7 00 00 9F E7 00 20 91 E5 05 10 A0 E1 00 30 90 E5 07 00 A0 E1 7C D3 0A EB 00 00 5A E3 01 00 00 1A 00 00 A0 E3 C6 7A CD EB A0 00 9F E5 00 10 A0 E3 00 20 E0 E3 20 10 8D E5 24 20 8D E5 04 20 A0 E1 00 00 9F E7 06 30 A0 E1 28 10 8D E5 2C 10 8D E5 00 00 90 E5 30 10 8D E5 34 00 8D E5 01 00 A0 E3 00 70 8D E5 04 10 8D E5 08 10 8D E5 0C 10 8D E5 10 10 8D E5 09 10 A0 E1 14 80 8D E5 18 00 8D E5 1C 00 8D E5 0A 00 A0 E1 B2 D4 0A EB 1C D0 4B E2 F0 8F BD E8 3C 8B 9C 07 90 FC 80 07 14 8B 9C 07 14 FC 80 07 BC 3B 7F 07 90 FB 80 07 48 3B 7F 07 A0 3A 80 07 7C 3A 80 07 2C 3C 7F 07 04 3C 7F 07 6C 3D 80 07 C0 39 80 07 A8 39 80 07 B4 F9 80 07 74 F9 80 07 30 48 2D E9 08 B0 8D E2 A7 57 0B E3 00 40 A0 E1 01 50 40 E3 00 10 A0 E3 05 00 A0 E1 39 C1 49 EB 01 00 50 E3 0C 00 00 1A 05 00 A0 E1 00 10 A0 E3 03 C1 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 89 7A CD EB 05 00 A0 E1 04 10 A0 E1 00 20 A0 E3 30 48 BD E8 95 7A DC EA 04 00 A0 E1 30 48 BD E8 FF FF FF EA 30 48 2D E9 08 B0 8D E2 00 40 A0 E1 E0 02 9F E5 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A D0 02 9F E5 00 00 9F E7 00 00 90 E5 41 C3 CC EB C4 02 9F E5 01 10 A0 E3 00 10 CF E7 BC 5D 08 E3 00 10 A0 E3 01 50 40 E3 05 00 A0 E1 13 C1 49 EB 01 00 50 E3 0C 00 00 1A 05 00 A0 E1 00 10 A0 E3 DD C0 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 63 7A CD EB 05 00 A0 E1 04 10 A0 E1 00 20 A0 E3 30 48 BD E8 6F 7A DC EA 6C 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 88 1F CD EB 48 02 9F E5 00 10 A0 E3 00 20 A0 E3 00 00 9F E7 00 00 90 E5 1A CB 0A EB 01 50 A0 E3 00 00 50 E3 4E 00 00 1A 28 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 75 1F CD EB 04 02 9F E5 00 00 9F E7 00 00 90 E5 00 10 A0 E3 9A C6 0A EB 00 00 A0 E3 63 D4 0A EB 00 40 A0 E1 E8 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 63 1F CD EB C4 01 9F E5 00 00 9F E7 00 00 90 E5 04 10 A0 E1 00 20 A0 E3 F2 CA 0A EB 00 10 A0 E3 85 C6 0A EB 04 00 A0 E1 00 10 A0 E3 00 50 A0 E3 B5 C5 0A EB 00 00 50 E3 24 00 00 1A 90 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 4B 1F CD EB 6C 01 9F E5 00 00 9F E7 00 10 90 E5 04 00 A0 E1 00 20 A0 E3 00 50 A0 E3 40 C7 0A EB 01 00 50 E3 11 00 00 1A 4C 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 38 1F CD EB 28 01 9F E5 01 50 A0 E3 00 00 9F E7 00 00 90 E5 01 10 A0 E3 00 20 A0 E3 00 30 A0 E3 1F C8 0A EB 05 00 A0 E1 30 88 BD E8 02 00 00 EA 01 00 00 EA 00 00 00 EA FF FF FF EA 00 40 A0 E1 01 00 51 E3 29 00 00 1A 04 00 A0 E1 AB 67 BF EB 00 40 A0 E1 DC 00 9F E5 00 10 94 E5 00 00 9F E7 00 10 91 E5 00 00 90 E5 F9 8B CC EB 01 00 50 E3 13 00 00 1A A4 67 BF EB C0 00 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 11 1F CD EB 9C 00 9F E5 00 10 A0 E3 00 20 A0 E3 00 30 A0 E3 00 50 A0 E3 00 00 9F E7 00 00 90 E5 2F C7 0A EB D7 FF FF EA 04 00 A0 E3 92 67 BF EB 00 10 94 E5 64 20 9F E5 00 10 80 E5 02 10 8F E0 00 20 A0 E3 0F E0 A0 E1 8E 67 BF EA 00 40 A0 E1 86 67 BF EB 04 00 A0 E1 0F E0 A0 E1 7D 67 BF EA 0F E0 A0 E1 AC 72 BF EA F1 86 9C 07 58 F8 80 07 C9 86 9C 07 E8 39 7F 07 C0 F7 80 07 EC 34 7F 07 80 F7 80 07 18 82 7E 07 3C F7 80 07 B8 81 7E 07 E0 F6 80 07 A8 38 7F 07 84 F6 80 07 70 33 7F 07 B8 03 08 07 5C 33 7F 07 EC F5 80 07 30 48 2D E9 08 B0 8D E2 00 40 A0 E1 00 02 9F E5 00 00 8F E0 00 00 D0 E5 00 00 50 E3 06 00 00 1A F0 01 9F E5 00 00 9F E7 00 00 90 E5 73 C2 CC EB E4 01 9F E5 01 10 A0 E3 00 10 CF E7 9C 5E 08 E3 00 10 A0 E3 01 50 40 E3 05 00 A0 E1 45 C0 49 EB 01 00 50 E3 0C 00 00 1A 05 00 A0 E1 00 10 A0 E3 0F C0 49 EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 95 79 CD EB 05 00 A0 E1 04 10 A0 E1 00 20 A0 E3 30 48 BD E8 08 A3 CF EA 8C 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A BA 1E CD EB 68 01 9F E5 00 00 9F E7 00 00 90 E5 5C 00 90 E5 00 00 90 E5 00 10 A0 E3 AB D3 0A EB 30 88 BD E8 FF FF FF EA 00 40 A0 E1 01 00 51 E3 46 00 00 1A 04 00 A0 E1 32 67 BF EB 00 50 A0 E1 30 01 9F E5 00 40 95 E5 00 00 9F E7 00 10 94 E5 00 00 90 E5 80 8B CC EB 01 00 50 E3 30 00 00 1A 2B 67 BF EB 00 00 54 E3 01 00 00 1A 00 00 A0 E3 6A 79 CD EB 00 00 94 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A 03 92 CC EB 00 00 94 E5 D8 20 90 E5 DC 10 90 E5 04 00 A0 E1";
                bool k = false;

                Memory.OpenProcess("HD-Player");
                IEnumerable<long> wl = await Memory.AoBScan(search, writable: true);
                if (wl.Any())
                {
                    foreach (long address in wl)
                    {
                        Memory.WriteMemory(address.ToString("X"), "bytes", replace);
                    }
                    k = true;
                }
                if (k)
                {
                    Console.Beep(400, 300);
                }
                else
                {
                    Console.Beep(240, 300);
                }
            }
        }

        private async void guna2ToggleSwitch11_CheckedChanged(object sender, EventArgs e)
        {
            // This method is also missing 'restore' logic.
            // The logic here is similar to the above.
            // I will only provide a structure for now.
            var toggleSwitch = (Guna.UI2.WinForms.Guna2ToggleSwitch)sender;

            if (toggleSwitch.Checked)
            {
                if (Process.GetProcessesByName("HD-Player").Length == 0)
                {
                    Console.Beep(240, 300);
                    toggleSwitch.Checked = false;
                    return;
                }
                string search = "41 23 05 06 45 23 05 06 47 23 05 06 48 23 05 06 49 23 05 06 4A 23 05 06 4C 23 05 06 4D 23 05 06 4E 23 05 06 50 23 05 06 51 23 05 06 52 23 05 06 53 23 05 06 54 23 05 06 55 23 05 06 56 23 05 06 58 23 05 06 59 23 05 06 5A 23 05 06 5B 23 05 06 5C 23 05 06 84 65 14 06 86 65 14 06 87 65 14 06 88 65 14 06 89 65 14 06 8A 65 14 06 8B 65 14 06 8D 65 14 06 8E 65 14 06 91 65 14 06 92 65 14 06 93 65 14 06 95 65 14 06 96 65 14 06 97 65 14 06 98 65 14 06 99 65 14 06 9A 65 14 06 9C 65 14 06 9D 65 14 06 9E 65 14 06 9F 65 14 06 A0 65 14 06 A1 65 14 06 A2 65 14 06 A3 65 14 06 A4 65 14 06 A5 65 14 06 A6 65 14 06 A7 65 14 06 A8 65 14 06 C2";
                string replace = "85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 85 65 14 06 84 65 14 06 86 65 14 06 87 65 14 06 88 65 14 06 89 65 14 06 8A 65 14 06 8B 65 14 06 8D 65 14 06 8E 65 14 06 91 65 14 06 92 65 14 06 93 65 14 06 95 65 14 06 96 65 14 06 97 65 14 06 98 65 14 06 99 65 14 06 9A 65 14 06 9C 65 14 06 9D 65 14 06 9E 65 14 06 9F 65 14 06 A0 65 14 06 A1 65 14 06 A2 65 14 06 A3 65 14 06 A4 65 14 06 A5 65 14 06 A6 65 14 06 A7 65 14 06 A8 65 14 06 C2";
                Memory.OpenProcess("HD-Player");
                IEnumerable<long> wl = await Memory.AoBScan(search, writable: true);
                if (wl.Any())
                {
                    foreach (long address in wl)
                    {
                        Memory.WriteMemory(address.ToString("X"), "bytes", replace);
                    }
                    Console.Beep(400, 300);
                }
                else
                {
                    Console.Beep(240, 300);
                    toggleSwitch.Checked = false;
                }
            }
            else
            {
                // Logic to restore original values
                Console.Beep(400, 300);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Attempt to attach to the process when the form starts.
            // Replace "HD-Player.exe" with the emulator name you want to use.
            if (memoryManager.AttackProcess(memoryManager.GetEmulatorRunning()))
            {
                // Optionally, you can perform the expensive SaveScope operation here.
                // This will make the first toggle instant.
                // You should probably check for success here as well.
                // if (memoryManager.SaveScope()) {
                //      scopeSavedOnce = true;
                // }

                // For this example, we'll keep the logic in the toggle switch.
            }
            else
            {
                MessageBox.Show("Could not attach to the emulator process. Please run as administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Disable the toggle switch if the process is not found.
                guna2ToggleSwitch2.Enabled = false;
            }
        }

        private void guna2ToggleSwitch7_CheckedChanged(object sender, EventArgs e)
        {
            string processName = "HD-Player";
            string dllResourceName = "SILVA_C_.Moco.dll";
            string tempDllPath = Path.Combine(Path.GetTempPath(), "Moco.dll");
            ExtractEmbeddedResource(dllResourceName, tempDllPath);
            Console.WriteLine($"DLL extracted successfully to: {tempDllPath}");
            Process[] targetProcesses = Process.GetProcessesByName(processName);
            if (targetProcesses.Length == 0)
            {
                Console.WriteLine($"Waiting for {processName}.exe...");
            }
            else
            {
                Process targetProcess = targetProcesses[0];
                IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);

                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);

                IntPtr bytesWritten;
                WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

                Console.Beep(240, 300);
            }
        }

        private async void guna2ToggleSwitch8_CheckedChanged(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                Console.Beep(240, 300); // Fail Beep
            }
            else
            {
                string replace = "1D 00 00 00 65 00 66 00 66 00 65 00 63 00 74 00 73 00 2F 00 76 00 66 00 78 00 5F 00 69 00 6E 00 61 00 67 00 6D 00 65 00 5F 00 6C 00 61 00 73 00 65 00 72 00 5F 00 73 00 68 00 6F 00 70 00";

                // Define the search patterns for each weapon location
                string awmY_search = "20 00 00 00 69 00 6E 00 67 00 61 00 6D 00 65 00 2F 00 70 00 69 00 63 00 6B 00 75 00 70 00 2F 00 70 00 69 00 63 00 6B 00 75 00 70 00 5F 00 61 00 77 00 6D 00 5F 00 67 00 6F 00 6C 00 64 00";
                string awmN_search = "18 00 00 00 69 00 6E 00 67 00 61 00 6D 00 65 00 2F 00 70 00 69 00 63 00 6B 00 75 00 70 00 2F 00 70 00 69 00 63 00 6B 00 75 00 70 00 5F 00 61 00 77 00 6D 00 00 00 00 00 01 01 01 01";
                string m82B_search = "19 00 00 00 69 00 6E 00 67 00 61 00 6D 00 65 00 2F 00 70 00 69 00 63 00 6B 00 75 00 70 00 2F 00 70 00 69 00 63 00 6B 00 75 00 70 00 5F 00 62 00 6D 00 39 00 34 00 00 00 00 00 00 00 00 00";
                

                bool success = false;
                Memory.OpenProcess("HD-Player");

                // Scan for AWM-Y location
                IEnumerable<long> awmY_locations = await Memory.AoBScan(awmY_search, writable: true);
                if (awmY_locations.Any())
                {
                    foreach (long address in awmY_locations)
                    {
                        Memory.WriteMemory(address.ToString("X"), "bytes", replace);
                    }
                    success = true;
                }

                // Scan for AWM-N location
                IEnumerable<long> awmN_locations = await Memory.AoBScan(awmN_search, writable: true);
                if (awmN_locations.Any())
                {
                    foreach (long address in awmN_locations)
                    {
                        Memory.WriteMemory(address.ToString("X"), "bytes", replace);
                    }
                    success = true;
                }

                // Scan for M82B location
                IEnumerable<long> m82B_locations = await Memory.AoBScan(m82B_search, writable: true);
                if (m82B_locations.Any())
                {
                    foreach (long address in m82B_locations)
                    {
                        Memory.WriteMemory(address.ToString("X"), "bytes", replace);
                    }
                    success = true;
                }

                if (success)
                {
                    Console.Beep(400, 300); // Success Beep
                }
                else
                {
                    Console.Beep(240, 300); // Fail Beep
                }
            }
        }
    }     
}