namespace SamFirm
{
    using DamienG.Security.Cryptography;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    public static class Utility
    {
        private static Stopwatch dswatch = new Stopwatch();
        private static int interval = 0;
        private static long lastBread = 0L;
        private static int lastSpeed = 0;
        public static bool ReconnectDownload = false;
        public static bool run_by_cmd = false;

        public static bool CheckConnection(string address, ref bool docheck)
        {
            bool flag = false;
            Ping ping = new Ping();
            while (docheck)
            {
                if (flag)
                {
                    return flag;
                }
                try
                {
                    flag = ping.Send(address, 0x7d0).Status == IPStatus.Success;
                }
                catch (PingException)
                {
                }
            }
            return false;
        }

        public static int CheckHTMLXMLStatus(int htmlstatus, int xmlstatus = 0)
        {
            int num = (xmlstatus == 0) ? htmlstatus : xmlstatus;
            switch (num)
            {
                case 400:
                    Logger.WriteLog("    Request was invalid. Are you sure the input data is correct?", false);
                    return num;

                case 0x191:
                    Logger.WriteLog("    Authorization failed", false);
                    return num;
            }
            return num;
        }

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

        public static bool CRCCheck(string file, byte[] crc)
        {
            byte[] buffer;
            if (!System.IO.File.Exists(file))
            {
                throw new FileNotFoundException("File for crc check not found");
            }
            Crc32 crc2 = new Crc32();
            using (FileStream stream = System.IO.File.Open(file, FileMode.Open, FileAccess.Read))
            {
                buffer = crc2.ComputeHash(stream);
            }
            return crc.Compare(buffer);
        }

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
            double num = ((double) sw.ElapsedMilliseconds) / 1000.0;
            long num2 = bread - lastBread;
            int num3 = (int) Math.Floor((double) ((((double) num2) / num) / 1024.0));
            if (lastSpeed != 0)
            {
                num3 = (lastSpeed + num3) / 2;
            }
            lastSpeed = num3;
            lastBread = bread;
            sw.Reset();
            return Round(num3, 2);
        }

        public static char[] GetCharArray(int size, char init)
        {
            char[] chArray = new char[size];
            for (int i = 0; i < size; i++)
            {
                chArray[i] = init;
            }
            return chArray;
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
            float num = (((float) value) / ((float) total)) * 100f;
            return (int) num;
        }

        public static char[] GetSpaceArray(int size) => 
            GetCharArray(size, ' ');

        public static int GetXMLStatusCode(string xml)
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
            return 0x29a;
        }

        public static string InfoExtract(string info, string type)
        {
            string[] strArray = info.Split(new char[] { '/' });
            if (strArray.Length >= 2)
            {
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
                }
            }
            return string.Empty;
        }

        public static bool IsRunningOnMono() => 
            (Type.GetType("Mono.Runtime") != null);

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
            if (dswatch.ElapsedMilliseconds > 0x7530L)
            {
                Imports.SetThreadExecutionState(Imports.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
                PreventDeepSleep(PDSMode.Start);
            }
        }

        public static void Reconnect(Action<object, EventArgs> action)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate (object o, DoWorkEventArgs _e) {
                Thread.Sleep(0x3e8);
                if (CheckConnection("cloud-neofussvr.sslcs.cdngc.net", ref ReconnectDownload))
                {
                    Form1.DownloadEventArgs args = new Form1.DownloadEventArgs {
                        isReconnect = true
                    };
                    action(null, args);
                }
            };
            worker.RunWorkerAsync();
        }

        public static void ReconnectCmdLine()
        {
            CheckConnection("cloud-neofussvr.sslcs.cdngc.net", ref ReconnectDownload);
        }

        public static void ResetSpeed(long _lastBread)
        {
            interval = lastSpeed = 0;
            lastBread = _lastBread;
        }

        public static int Round(int num, int pos)
        {
            double num2 = Math.Pow(10.0, (double) pos);
            if (num2 > num)
            {
                return num;
            }
            return ((num / ((int) num2)) * ((int) num2));
        }

        public static void TaskBarProgressState(bool paused)
        {
            try
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
            catch (Exception)
            {
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

