namespace SamFirm
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class Logger
    {
        public static Form1 form;
        public static bool nologging = false;

        private static void CleanLog()
        {
            if (!Utility.run_by_cmd)
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
        }

        private static string GetTimeDate() => 
            (DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss"));

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
            if (!nologging)
            {
                CleanLog();
                if (!raw)
                {
                    str = str + "\n";
                }
                if (Utility.run_by_cmd)
                {
                    Console.Write(str);
                }
                else if (form.log_textbox.InvokeRequired)
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
}

