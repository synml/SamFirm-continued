using System;
using System.Windows.Forms;

namespace SamFirm
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Imports.FreeConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args[0] == "beta")
            {
                Application.Run(new MainForm2());
            }
            else
            {
                Application.Run(new MainForm());
            }
        }
    }
}