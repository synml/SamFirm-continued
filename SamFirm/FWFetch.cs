using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SamFirm
{
    internal static class FWFetch
    {
        public static List<Func<string, string, string>> FWFetchFuncs;
        private static string SamMobileHtml;
        private static string SamsungFirmwareOrgHtml;

        static FWFetch()
        {
            List<Func<string, string, string>> list = new List<Func<string, string, string>> {
                new Func<string, string, string>(FWFetch.FOTAInfoFetch1),
                new Func<string, string, string>(FWFetch.FOTAInfoFetch2),
                new Func<string, string, string>(FWFetch.SamsungFirmwareOrgFetch1),
                new Func<string, string, string>(FWFetch.SamsungFirmwareOrgFetch2),
                new Func<string, string, string>(FWFetch.SamMobileFetch1),
                new Func<string, string, string>(FWFetch.SamMobileFetch2),
                new Func<string, string, string>(FWFetch.SamsungUpdatesFetch)
            };
            FWFetchFuncs = list;
        }

        public static string FOTAInfoFetch(string model, string region, bool latest = true)
        {
            string str3;
            try
            {
                using (WebClient client = new WebClient())
                {
                    string xml = client.DownloadString("http://fota-cloud-dn.ospserver.net/firmware/" + region + "/" + model + "/version.xml");
                    string str2 = null;
                    if (latest)
                    {
                        str2 = Xml.GetXMLValue(xml, "firmware/version/latest", null, null).ToUpper();
                    }
                    else
                    {
                        str2 = Xml.GetXMLValue(xml, "firmware/version/upgrade/value", null, null).ToUpper();
                    }
                    str3 = str2;
                }
            }
            catch (Exception)
            {
                str3 = string.Empty;
            }
            return str3;
        }

        public static string FOTAInfoFetch1(string model, string region) => 
            FOTAInfoFetch(model, region, true);

        public static string FOTAInfoFetch2(string model, string region) => 
            FOTAInfoFetch(model, region, false);

        private static string GetInfoSFO(string html, string search)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            int index = html.IndexOf(">" + search + "<");
            if (index < 0)
            {
                return string.Empty;
            }
            index += (search.Length + 1) + 0x13;
            string str = html.Substring(index);
            return str.Substring(0, str.IndexOf('<'));
        }

        public static string SamMobileFetch(string model, string region, int index)
        {
            string samMobileHtml = SamMobileHtml;
            if (string.IsNullOrEmpty(samMobileHtml))
            {
                return string.Empty;
            }
            if (samMobileHtml.Contains("Device model not found"))
            {
                return string.Empty;
            }
            int num = 0;
            while ((index-- >= 0) && (num >= 0))
            {
                num = samMobileHtml.IndexOf("<a class=\"firmware-table-row\" href=\"", num + 1);
            }
            if (num < 0)
            {
                return string.Empty;
            }
            string str2 = samMobileHtml.Substring(num + 0x24);
            samMobileHtml = Utility.GetHtml(str2.Substring(0, str2.IndexOf('"')));

            StringReader reader = new StringReader(samMobileHtml);
            StringBuilder strbld = new StringBuilder();
            string str4;
            bool flag = false;
            while ((str4 = reader.ReadLine()) != null)
            {
                string str5 = tdExtract(str4).Trim();
                if ((str5 == "PDA") || (str5 == "CSC"))
                {
                    flag = true;
                    continue;
                }

                if (flag)
                {
                    strbld.Append(str5 + "/");
                    flag = false;
                }
            }
            string input = strbld.ToString();
            return Regex.Replace(input, "^(.*)/(.*)/$", "$1/$2/$1");
        }

        public static string SamMobileFetch1(string model, string region)
        {
            try
            {
                SamMobileHtml = Utility.GetHtml("http://www.sammobile.com/firmwares/database/" + model + "/" + region + "/");
                return SamMobileFetch(model, region, 0);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string SamMobileFetch2(string model, string region) => 
            SamMobileFetch(model, region, 1);

        public static string SamsungFirmwareOrgFetch(string model, string region)
        {
            string samsungFirmwareOrgHtml = SamsungFirmwareOrgHtml;
            int index = samsungFirmwareOrgHtml.IndexOf("\"/model/" + model + "/\"");
            if (index < 0)
            {
                return string.Empty;
            }
            samsungFirmwareOrgHtml = samsungFirmwareOrgHtml.Substring(index);
            string url = "https://samsung-firmware.org";
            using (StringReader reader = new StringReader(samsungFirmwareOrgHtml))
            {
                string str3;
                while ((str3 = reader.ReadLine()) != null)
                {
                    if (str3.Contains("Download"))
                    {
                        int num2 = str3.IndexOf('"');
                        int length = str3.Substring(num2 + 1).IndexOf('"');
                        url = url + str3.Substring(num2 + 1, length);
                        goto Label_0098;
                    }
                }
            }
        Label_0098:
            samsungFirmwareOrgHtml = Utility.GetHtml(url);
            string infoSFO = GetInfoSFO(samsungFirmwareOrgHtml, "PDA Version");
            string str5 = GetInfoSFO(samsungFirmwareOrgHtml, "CSC Version");
            string str6 = GetInfoSFO(samsungFirmwareOrgHtml, "PHONE Version");
            if ((string.IsNullOrEmpty(infoSFO) || string.IsNullOrEmpty(str5)) || string.IsNullOrEmpty(str6))
            {
                return string.Empty;
            }
            return (infoSFO + "/" + str5 + "/" + str6);
        }

        public static string SamsungFirmwareOrgFetch1(string model, string region)
        {
            SamsungFirmwareOrgHtml = Utility.GetHtml("https://samsung-firmware.org/model/" + model + "/region/" + region + "/");
            return SamsungFirmwareOrgFetch(model, region);
        }

        public static string SamsungFirmwareOrgFetch2(string model, string region)
        {
            string str = SamsungFirmwareOrgFetch(model, region);
            if (!string.IsNullOrEmpty(str))
            {
                string[] strArray = str.Split(new [] { '/' });
                str = strArray[0] + "/" + strArray[2] + "/" + strArray[1];
            }
            return str;
        }

        private static string SamsungUpdatesFetch(string model, string region)
        {
            string str5;
            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString("http://samsung-updates.com/device/?id=" + model);
                    string address = "http://samsung-updates.com";
                    bool flag = false;
                    using (StringReader reader = new StringReader(s))
                    {
                        string str3;
                        while ((str3 = reader.ReadLine()) != null)
                        {
                            if (str3.Contains("/" + model + "/" + region + "/"))
                            {
                                int index = str3.IndexOf("a href=\"");
                                int length = str3.Substring(index + 8).IndexOf('"');
                                address = address + str3.Substring(index + 8, length);
                                flag = true;
                                goto Label_00BE;
                            }
                        }
                    }
                Label_00BE:
                    if (!flag)
                    {
                        return string.Empty;
                    }
                    s = client.DownloadString(address);
                    Match match = Regex.Match(s, @"PDA:</b> ([^\s]+) <b>");
                    if (!match.Success)
                    {
                        return string.Empty;
                    }
                    string format = match.Groups[1].Value + "/{0}/" + match.Groups[1].Value;
                    match = Regex.Match(s, @"CSC:</b> ([^\s]+) <b>");
                    if (!match.Success)
                    {
                        return string.Empty;
                    }
                    str5 = string.Format(format, match.Groups[1].Value);
                }
            }
            catch (WebException)
            {
                str5 = string.Empty;
            }
            return str5;
        }

        private static string tdExtract(string line)
        {
            int startIndex = line.IndexOf("<td>") + 4;
            int index = line.IndexOf("</td>");
            if ((startIndex >= 0) && (index >= 0))
            {
                return line.Substring(startIndex, index - startIndex);
            }
            return string.Empty;
        }
    }
}