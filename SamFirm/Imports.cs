using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SamFirm
{
    internal class Imports
    {
        private static IntPtr mod = IntPtr.Zero;

        //콘솔을 메모리에서 반환하는 메소드
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        //모듈(라이브러리)을 메모리에서 반환하는 메소드 
        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);
        public static void FreeModule()
        {
            if (mod != IntPtr.Zero)
            {
                if (!FreeLibrary(mod))
                {
                    Logger.WriteLog("Error: Unable to free library");
                }
                mod = IntPtr.Zero;
            }
        }

        //권한을 얻는 메소드
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
        private static T load_function<T>(IntPtr module, string name) where T: class => 
            (Marshal.GetDelegateForFunctionPointer(GetProcAddress(module, name), typeof(T)) as T);

        //모듈(라이브러리)을 로드하는 메소드
        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);
        private static int LoadModule(string module = "AgentModule.dll")
        {
            try
            {
                string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                module = Path.Combine(directoryName, "DLL", module);
                if (!File.Exists(module))
                {
                    Logger.WriteLog("Error: Library " + module + " does not exist");
                    return 1;
                }

                mod = LoadLibrary(module);
                if (mod == IntPtr.Zero)
                {
                    Logger.WriteLog("Error loading library: " + Marshal.GetLastWin32Error());
                    Logger.WriteLog("Please make sure \"Microsoft Visual C++ 2008 Redistributable Package (x86)\" and \"Microsoft Visual C++ 2010 Redistributable Package (x86)\" are installed");
                    return 1;
                }
            }
            catch (Exception exception)
            {
                Logger.WriteLog("Error Loading Module: " + exception.Message);
                return 1;
            }
            return 0;
        }

        //스레드 실행 상태를 설정하는 메소드
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate IntPtr Auth_t(IntPtr nonce);

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