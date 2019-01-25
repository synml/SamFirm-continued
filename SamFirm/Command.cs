using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SamFirm
{
    internal class Command
    {
        public static int Download(string path, string file, string version, string region, string model_type, string saveTo, string size, bool GUI = true)
        {
            string str;
            int htmlstatus = Web.GenerateNonce();
            if (htmlstatus != 200)
            {
                Logger.WriteLine("Error Download(): Could not generate Nonce. Status code (" + htmlstatus + ")");
                return -1;
            }
            htmlstatus = Web.DownloadBinaryInit(Xml.GetXmlBinaryInit(file, version, region, model_type), out str);
            if ((htmlstatus == 200) && (Utility.GetXMLStatusCode(str) == 200))
            {
                return Web.DownloadBinary(path, file, saveTo, size, GUI);
            }
            Logger.WriteLine("Error Download(): Could not send BinaryInform. Status code (" + htmlstatus + "/" + Utility.GetXMLStatusCode(str) + ")");
            Utility.CheckHTMLXMLStatus(htmlstatus, Utility.GetXMLStatusCode(str));
            return -1;
        }

        public static Firmware UpdateCheck(string model, string region, string info, bool BinaryNature, bool AutoFetch = false)
        {
            if (string.IsNullOrEmpty(info))
            {
                return new Firmware();
            }
            string str = Utility.InfoExtract(info, "pda");
            if (string.IsNullOrEmpty(str))
            {
                return new Firmware();
            }
            string csc = Utility.InfoExtract(info, "csc");
            string phone = Utility.InfoExtract(info, "phone");
            string data = Utility.InfoExtract(info, "data");
            return UpdateCheck(model, region, str, csc, phone, data, BinaryNature, AutoFetch);
        }

        public static Firmware UpdateCheck(string model, string region, string pda, string csc, string phone, string data, bool BinaryNature, bool AutoFetch = false)
        {
            string str;
            Firmware firmware = new Firmware();
            Logger.WriteLine("Checking firmware for " + model + "/" + region + "/" + pda + "/" + csc + "/" + phone + "/" + data);
            int htmlstatus = Web.GenerateNonce();
            if (htmlstatus != 200)
            {
                Logger.WriteLine("Error UpdateCheck(): Could not generate Nonce. Status code (" + htmlstatus + ")");
                firmware.ConnectionError = true;
                return firmware;
            }
            htmlstatus = Web.DownloadBinaryInform(Xml.GetXmlBinaryInform(model, region, pda, csc, phone, data, BinaryNature), out str);
            if ((htmlstatus != 200) || (Utility.GetXMLStatusCode(str) != 200))
            {
                Logger.WriteLine("Error UpdateCheck(): Could not send BinaryInform. Status code (" + htmlstatus + "/" + Utility.GetXMLStatusCode(str) + ")");
                Utility.CheckHTMLXMLStatus(htmlstatus, Utility.GetXMLStatusCode(str));
                return firmware;
            }
            firmware.Version = Xml.GetXMLValue(str, "FUSBody/Results/LATEST_FW_VERSION/Data", null, null);
            firmware.Model = Xml.GetXMLValue(str, "FUSBody/Put/DEVICE_MODEL_NAME/Data", null, null);
            firmware.DisplayName = Xml.GetXMLValue(str, "FUSBody/Put/DEVICE_MODEL_DISPLAYNAME/Data", null, null);
            firmware.OS = Xml.GetXMLValue(str, "FUSBody/Put/LATEST_OS_VERSION/Data", null, null);
            firmware.LastModified = Xml.GetXMLValue(str, "FUSBody/Put/LAST_MODIFIED/Data", null, null);
            firmware.Filename = Xml.GetXMLValue(str, "FUSBody/Put/BINARY_NAME/Data", null, null);
            firmware.Size = Xml.GetXMLValue(str, "FUSBody/Put/BINARY_BYTE_SIZE/Data", null, null);
            string str3 = Xml.GetXMLValue(str, "FUSBody/Put/BINARY_CRC/Data", null, null);
            if (!string.IsNullOrEmpty(str3))
            {
                firmware.CRC = BitConverter.GetBytes(Convert.ToUInt32(str3)).Reverse<byte>().ToArray<byte>();
            }
            firmware.Model_Type = Xml.GetXMLValue(str, "FUSBody/Put/DEVICE_MODEL_TYPE/Data", null, null);
            firmware.Path = Xml.GetXMLValue(str, "FUSBody/Put/MODEL_PATH/Data", null, null);
            firmware.Region = Xml.GetXMLValue(str, "FUSBody/Put/DEVICE_LOCAL_CODE/Data", null, null);
            firmware.BinaryNature = int.Parse(Xml.GetXMLValue(str, "FUSBody/Put/BINARY_NATURE/Data", null, null));
            if (Xml.GetXMLValue(str, "FUSBody/Put/LOGIC_OPTION_FACTORY/Data", null, null) == "1")
            {
                firmware.LogicValueFactory = Xml.GetXMLValue(str, "FUSBody/Put/LOGIC_VALUE_FACTORY/Data", null, null);
            }
            if (Xml.GetXMLValue(str, "FUSBody/Put/LOGIC_OPTION_HOME/Data", null, null) == "1")
            {
                firmware.LogicValueHome = Xml.GetXMLValue(str, "FUSBody/Put/LOGIC_VALUE_HOME/Data", null, null);
            }
            if (!AutoFetch)
            {
                if ((pda + "/" + csc + "/" + phone + "/" + pda) == firmware.Version)
                {
                    Logger.WriteLine("\nCurrent firmware is latest:");
                }
                else
                {
                    Logger.WriteLine("\nNewer firmware available:");
                }
            }
            Logger.WriteLine("Model: " + firmware.Model);
            Logger.WriteLine("Version: " + firmware.Version);
            Logger.WriteLine("OS: " + firmware.OS);
            Logger.WriteLine("Filename: " + firmware.Filename);
            Logger.WriteLine("Size: " + firmware.Size + " bytes");
            if ((firmware.BinaryNature == 1) && !string.IsNullOrEmpty(firmware.LogicValueFactory))
            {
                Logger.WriteLine("LogicValue: " + firmware.LogicValueFactory);
            }
            else if (!string.IsNullOrEmpty(firmware.LogicValueHome))
            {
                Logger.WriteLine("LogicValue: " + firmware.LogicValueHome);
            }
            Logger.WriteLine("");
            return firmware;
        }

        public static Firmware UpdateCheckAuto(string model, string region, bool BinaryNature)
        {
            int num = 0;
            Firmware firmware = new Firmware();
            foreach (Func<string, string, string> func in FWFetch.FWFetchFuncs)
            {
                string str = func(model, region);
                if (!string.IsNullOrEmpty(str))
                {
                    num++;
                    firmware = UpdateCheck(model, region, str, BinaryNature, true);
                    if ((firmware.Version != null) || firmware.ConnectionError)
                    {
                        break;
                    }
                }
            }
            if (firmware.Version == null)
            {
                Logger.WriteLine("Error UpdateCheckAuto(): Could not fetch info for " + model + "/" + region + ". Please check the input data.");
            }
            firmware.FetchAttempts = num;
            return firmware;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Firmware
        {
            public string Model;
            public string DisplayName;
            public string Version;
            public string OS;
            public string LastModified;
            public string Filename;
            public string Path;
            public string Size;
            public byte[] CRC;
            public string Model_Type;
            public string Region;
            public int BinaryNature;
            public string LogicValueFactory;
            public string LogicValueHome;
            public string Announce;
            public bool ConnectionError;
            public int FetchAttempts;
        }
    }
}