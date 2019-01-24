using System;
using System.IO;
using System.Windows.Forms;

namespace SamFirm
{
    internal static class Logger
    {
        internal static Form1 form;

        //로그 텍스트 박스의 줄 개수가 30개를 초과하면 지우는 메소드
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

        //현재 날짜와 시각을 알아내는 함수
        private static string GetTimeDate()
        {
            return DateTime.Now.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToString("HH:mm:ss");
        }

        //로그를 파일로 저장하는 메소드
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

        //로그 텍스트 박스에 문자열을 출력하는 메소드
        public static void WriteLine(string str)
        {
            MethodInvoker method = null;

            CleanLog();

            if (form.log_textbox.InvokeRequired)
            {
                method = delegate {
                    form.log_textbox.AppendText(str + "\n");
                    form.log_textbox.ScrollToCaret();
                };
                form.log_textbox.Invoke(method);
            }
            else
            {
                form.log_textbox.AppendText(str + "\n");
                form.log_textbox.ScrollToCaret();
            }
        }
    }
}