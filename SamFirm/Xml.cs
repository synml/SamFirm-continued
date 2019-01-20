namespace SamFirm
{
    using System;
    using System.Xml.Linq;

    internal class Xml
    {
        private static string BinaryInit = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><FUSMsg>\r\n\t<FUSHdr>\r\n\t\t<ProtoVer>1</ProtoVer>\r\n\t\t<SessionID>0</SessionID>\r\n\t\t<MsgID>1</MsgID>\r\n\t</FUSHdr>\r\n\t<FUSBody>\r\n\t\t<Put>\r\n\t\t\t<CmdID>1</CmdID>\r\n\t\t\t<BINARY_FILE_NAME>\r\n\t\t\t\t<Data>SM-T805_AUT_1_20140929155250_b8l0mvlbba_fac.zip.enc2</Data>\r\n\t\t\t</BINARY_FILE_NAME>\r\n\t\t\t<BINARY_NATURE>\r\n\t\t\t\t<Data>0</Data>\r\n\t\t\t</BINARY_NATURE>\r\n\t\t\t<BINARY_VERSION>\r\n\t\t\t\t<Data>T805XXU1ANFB/T805AUT1ANF1/T805XXU1ANF6/T805XXU1ANFB</Data>\r\n\t\t\t</BINARY_VERSION>\r\n\t\t\t<DEVICE_LOCAL_CODE>\r\n\t\t\t\t<Data>AUT</Data>\r\n\t\t\t</DEVICE_LOCAL_CODE>\r\n\t\t\t<DEVICE_MODEL_TYPE>\r\n\t\t\t\t<Data>9</Data>\r\n\t\t\t</DEVICE_MODEL_TYPE>\r\n            <LOGIC_CHECK>\r\n                <Data>805XXU1ANFU1ANXX</Data>\r\n            </LOGIC_CHECK>\r\n\t\t</Put>\r\n\t\t<Get>\r\n\t\t\t<CmdID>2</CmdID>\r\n\t\t\t<LATEST_FW_VERSION/>\r\n\t\t</Get>\r\n\t</FUSBody>\r\n</FUSMsg>";
        private static string LatestVer = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><FUSMsg>\r\n\t<FUSHdr>\r\n\t\t<ProtoVer>1</ProtoVer>\r\n\t\t<SessionID>0</SessionID>\r\n\t\t<MsgID>1</MsgID>\r\n\t</FUSHdr>\r\n\t<FUSBody>\r\n\t\t<Put>\r\n\t\t\t<CmdID>1</CmdID>\r\n\t\t\t<ACCESS_MODE>\r\n\t\t\t\t<Data>2</Data>\r\n\t\t\t</ACCESS_MODE>\r\n\t\t\t<BINARY_NATURE>\r\n\t\t\t\t<Data>0</Data>\r\n\t\t\t</BINARY_NATURE>\r\n\t\t\t<CLIENT_LANGUAGE>\r\n\t\t\t\t<Type>String</Type>\r\n\t\t\t\t<Type>ISO 3166-1-alpha-3</Type>\r\n\t\t\t\t<Data>1033</Data>\r\n\t\t\t</CLIENT_LANGUAGE>\r\n\t\t\t<CLIENT_PRODUCT>\r\n\t\t\t\t<Data>Smart Switch</Data>\r\n\t\t\t</CLIENT_PRODUCT>\r\n\t\t\t<CLIENT_VERSION>\r\n\t\t\t\t<Data>4.1.16014_12</Data>\r\n\t\t\t</CLIENT_VERSION>\r\n\t\t\t<DEVICE_CONTENTS_DATA_VERSION>\r\n\t\t\t\t<Data>T805XXU1ANF6</Data>\r\n\t\t\t</DEVICE_CONTENTS_DATA_VERSION>\r\n\t\t\t<DEVICE_CSC_CODE2_VERSION>\r\n\t\t\t\t<Data>T805AUT1ANF1</Data>\r\n\t\t\t</DEVICE_CSC_CODE2_VERSION>\r\n\t\t\t<DEVICE_FW_VERSION>\r\n\t\t\t\t<Data>T805XXU1ANFB/T805AUT1ANF1/T805XXU1ANF6/T805XXU1ANFB</Data>\r\n\t\t\t</DEVICE_FW_VERSION>\r\n\t\t\t<DEVICE_LOCAL_CODE>\r\n\t\t\t\t<Data>AUT</Data>\r\n\t\t\t</DEVICE_LOCAL_CODE>\r\n\t\t\t<DEVICE_MODEL_NAME>\r\n\t\t\t\t<Data>SM-T805</Data>\r\n\t\t\t</DEVICE_MODEL_NAME>\r\n\t\t\t<DEVICE_PDA_CODE1_VERSION>\r\n\t\t\t\t<Data>T805XXU1ANE6</Data>\r\n\t\t\t</DEVICE_PDA_CODE1_VERSION>\r\n\t\t\t<DEVICE_PHONE_FONT_VERSION>\r\n\t\t\t\t<Data>T805XXU1ANF6</Data>\r\n\t\t\t</DEVICE_PHONE_FONT_VERSION>\r\n\t\t\t<DEVICE_PLATFORM>\r\n\t\t\t\t<Data>Android</Data>\r\n\t\t\t</DEVICE_PLATFORM>\r\n            <LOGIC_CHECK>\r\n                <Data>805XXU1ANFU1ANXX</Data>\r\n            </LOGIC_CHECK>\r\n\t\t</Put>\r\n\t\t<Get>\r\n\t\t\t<CmdID>2</CmdID>\r\n\t\t\t<LATEST_FW_VERSION/>\r\n\t\t</Get>\r\n\t</FUSBody>\r\n</FUSMsg>";

        public static string GetXmlBinaryInform(string model, string region, string pdaver, string cscver, string phonever, string dataver, bool BinaryNature = false)
        {
            XDocument document = XDocument.Parse(LatestVer);
            XElement element = document.Element("FUSMsg").Element("FUSBody").Element("Put");
            element.Element("DEVICE_MODEL_NAME").Element("Data").Value = model;
            element.Element("DEVICE_LOCAL_CODE").Element("Data").Value = region;
            element.Element("DEVICE_CONTENTS_DATA_VERSION").Element("Data").Value = dataver;
            element.Element("DEVICE_CSC_CODE2_VERSION").Element("Data").Value = cscver;
            element.Element("DEVICE_PDA_CODE1_VERSION").Element("Data").Value = pdaver;
            element.Element("DEVICE_PHONE_FONT_VERSION").Element("Data").Value = phonever;
            element.Element("DEVICE_FW_VERSION").Element("Data").Value = pdaver + "/" + cscver + "/" + phonever + "/" + dataver;
            element.Element("BINARY_NATURE").Element("Data").Value = Convert.ToInt32(BinaryNature).ToString();
            element.Element("LOGIC_CHECK").Element("Data").Value = Utility.GetLogicCheck(pdaver + "/" + cscver + "/" + phonever + "/" + dataver, Web.Nonce);
            return document.ToString();
        }

        public static string GetXmlBinaryInit(string file, string version, string region, string model_type)
        {
            XDocument document = XDocument.Parse(BinaryInit);
            XElement element = document.Element("FUSMsg").Element("FUSBody").Element("Put");
            element.Element("BINARY_FILE_NAME").Element("Data").Value = file;
            element.Element("BINARY_VERSION").Element("Data").Value = version;
            element.Element("DEVICE_LOCAL_CODE").Element("Data").Value = region;
            element.Element("DEVICE_MODEL_TYPE").Element("Data").Value = model_type;
            element.Element("LOGIC_CHECK").Element("Data").Value = Utility.GetLogicCheck(file, Web.Nonce);
            return document.ToString();
        }

        public static string GetXMLValue(string xml, string element, string attributename = null, string attributevalue = null)
        {
            XDocument document = XDocument.Parse(xml);
            string[] strArray = element.Split(new char[] { '/' });
            XElement root = document.Root;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (i < (strArray.Length - 1))
                {
                    root = root.Element(strArray[i]);
                }
                else
                {
                    foreach (XElement element3 in root.Elements(strArray[i]))
                    {
                        if (attributename == null)
                        {
                            root = element3;
                            break;
                        }
                        XAttribute attribute = element3.Attribute(attributename);
                        if ((attribute != null) && ((attributevalue == null) || (attribute.Value == attributevalue)))
                        {
                            root = element3;
                            break;
                        }
                    }
                }
            }
            return root.Value;
        }
    }
}

