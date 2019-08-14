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
        internal static readonly List<Func<string, string, string>> FWFetchFuncs;
        private static string SamMobileHtml;
        private static string SamsungFirmwareOrgHtml;

        static FWFetch()
        {
            List<Func<string, string, string>> list = new List<Func<string, string, string>> {
                new Func<string, string, string>(FOTAInfoFetch1),
                new Func<string, string, string>(FOTAInfoFetch2),
                new Func<string, string, string>(SamsungFirmwareOrgFetch1),
                new Func<string, string, string>(SamsungFirmwareOrgFetch2),
                new Func<string, string, string>(SamMobileFetch1),
                new Func<string, string, string>(SamMobileFetch2),
                new Func<string, string, string>(SamsungUpdatesFetch)
            };
            FWFetchFuncs = list;
        }

        //http://fota-cloud-dn.ospserver.net
        public static string FOTAInfoFetch(string model, string region, bool latest)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string xml = client.DownloadString("http://fota-cloud-dn.ospserver.net/firmware/" + region + "/" + model + "/version.xml");
                    string str2;
                    if (latest == true)
                    {
                        str2 = Xml.GetXMLValue(xml, "firmware/version/latest", null, null).ToUpper();
                    }
                    else
                    {
                        str2 = Xml.GetXMLValue(xml, "firmware/version/upgrade/value", null, null).ToUpper();
                    }
                    return str2;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
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

        //http://www.sammobile.com
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

            string input = string.Empty;
            using (StringReader reader = new StringReader(samMobileHtml))
            {
                StringBuilder bld = new StringBuilder();
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
                        bld.Append(str5 + "/");
                        flag = false;
                    }
                }
                input = bld.ToString();
            }
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

        //https://samsung-firmware.org
        public static string SamsungFirmwareOrgFetch(string model, string region)
        {
            string samsungFirmwareOrgHtml = SamsungFirmwareOrgHtml;
            int index = samsungFirmwareOrgHtml.IndexOf("\"/model/" + model + "/\"");
            if (index < 0)
            {
                return string.Empty;
            }
            samsungFirmwareOrgHtml = samsungFirmwareOrgHtml.Substring(index);

            StringBuilder bld = new StringBuilder();
            bld.Append("https://samsung-firmware.org");

            using (StringReader reader = new StringReader(samsungFirmwareOrgHtml))
            {
                string str3;
                while ((str3 = reader.ReadLine()) != null)
                {
                    if (str3.Contains("Download"))
                    {
                        int num2 = str3.IndexOf('"');
                        int length = str3.Substring(num2 + 1).IndexOf('"');
                        bld.Append(str3.Substring(num2 + 1, length));
                        break;
                    }
                }
            }

            string url = bld.ToString();
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

        //http://samsung-updates.com (연결안됨)
        private static string SamsungUpdatesFetch(string model, string region)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString("http://samsung-updates.com/device/?id=" + model);
                    StringBuilder bld = new StringBuilder();
                    bld.Append("http://samsung-updates.com");
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
                                bld.Append(str3.Substring(index + 8, length));
                                flag = true;
                                break;
                            }
                        }
                    }

                    if (!flag)
                    {
                        return string.Empty;
                    }
                    string address = bld.ToString();
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
                    return string.Format(format, match.Groups[1].Value);
                }
            }
            catch (WebException)
            {
                return string.Empty;
            }
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