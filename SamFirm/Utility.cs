using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace SamFirm
{
    public static class Utility
    {
        private static Stopwatch dswatch = new Stopwatch();
        private static int interval;
        private static long lastBread;
        private static int lastSpeed;
        public static bool ReconnectDownload { get; set; }

        public static bool CheckConnection(string address)
        {
            bool flag = false;
            Ping ping = new Ping();
            for (int i = 1; i <= 4; i++)
            {
                if (flag == true)
                {
                    return true;
                }

                flag = ping.Send(address, 2000).Status == IPStatus.Success;
            }
            return false;
        }

        public static int CheckHtmlXmlStatus(int htmlstatus, int xmlstatus)
        {
            int code = (xmlstatus == 0) ? htmlstatus : xmlstatus;
            switch (code)
            {
                case 400:
                    Logger.WriteLine("Error CheckHtmlXmlStatus(): Request was invalid. Please check the input data.");
                    return code;

                case 401:
                    Logger.WriteLine("Error CheckHtmlXmlStatus(): Authorization failed.");
                    return code;

                default:
                    return code;
            }
        }

        //배열 2개가 같은 원소를 가졌는지 검사하는 메소드
        public static bool Compare(this byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
            {
                return false;
            }
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }

        //CRC32를 검사하는 메소드
        public static bool CRCCheck(string file, byte[] crc)
        {
            byte[] buffer;
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("File for crc check not found");
            }
            Crc32 crc2 = new Crc32();
            using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                buffer = crc2.ComputeHash(stream);
            }
            return crc.Compare(buffer);
        }

        //다운로드 속도를 알아내는 메소드
        public static int DownloadSpeed(long bread, Stopwatch sw)
        {
            if (!sw.IsRunning)
            {
                sw.Start();
            }
            if (interval < 150)
            {
                interval++;
                return -1;
            }
            interval = 0;
            double num = sw.ElapsedMilliseconds / 1000.0;
            long num2 = bread - lastBread;
            int num3 = (int) Math.Floor(num2 / num / 1024.0);
            if (lastSpeed != 0)
            {
                num3 = (lastSpeed + num3) / 2;
            }
            lastSpeed = num3;
            lastBread = bread;
            sw.Reset();
            return Round(num3, 2);
        }

        public static string GetHtml(string url)
        {
            int num = 0;
        Label_0002:
            try
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch (WebException)
            {
                if (num < 2)
                {
                    num++;
                    goto Label_0002;
                }
            }
            return string.Empty;
        }

        public static string GetLogicCheck(string input, string nonce)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            int num = 0;
            if (input.EndsWith(".zip.enc2") || input.EndsWith(".zip.enc4"))
            {
                num = input.Length - 0x19;
            }
            foreach (char ch in nonce)
            {
                int num2 = ch & '\x000f';
                if (input.Length <= (num2 + num))
                {
                    return string.Empty;
                }
                builder.Append(input[num2 + num]);
            }
            return builder.ToString();
        }

        public static int GetProgress(long value, long total)
        {
            float num = ((float) value) / total * 100f;
            return (int) num;
        }

        public static int GetXmlStatusCode(string xml)
        {
            int num;
            if (string.IsNullOrEmpty(xml))
            {
                return 0;
            }
            if (int.TryParse(Xml.GetXMLValue(xml, "FUSBody/Results/Status", null, null), out num))
            {
                return num;
            }
            return 666;
        }

        public static string InfoExtract(string info, string type)
        {
            string[] strArray = info.Split(new [] { '/' });

            if (strArray.Length < 2)
            {
                Logger.WriteLine("Error InfoExtract(): The number of info is too small.");
                return string.Empty;
            }

            switch (type)
            {
                case "pda":
                    return strArray[0];

                case "csc":
                    return strArray[1];

                case "phone":
                    if ((strArray.Length >= 3) && !string.IsNullOrEmpty(strArray[2]))
                    {
                        return strArray[2];
                    }
                    return strArray[0];

                case "data":
                    if (strArray.Length < 4)
                    {
                        return strArray[0];
                    }
                    return strArray[3];

                default:
                    Logger.WriteLine("Error InfoExtract(): Wrong type of info.");
                    return string.Empty;
            }
        }

        public static void PreventDeepSleep(PDSMode mode)
        {
            if (mode == PDSMode.Start)
            {
                dswatch.Reset();
                dswatch.Start();
            }
            else if (mode == PDSMode.Stop)
            {
                dswatch.Stop();
            }
            if (dswatch.ElapsedMilliseconds > 30000L)
            {
                Imports.SetThreadExecutionState(Imports.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
                PreventDeepSleep(PDSMode.Start);
            }
        }

        public static void Reconnect(Action<object, EventArgs> action)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                Thread.Sleep(1000);
                if (CheckConnection("cloud-neofussvr.sslcs.cdngc.net"))
                {
                    MainForm.DownloadEventArgs args = new MainForm.DownloadEventArgs {
                        isReconnect = true
                    };
                    action(null, args);
                }
            };
            worker.RunWorkerAsync();
        }

        public static void ResetSpeed(long _lastBread)
        {
            lastSpeed = 0;
            interval = 0;
            lastBread = _lastBread;
        }

        public static int Round(int num, int pos)
        {
            double num2 = Math.Pow(10.0, pos);
            if (num2 > num)
            {
                return num;
            }
            return num / ((int) num2) * ((int) num2);
        }

        public static void TaskBarProgressPaused(bool paused)
        {
            if (paused)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
            }
            else
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            }
        }

        public enum PDSMode
        {
            Start,
            Stop,
            Continue
        }
    }
}