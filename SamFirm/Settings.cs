using System;
using System.IO;
using System.Xml.Linq;

namespace SamFirm
{
    internal static class Settings
    {
        //설정파일의 이름을 가진 상수
        private const string settingsXml = "Settings.xml";

        //설정파일을 만드는 메소드
        private static void GenerateSettings()
        {
            string default_contents =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<SamFirm>\r\n" +
                    "\t<Model></Model>\r\n" +
                    "\t<Region></Region>\r\n" +
                    "\t<PDAVer></PDAVer>\r\n" +
                    "\t<CSCVer></CSCVer>\r\n" +
                    "\t<PHONEVer></PHONEVer>\r\n" +
                    "\t<AutoInfo>True</AutoInfo>\r\n" +
                    "\t<BinaryNature>False</BinaryNature>\r\n" +
                    "\t<CheckCRC>True</CheckCRC>\r\n" +
                    "\t<AutoDecrypt>True</AutoDecrypt>\r\n" +
                "</SamFirm>";
            File.WriteAllText(settingsXml, default_contents);
        }

        //설정파일을 읽는 메소드
        public static string ReadSetting(string element)
        {
            try
            {
                if (!File.Exists(settingsXml))
                {
                    GenerateSettings();
                }
                return XDocument.Load(settingsXml).Element("SamFirm").Element(element).Value;
            }
            catch (Exception exception)
            {
                Logger.WriteLine("Error ReadSetting() -> " + exception);
                return string.Empty;
            }
        }

        //설정파일을 쓰는 메소드
        public static void SetSetting(string element, string value)
        {
            if (!File.Exists(settingsXml))
            {
                GenerateSettings();
            }
            XDocument document = XDocument.Load(settingsXml);
            XElement element2 = document.Element("SamFirm").Element(element);
            if (element2 == null)
            {
                document.Element("SamFirm").Add(new XElement(element, value));
            }
            else
            {
                element2.Value = value;
            }
            document.Save(settingsXml);
        }
    }
}