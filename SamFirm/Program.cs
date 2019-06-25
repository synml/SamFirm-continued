using System;
using System.Diagnostics;
using System.Reflection;
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
            MainForm mainForm = new MainForm();

            //버전정보를 출력한다.
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            mainForm.Text = "SamFirm Continued (v" + versionInfo.FileVersion + ")";
            Application.Run(mainForm);
        }
    }
}