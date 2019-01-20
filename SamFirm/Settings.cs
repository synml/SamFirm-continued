namespace SamFirm
{
    using System;
    using System.IO;
    using System.Xml.Linq;

    internal class Settings
    {
        private const string SettingFile = "SamFirm.xml";

        private static void GenerateSettings()
        {
            string contents = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<SamFirm>\r\n    <SaveFileDialog></SaveFileDialog>\r\n    <AutoInfo></AutoInfo>\r\n\t<Region></Region>\r\n\t<Model></Model>\r\n\t<PDAVer></PDAVer>\r\n\t<CSCVer></CSCVer>\r\n\t<PHONEVer></PHONEVer>\r\n    <BinaryNature></BinaryNature>\r\n    <CheckCRC></CheckCRC>\r\n    <AutoDecrypt></AutoDecrypt>\r\n</SamFirm>";
            File.WriteAllText("SamFirm.xml", contents);
        }

        public static string ReadSetting(string element)
        {
            try
            {
                if (!File.Exists("SamFirm.xml"))
                {
                    GenerateSettings();
                }
                return XDocument.Load("SamFirm.xml").Element("SamFirm").Element(element).Value;
            }
            catch (Exception exception)
            {
                Logger.WriteLog("Error reading config file: " + exception.Message, false);
                return string.Empty;
            }
        }

        public static void SetSetting(string element, string value)
        {
            if (!File.Exists("SamFirm.xml"))
            {
                GenerateSettings();
            }
            XDocument document = XDocument.Load("SamFirm.xml");
            XElement element2 = document.Element("SamFirm").Element(element);
            if (element2 == null)
            {
                document.Element("SamFirm").Add(new XElement(element, value));
            }
            else
            {
                element2.Value = value;
            }
            document.Save("SamFirm.xml");
        }
    }
}

