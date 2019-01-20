namespace SamFirm
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Imports.FreeConsole();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                Utility.run_by_cmd = true;
                Environment.Exit(CmdLine.Main(args));
            }
            return 0;
        }

        private static void SendEnterToParent()
        {
            Imports.EnumWindows(delegate (IntPtr wnd, IntPtr param) {
                uint lpdwProcessId = 0;
                Imports.GetWindowThreadProcessId(wnd, out lpdwProcessId);
                Process parentProcess = Imports.ParentProcessUtilities.GetParentProcess();
                if (lpdwProcessId == parentProcess.Id)
                {
                    Imports.SendMessage(wnd, 0x102, (IntPtr) 13, IntPtr.Zero);
                    return false;
                }
                return true;
            }, IntPtr.Zero);
        }
    }
}

