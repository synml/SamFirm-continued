using System;
using System.IO;
using System.Xml.Linq;

namespace SamFirm
{
    internal class Settings
    {
        //설정파일의 이름을 가진 상수
        private const string settingXml = "Settings.xml";
        private const string settingXml_name = "Settings";

        //설정파일을 만드는 메소드
        private static void GenerateSettings()
        {
            string default_contents =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<SamFirm>\r\n" +
                    "\t<SaveFileDialog>True</SaveFileDialog>\r\n" +
                    "\t<AutoInfo>True</AutoInfo>\r\n" +
                    "\t<Region></Region>\r\n" +
                    "\t<Model></Model>\r\n" +
                    "\t<PDAVer></PDAVer>\r\n" +
                    "\t<CSCVer></CSCVer>\r\n" +
                    "\t<PHONEVer></PHONEVer>\r\n" +
                    "\t<BinaryNature></BinaryNature>\r\n" +
                    "\t<CheckCRC>True</CheckCRC>\r\n" +
                    "\t<AutoDecrypt>True</AutoDecrypt>\r\n" +
                "</SamFirm>";
            File.WriteAllText(settingXml, default_contents);
        }

        //설정파일을 읽는 메소드
        public static string ReadSetting(string element)
        {
            try
            {
                if (!File.Exists(settingXml))
                {
                    GenerateSettings();
                }
                return XDocument.Load(settingXml).Element(settingXml_name).Element(element).Value;
            }
            catch (Exception exception)
            {
                Logger.WriteLog("Error reading config file: " + exception.Message, false);
                return string.Empty;
            }
        }

        //설정파일을 쓰는 메소드
        public static void SetSetting(string element, string value)
        {
            if (!File.Exists(settingXml))
            {
                GenerateSettings();
            }
            XDocument document = XDocument.Load(settingXml);
            XElement element2 = document.Element(settingXml_name).Element(element);
            if (element2 == null)
            {
                document.Element(settingXml_name).Add(new XElement(element, value));
            }
            else
            {
                element2.Value = value;
            }
            document.Save(settingXml);
        }
    }
}