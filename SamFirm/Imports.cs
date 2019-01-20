namespace SamFirm
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class Imports
    {
        private static IntPtr mod = IntPtr.Zero;

        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);
        public static void FreeModule()
        {
            if (mod != IntPtr.Zero)
            {
                if (!FreeLibrary(mod))
                {
                    Logger.WriteLog("Error: Unable to free library", false);
                }
                mod = IntPtr.Zero;
            }
        }

        public static string GetAuthorization(string Nonce)
        {
            if ((mod == IntPtr.Zero) && (LoadModule("AgentModule.dll") != 0))
            {
                return string.Empty;
            }
            Auth_t _t = load_function<Auth_t>(mod, "?MakeAuthorizationHeaderWithGeneratedNonceValueAndAMModule@AgentNetworkModule@@CAPB_WPB_W@Z");
            IntPtr nonce = Marshal.StringToHGlobalUni(Nonce);
            string str = Marshal.PtrToStringUni(_t(nonce));
            Marshal.FreeHGlobal(nonce);
            return str;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private static T load_function<T>(IntPtr module, string name) where T: class => 
            (Marshal.GetDelegateForFunctionPointer(GetProcAddress(module, name), typeof(T)) as T);

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);
        private static int LoadModule(string module = "AgentModule.dll")
        {
            try
            {
                if (!File.Exists(module))
                {
                    string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    if (!File.Exists(Path.Combine(directoryName, module)))
                    {
                        Logger.WriteLog("Error: Library " + module + " does not exist", false);
                        return 1;
                    }
                    module = Path.Combine(directoryName, module);
                }
                mod = LoadLibrary(module);
                if (mod == IntPtr.Zero)
                {
                    Logger.WriteLog("Error loading library: " + Marshal.GetLastWin32Error(), false);
                    Logger.WriteLog("Please make sure \"Microsoft Visual C++ 2008 Redistributable Package (x86)\" and \"Microsoft Visual C++ 2010 Redistributable Package (x86)\" are installed", false);
                    return 1;
                }
            }
            catch (Exception exception)
            {
                Logger.WriteLog("Error LoadModule: " + exception.Message, false);
                return 1;
            }
            return 0;
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate IntPtr Auth_t(IntPtr nonce);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x40,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 2,
            ES_SYSTEM_REQUIRED = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
            [DllImport("ntdll.dll")]
            private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref Imports.ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);
            public static Process GetParentProcess() => 
                GetParentProcess(Process.GetCurrentProcess().Handle);

            public static Process GetParentProcess(int id) => 
                GetParentProcess(Process.GetProcessById(id).Handle);

            public static Process GetParentProcess(IntPtr handle)
            {
                int num;
                Imports.ParentProcessUtilities processInformation = new Imports.ParentProcessUtilities();
                int error = NtQueryInformationProcess(handle, 0, ref processInformation, Marshal.SizeOf(processInformation), out num);
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
                try
                {
                    return Process.GetProcessById(processInformation.InheritedFromUniqueProcessId.ToInt32());
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
        }
    }
}