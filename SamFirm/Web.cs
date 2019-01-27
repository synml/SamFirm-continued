using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SamFirm
{
    internal static class Web
    {
        public static MainForm Form { get; set; }
        public static string JSessionID { get; set; } = string.Empty;
        public static string Nonce { get; set; } = string.Empty;

        public static int DownloadBinary(string path, string file, string saveTo, string size)
        {
            int num5;
            long num = 0L;
            HttpWebRequest wr = KiesRequest.Create("http://cloud-neofussvr.sslcs.cdngc.net/NF_DownloadBinaryForMass.do?file=" + path + file);
            wr.Method = "GET";
            wr.Headers["Authorization"] = Imports.GetAuthorization(Nonce).Replace("Authorization: ", "").Replace("nonce=\"", "nonce=\"" + Nonce);
            wr.Timeout = 0x61a8;
            wr.ReadWriteTimeout = 0x61a8;
            if (System.IO.File.Exists(saveTo))
            {
                long length = new FileInfo(saveTo).Length;
                if (long.Parse(size) == length)
                {
                    Logger.WriteLine("File already downloaded.");
                    return 200;
                }
                Logger.WriteLine("File exists. Resuming download...");
                wr.AddRange((int) length);
                num = length;
            }
            using (HttpWebResponse response = (HttpWebResponse) wr.GetResponseFUS())
            {
                if (response == null)
                {
                    Logger.WriteLine("Error DownloadBinary(): response is null");
                    return 0x385;
                }
                if ((response.StatusCode != HttpStatusCode.OK) && (response.StatusCode != HttpStatusCode.PartialContent))
                {
                    Logger.WriteLine("Error DownloadBinary(): " + ((int)response.StatusCode));
                }
                else
                {
                    long total = long.Parse(response.GetResponseHeader("content-length")) + num;
                    if (!System.IO.File.Exists(saveTo) || (new FileInfo(saveTo).Length != total))
                    {
                        byte[] buffer = new byte[0x2000];
                        Stopwatch sw = new Stopwatch();
                        Utility.ResetSpeed(num);
                        try
                        {
                            Utility.PreventDeepSleep(Utility.PDSMode.Start);
                            using (BinaryWriter writer = new BinaryWriter(new FileStream(saveTo, FileMode.Append)))
                            {
                                int count = 0;
                                do
                                {
                                    Utility.PreventDeepSleep(Utility.PDSMode.Continue);
                                    if (Form.PauseDownload)
                                    {
                                        goto Label_02BB;
                                    }
                                    count = response.GetResponseStream().Read(buffer, 0, buffer.Length);
                                    num += count;
                                    if (count > 0)
                                    {
                                        writer.Write(buffer, 0, count);
                                        int dlspeed = Utility.DownloadSpeed(num, sw);
                                        if (dlspeed != -1)
                                        {
                                            Form.Speed_label.Invoke(new Action(delegate {
                                                Form.Speed_label.Text = dlspeed + " kB/s";
                                            }));
                                        }
                                    }
                                    Form.SetProgressBar(Utility.GetProgress(num, total));
                                }
                                while (count > 0);
                            }
                        }
                        catch (IOException exception)
                        {
                            Logger.WriteLine("Error DownloadBinary(): Can't access output file " + saveTo);
                            Logger.WriteLine(exception.ToString());
                            Form.PauseDownload = true;
                            return -1;
                        }
                        catch (WebException)
                        {
                            Logger.WriteLine("Error DownloadBinary(): Connection interrupted");
                            SetReconnect();
                        }
                        finally
                        {
                            Utility.PreventDeepSleep(Utility.PDSMode.Stop);
                            Form.Speed_label.Invoke(new Action(() => Form.Speed_label.Text = "0 kB/s"));
                        }
                    }
                }
            Label_02BB:
                num5 = (int) response.StatusCode;
            }
            return num5;
        }

        public static int DownloadBinaryInform(string xml, out string xmlresponse) => 
            XMLFUSRequest("https://neofussvr.sslcs.cdngc.net/NF_DownloadBinaryInform.do", xml, out xmlresponse);

        public static int DownloadBinaryInit(string xml, out string xmlresponse) => 
            XMLFUSRequest("https://neofussvr.sslcs.cdngc.net/NF_DownloadBinaryInitForMass.do", xml, out xmlresponse);

        public static int GenerateNonce()
        {
            HttpWebRequest wr = KiesRequest.Create("https://neofussvr.sslcs.cdngc.net/NF_DownloadGenerateNonce.do");
            wr.Method = "POST";
            wr.ContentLength = 0L;
            using (HttpWebResponse response = (HttpWebResponse) wr.GetResponseFUS())
            {
                if (response == null)
                {
                    return 0x385;
                }
                return (int) response.StatusCode;
            }
        }

        public static void SetReconnect()
        {
            if (!Form.PauseDownload)
            {
                Utility.ReconnectDownload = true;
            }
            Form.PauseDownload = true;
        }

        private static int XMLFUSRequest(string URL, string xml, out string xmlresponse)
        {
            xmlresponse = null;
            HttpWebRequest wr = KiesRequest.Create(URL);
            wr.Method = "POST";
            wr.Headers["Authorization"] = "FUS nonce=\"\", signature=\"" + Imports.GetAuthorization(Nonce) + "\", nc=\"\", type=\"\", realm=\"\"";
            byte[] bytes = Encoding.ASCII.GetBytes(Regex.Replace(xml, @"\r\n?|\n|\t", string.Empty));
            wr.ContentLength = bytes.Length;
            using (Stream stream = wr.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse) wr.GetResponseFUS())
            {
                if (response == null)
                {
                    return 0x385;
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        xmlresponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                    catch (Exception)
                    {
                        return 900;
                    }
                }
                return (int) response.StatusCode;
            }
        }
    }
}