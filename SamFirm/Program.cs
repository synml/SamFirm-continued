using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace SamFirm
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            Imports.FreeConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();

            //제목 표시줄에 버전정보를 출력한다.
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            mainForm.Text = "SamFirm Continued (v" + versionInfo.FileVersion + ")";

            //프로그램 실행
            Application.Run(mainForm);
        }
    }
}