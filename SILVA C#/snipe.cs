using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BLUE_C_
{
    public class Memory123k
    {
        // P/Invoke definitions for Windows API functions
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        // P/Invoke for mouse and keyboard input
        public class User32
        {
            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(int vKey);
        }

        // Enums and Structs for P/Invoke
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x00000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        // Virtual-key codes
        public enum VirtualKey
        {
            VK_LBUTTON = 0x01, // Left mouse button
            VK_RBUTTON = 0x02, // Right mouse button
        }

        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_GUARD = 0x100;
        private const uint PAGE_NOACCESS = 0x01;
        private const uint PAGE_NOCACHE = 0x200;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint PAGE_EXECUTE_READ = 0x20;

        // Class members
        public int ProcessId { get; private set; }
        public IntPtr ProcessHandle { get; private set; } = IntPtr.Zero;

        private readonly Dictionary<long, int> originalValuesWrite = new Dictionary<long, int>();
        private readonly Dictionary<long, int> originalValuesWrite2 = new Dictionary<long, int>();
        private readonly Dictionary<long, int> modifiedValuesWrite = new Dictionary<long, int>();
        private readonly Dictionary<long, int> modifiedValuesWrite2 = new Dictionary<long, int>();
        private readonly Dictionary<long, int> modifiedAoBs = new Dictionary<long, int>();

        public struct MemoryRegion
        {
            public long BaseAddress;
            public long MemorySize;
        }

        public List<EntityWallHackHere> Oldspeed { get; private set; } = new List<EntityWallHackHere>();
        public List<long> Newspeed { get; private set; } = new List<long>();

        public struct EntityWallHackHere
        {
            public long addressWallHack;
            public byte[] patternWallHack;
        }

        private NotificationSystem notificationSystem = new NotificationSystem();

        public class NotificationSystem
        {
            public void Notification1(string title, string message, string color)
            {
                Console.WriteLine($"Notification: {title} {message} ({color})");
            }
        }

        // Methods
        public int GetPid(string procname)
        {
            if (string.IsNullOrEmpty(procname))
                return 0;

            int pid = 0;
            int threadCount = 0;

            foreach (Process process in Process.GetProcessesByName(procname.Replace(".exe", "")))
            {
                try
                {
                    if (process.Threads.Count > threadCount)
                    {
                        threadCount = process.Threads.Count;
                        pid = process.Id;
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Access denied
                }
            }
            return pid;
        }

        public string GetEmulatorRunning()
        {
            if (GetPid("HD-Player.exe") != 0) return "HD-Player.exe";
            if (GetPid("LdVBoxHeadless.exe") != 0) return "LdVBoxHeadless.exe";
            if (GetPid("MEmuHeadless.exe") != 0) return "MEmuHeadless.exe";
            if (GetPid("AndroidProcess.exe") != 0) return "AndroidProcess.exe";
            if (GetPid("aow_exe.exe") != 0) return "aow_exe.exe";
            if (GetPid("NoxVMHandle.exe") != 0) return "NoxVMHandle.exe";

            return null;
        }

        public bool AttackProcess(string procname)
        {
            int pid = GetPid(procname);
            if (pid == 0)
                return false;

            ProcessId = pid;
            ProcessHandle = OpenProcess((uint)ProcessAccessFlags.All, false, ProcessId);
            return ProcessHandle != IntPtr.Zero;
        }

        // --- CORRECTED HANDLE MANAGEMENT ---

        // This method will close the handle to the process
        public void CloseProcessHandle()
        {
            if (ProcessHandle != IntPtr.Zero)
            {
                CloseHandle(ProcessHandle);
                ProcessHandle = IntPtr.Zero; // Set to IntPtr.Zero to prevent using a stale handle
            }
        }

        // These methods no longer call CloseHandle themselves
        public void Restore()
        {
            if (ProcessHandle == IntPtr.Zero) return;

            foreach (var entry in originalValuesWrite)
            {
                byte[] valueBytes = BitConverter.GetBytes(entry.Value);
                WriteProcessMemory(ProcessHandle, new IntPtr(entry.Key), valueBytes, valueBytes.Length, out _);
            }
            foreach (var entry in originalValuesWrite2)
            {
                byte[] valueBytes = BitConverter.GetBytes(entry.Value);
                WriteProcessMemory(ProcessHandle, new IntPtr(entry.Key), valueBytes, valueBytes.Length, out _);
            }
        }

        public void Reapply()
        {
            if (ProcessHandle == IntPtr.Zero) return;

            foreach (var entry in modifiedValuesWrite)
            {
                byte[] valueBytes = BitConverter.GetBytes(entry.Value);
                WriteProcessMemory(ProcessHandle, new IntPtr(entry.Key), valueBytes, valueBytes.Length, out _);
            }
            foreach (var entry in modifiedValuesWrite2)
            {
                byte[] valueBytes = BitConverter.GetBytes(entry.Value);
                WriteProcessMemory(ProcessHandle, new IntPtr(entry.Key), valueBytes, valueBytes.Length, out _);
            }
        }

        public bool SaveScope()
        {
            SYSTEM_INFO si = new SYSTEM_INFO();
            GetSystemInfo(out si);
            long startAddress = si.lpMinimumApplicationAddress.ToInt64();
            long endAddress = si.lpMaximumApplicationAddress.ToInt64();

            byte[] searchWallHack = new byte[] { 0xCC, 0x3D, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x33, 0x33, 0x13, 0x40, 0x00, 0x00, 0xB0, 0x3F, 0x00, 0x00, 0x80, 0x3F };

            // The handle is now managed by Form2, so we just check if it's valid.
            if (ProcessHandle == IntPtr.Zero)
            {
                notificationSystem.Notification1("", "Process handle is not valid!", "red");
                return false;
            }

            notificationSystem.Notification1("", "Loading Sniper Scope...", "green");

            Oldspeed.Clear();
            Newspeed.Clear();

            if (!FindPattern(startAddress, endAddress, searchWallHack, Newspeed))
            {
                notificationSystem.Notification1("", "Pattern search failed.", "red");
                return false;
            }

            if (Newspeed.Count == 0)
            {
                notificationSystem.Notification1("", "Address Not Found!", "red");
                return false;
            }

            notificationSystem.Notification1("", "Sniper Scope Loaded", "green");
            return true;
        }

        public bool EnableScope()
        {
            byte[] replace = { 0xFF };
            byte[] replace1 = { 0xFF, 0xFF };

            if (Newspeed.Count == 0)
                return false;

            foreach (var address in Newspeed)
            {
                WriteProcessMemory(ProcessHandle, new IntPtr(address + 8L), replace, replace.Length, out _);
                WriteProcessMemory(ProcessHandle, new IntPtr(address + 13L), replace1, replace1.Length, out _);
            }

            return true;
        }

        public bool DisableScope()
        {
            byte[] replace = { 0x00 };
            byte[] replace1 = { 0x00, 0x00 };

            if (Newspeed.Count == 0)
                return false;

            foreach (var address in Newspeed)
            {
                WriteProcessMemory(ProcessHandle, new IntPtr(address + 8L), replace, replace.Length, out _);
                WriteProcessMemory(ProcessHandle, new IntPtr(address + 13L), replace1, replace1.Length, out _);
            }

            return true;
        }

        public bool ReplacePattern(long startRange, long endRange, byte[] searchAob, byte[] replaceAob)
        {
            if (replaceAob == null || replaceAob.Length == 0)
                return false;

            List<long> foundedAddress = new List<long>();
            FindPattern(startRange, endRange, searchAob, foundedAddress);
            if (foundedAddress.Count == 0)
                return false;

            uint oldProtect;
            foreach (long address in foundedAddress)
            {
                IntPtr currentAddress = new IntPtr(address);
                VirtualProtectEx(ProcessHandle, currentAddress, (uint)replaceAob.Length, PAGE_EXECUTE_READWRITE, out oldProtect);
                WriteProcessMemory(ProcessHandle, currentAddress, replaceAob, replaceAob.Length, out _);
                VirtualProtectEx(ProcessHandle, currentAddress, (uint)replaceAob.Length, PAGE_EXECUTE_READ, out oldProtect);
            }

            return true;
        }

        public bool ChangePattern(long startRange, long endRange, byte[] search, byte[] replace)
        {
            // AttackProcess is handled by Form2
            if (ProcessHandle == IntPtr.Zero)
                return false;

            bool status = ReplacePattern(startRange, endRange, search, replace);
            return status;
        }

        public bool HookPattern(long startRange, long endRange, byte[] searchAob, List<long> addressRet, int offset, byte[] newValue, int valueSize)
        {
            if (ProcessHandle == IntPtr.Zero)
                return false;

            if (addressRet.Count == 0)
            {
                FindPattern(startRange, endRange, searchAob, addressRet);
                if (addressRet.Count == 0)
                    return false;

                foreach (long address in addressRet)
                {
                    long targetAddress = address + offset;
                    WriteProcessMemory(ProcessHandle, new IntPtr(targetAddress), newValue, valueSize, out _);
                }
                return true;
            }
            else
            {
                foreach (long address in addressRet)
                {
                    long targetAddress = address + offset;
                    WriteProcessMemory(ProcessHandle, new IntPtr(targetAddress), newValue, valueSize, out _);
                }
                return true;
            }

            return true;
        }

        public bool FindPattern(long startRange, long endRange, byte[] searchBytes, List<long> addressRet)
        {
            IntPtr dwAddress = new IntPtr(startRange);
            List<MemoryRegion> memoryRegions = new List<MemoryRegion>();
            MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();

            while (VirtualQueryEx(ProcessHandle, dwAddress, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))))
            {
                if (dwAddress.ToInt64() >= endRange) break;

                if (mbi.State == MEM_COMMIT &&
                    (mbi.Protect & PAGE_GUARD) == 0 &&
                    (mbi.Protect != PAGE_NOACCESS) &&
                    (mbi.AllocationProtect & PAGE_NOCACHE) != PAGE_NOCACHE)
                {
                    memoryRegions.Add(new MemoryRegion
                    {
                        BaseAddress = mbi.BaseAddress.ToInt64(),
                        MemorySize = mbi.RegionSize.ToInt64()
                    });
                }
                dwAddress = new IntPtr(mbi.BaseAddress.ToInt64() + mbi.RegionSize.ToInt64());
            }

            foreach (var region in memoryRegions)
            {
                byte[] buffer = new byte[region.MemorySize];
                if (!ReadProcessMemory(ProcessHandle, new IntPtr(region.BaseAddress), buffer, buffer.Length, out IntPtr bytesRead))
                    continue;

                int offset = 0;
                while (true)
                {
                    int foundIndex = Memfind(buffer, (long)bytesRead, searchBytes, offset);
                    if (foundIndex == -1) break;

                    long firstByteAddress = region.BaseAddress + foundIndex;
                    if (!modifiedAoBs.ContainsKey(firstByteAddress))
                    {
                        addressRet.Add(firstByteAddress);
                        modifiedAoBs[firstByteAddress] = 1;
                    }
                    offset = foundIndex + searchBytes.Length;
                }
            }
            return true;
        }

        public int Memfind(byte[] buffer, long bufferSize, byte[] searchPattern, int startOffset)
        {
            for (int i = startOffset; i < bufferSize - searchPattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < searchPattern.Length; j++)
                {
                    if (searchPattern[j] == 0xCC) continue;
                    if (searchPattern[j] == 0xFC && buffer[i + j] == 0x00) continue;

                    if (buffer[i + j] != searchPattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return i;
            }
            return -1;
        }

        // --- Thread Management ---
        private Thread monitorThread;
        private volatile bool stopThread = false;

        private void MonitorLeftClickInternal()
        {
            bool isScoped = false;

            while (!stopThread)
            {
                bool isPressed = (User32.GetAsyncKeyState((int)VirtualKey.VK_LBUTTON) & 0x8000) != 0;

                if (isPressed)
                {
                    if (!isScoped)
                    {
                        this.EnableScope();
                        isScoped = true;
                    }
                }
                else
                {
                    if (isScoped)
                    {
                        this.DisableScope();
                        isScoped = false;
                    }
                }

                Thread.Sleep(1);
            }
        }

        public void StartMonitor()
        {
            if (monitorThread == null || !monitorThread.IsAlive)
            {
                stopThread = false;
                monitorThread = new Thread(MonitorLeftClickInternal);
                monitorThread.IsBackground = true;
                monitorThread.Start();
            }
        }

        public void StopMonitor()
        {
            stopThread = true;
            if (monitorThread != null && monitorThread.IsAlive)
            {
                monitorThread.Join();
            }
        }
    }
}