using System;
using System.IO;
using System.Windows.Forms;

namespace SamFirm
{
    internal static class Logger
    {
        public static Form1 form;

        private static void CleanLog()
        {
            if (form.log_textbox.InvokeRequired)
            {
                form.log_textbox.Invoke(new Action(delegate {
                    if (form.log_textbox.Lines.Length > 30)
                    {
                        form.log_textbox.Text.Remove(0, form.log_textbox.GetFirstCharIndexFromLine(1));
                    }
                }));
            }
            else if (form.log_textbox.Lines.Length > 30)
            {
                form.log_textbox.Text.Remove(0, form.log_textbox.GetFirstCharIndexFromLine(1));
            }
        }

        private static string GetTimeDate() => 
            (DateTime.Now.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToString("HH:mm:ss"));

        public static void SaveLog()
        {
            if (!string.IsNullOrEmpty(form.log_textbox.Text))
            {
                if (File.Exists("SamFirm.log") && (new FileInfo("SamFirm.log").Length > 0x200000L))
                {
                    File.Delete("SamFirm.log.old");
                    File.Move("SamFirm.log", "SamFirm.log.old");
                }
                using (TextWriter writer = new StreamWriter(new FileStream("SamFirm.log", FileMode.Append)))
                {
                    writer.WriteLine();
                    writer.WriteLine(GetTimeDate());
                    foreach (string str in form.log_textbox.Lines)
                    {
                        writer.WriteLine(str);
                    }
                }
            }
        }

        public static void WriteLog(string str, bool raw = false)
        {
            MethodInvoker method = null;
            CleanLog();
            if (!raw)
            {
                str = str + "\n";
            }
            if (form.log_textbox.InvokeRequired)
            {
                if (method == null)
                {
                    method = delegate {
                        form.log_textbox.AppendText(str);
                        form.log_textbox.ScrollToCaret();
                    };
                }
                form.log_textbox.Invoke(method);
            }
            else
            {
                form.log_textbox.AppendText(str);
                form.log_textbox.ScrollToCaret();
            }
        }
    }
}