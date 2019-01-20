namespace SamFirm
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class CmdLine
    {
        private static bool autodecrypt = false;
        private static bool binary = false;
        private static bool checkonly = false;
        private static string file = string.Empty;
        private static string folder = string.Empty;
        private static string fwdest = string.Empty;
        private static string logicValue = string.Empty;
        private static string metafile = string.Empty;
        private static string model = string.Empty;
        public static ProgressBarInfo progressBar = new ProgressBarInfo();
        private static string region = string.Empty;
        private static string version = string.Empty;

        private static void CreateProgressbar()
        {
            try
            {
                Console.CursorTop = Console.CursorTop;
                char[] spaceArray = Utility.GetSpaceArray(Console.BufferWidth);
                spaceArray[0] = '[';
                spaceArray[spaceArray.Length - 1] = ']';
                progressBar.Line = Console.CursorTop;
                Logger.WriteLog(new string(spaceArray), true);
            }
            catch (IOException)
            {
                progressBar.Line = -1;
            }
        }

        private static void DisplayUsage()
        {
            Logger.WriteLog("Usage:\n", false);
            Logger.WriteLog("Update check:", false);
            Logger.WriteLog("     SamFirm.exe -c -model [device model] -region [region code]\n                [-version [pda/csc/phone/data]] [-binary]", false);
            Logger.WriteLog("\nDecrypting:", false);
            Logger.WriteLog("     SamFirm.exe -file [path-to-file.zip.enc2] -version [pda/csc/phone/data]", false);
            Logger.WriteLog("     SamFirm.exe -file [path-to-file.zip.enc4] -version [pda/csc/phone/data] -logicValue [logicValue]", false);
            Logger.WriteLog("\nDownloading:", false);
            Logger.WriteLog("     SamFirm.exe -model [device model] -region [region code]\n                [-version [pda/csc/phone/data]] [-folder [output folder]]\n                [-binary] [-autodecrypt]", false);
        }

        private static int DoCheck()
        {
            Command.Firmware firmware;
            Logger.WriteLog("========== SamFirm Firmware Update Check ==========\n", false);
            if (string.IsNullOrEmpty(version))
            {
                firmware = Command.UpdateCheckAuto(model, region, binary);
                if (firmware.FetchAttempts == 0)
                {
                    return 5;
                }
            }
            else
            {
                firmware = Command.UpdateCheck(model, region, version, binary, false);
            }
            if (firmware.Version == null)
            {
                return 2;
            }
            return 0;
        }

        private static int DoDecrypt()
        {
            Logger.WriteLog("========== SamFirm Firmware Decrypter ==========\n", false);
            Logger.WriteLog("Decrypting file " + file + "...", false);
            CreateProgressbar();
            if (file.EndsWith(".enc2"))
            {
                Crypto.SetDecryptKey(region, model, version);
            }
            else if (file.EndsWith(".enc4"))
            {
                Crypto.SetDecryptKey(version, logicValue);
            }
            if (Crypto.Decrypt(file, Path.GetFileNameWithoutExtension(file), false) != 0)
            {
                Logger.WriteLog("\nError decrypting file", false);
                Logger.WriteLog("Please make sure the filename is not modified and verify the version / logicValue argument", false);
                File.Delete(Path.GetFileNameWithoutExtension(file));
                return 3;
            }
            Logger.WriteLog("\nDecrypting successful", false);
            return 0;
        }

        private static int DoDownload()
        {
            Command.Firmware firmware;
            int num;
            Logger.WriteLog("========== SamFirm Firmware Downloader ==========\n", false);
            if (string.IsNullOrEmpty(version))
            {
                firmware = Command.UpdateCheckAuto(model, region, binary);
                if (firmware.FetchAttempts == 0)
                {
                    return 5;
                }
            }
            else
            {
                firmware = Command.UpdateCheck(model, region, version, binary, false);
            }
            if (firmware.Version == null)
            {
                return 2;
            }
            string saveTo = Path.Combine(folder, firmware.Filename);
            Logger.WriteLog("Downloading...\n", false);
            CreateProgressbar();
            do
            {
                Utility.ReconnectCmdLine();
                Utility.ReconnectDownload = false;
                num = Command.Download(firmware.Path, firmware.Filename, firmware.Version, firmware.Region, firmware.Model_Type, saveTo, firmware.Size, false);
            }
            while (Utility.ReconnectDownload);
            if ((num != 200) && (num != 0xce))
            {
                Logger.WriteLog("Error: " + num, false);
                return 4;
            }
            if (autodecrypt)
            {
                if (saveTo.EndsWith(".enc2"))
                {
                    Crypto.SetDecryptKey(firmware.Region, firmware.Model, firmware.Version);
                }
                else if (saveTo.EndsWith(".enc4"))
                {
                    if (firmware.BinaryNature == 1)
                    {
                        Crypto.SetDecryptKey(firmware.Version, firmware.LogicValueFactory);
                    }
                    else
                    {
                        Crypto.SetDecryptKey(firmware.Version, firmware.LogicValueHome);
                    }
                }
                Logger.WriteLog("\nDecrypting...\n", false);
                CreateProgressbar();
                fwdest = Path.Combine(Path.GetDirectoryName(saveTo), Path.GetFileNameWithoutExtension(firmware.Filename));
                int num2 = Crypto.Decrypt(saveTo, fwdest, false);
                File.Delete(saveTo);
                if (num2 != 0)
                {
                    File.Delete(fwdest);
                    return 3;
                }
            }
            if (!string.IsNullOrEmpty(metafile))
            {
                SaveMeta(firmware);
            }
            Logger.WriteLog("\nFinished", false);
            return 0;
        }

        private static bool InputValidation(string[] args)
        {
            if (!ParseArgs(args))
            {
                Logger.WriteLog("Error parsing arguments\n", false);
                return false;
            }
            if (!string.IsNullOrEmpty(file) && !File.Exists(file))
            {
                Logger.WriteLog("File " + file + " does not exist\n", false);
                return false;
            }
            if (!string.IsNullOrEmpty(file) && !ParseFileName())
            {
                Logger.WriteLog("Could not parse filename. Make sure the filename was not edited\n", false);
                return false;
            }
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Logger.WriteLog("Folder " + folder + " does not exist\n", false);
                return false;
            }
            return true;
        }

        public static int Main(string[] args)
        {
            Thread.Sleep(200);
            if (!InputValidation(args))
            {
                DisplayUsage();
                return 1;
            }
            return ProcessAction();
        }

        private static bool ParseArgs(string[] args)
        {
            if ((args.Length < 4) || (args.Length > 12))
            {
                Logger.WriteLog("Error: Not enough / too many arguments", false);
                return false;
            }
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i++])
                {
                    case "-file":
                        file = args[i];
                        break;

                    case "-version":
                        version = args[i];
                        break;

                    case "-logicValue":
                        logicValue = args[i];
                        break;

                    case "-folder":
                        folder = args[i];
                        break;

                    case "-region":
                        region = args[i].ToUpper();
                        break;

                    case "-model":
                        model = args[i];
                        break;

                    case "-binary":
                        i--;
                        binary = true;
                        break;

                    case "-autodecrypt":
                        i--;
                        autodecrypt = true;
                        break;

                    case "-c":
                        i--;
                        checkonly = true;
                        break;

                    case "-meta":
                        metafile = args[i];
                        break;

                    default:
                        return false;
                }
            }
            return true;
        }

        private static bool ParseFileName()
        {
            if (!file.EndsWith(".enc4"))
            {
                string[] strArray = file.Split(new char[] { '_' });
                if (strArray.Length < 2)
                {
                    return false;
                }
                model = strArray[0];
                if (strArray[1].Length != 3)
                {
                    return false;
                }
                region = strArray[1];
            }
            return true;
        }

        private static int ProcessAction()
        {
            int num = -1;
            if (!string.IsNullOrEmpty(file))
            {
                if ((!string.IsNullOrEmpty(region) && !string.IsNullOrEmpty(model)) && !string.IsNullOrEmpty(version))
                {
                    num = DoDecrypt();
                }
                else if (!string.IsNullOrEmpty(logicValue) && !string.IsNullOrEmpty(version))
                {
                    num = DoDecrypt();
                }
            }
            else if (!string.IsNullOrEmpty(model) && !string.IsNullOrEmpty(region))
            {
                if (checkonly)
                {
                    num = DoCheck();
                }
                else
                {
                    num = DoDownload();
                }
            }
            if (num == -1)
            {
                DisplayUsage();
                num = 1;
            }
            return num;
        }

        private static void SaveMeta(Command.Firmware fw)
        {
            if ((fw.Version != null) && !string.IsNullOrEmpty(fwdest))
            {
                if (!string.IsNullOrEmpty(Path.GetDirectoryName(metafile)) && !Directory.Exists(Path.GetDirectoryName(metafile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(metafile));
                }
                using (TextWriter writer = File.CreateText(metafile))
                {
                    writer.WriteLine("[SamFirmData]");
                    writer.WriteLine("Model=" + fw.Model);
                    writer.WriteLine("Devicename=" + fw.DisplayName);
                    writer.WriteLine("Region=" + fw.Region);
                    writer.WriteLine("Version=" + fw.Version);
                    writer.WriteLine("OS=" + fw.OS);
                    writer.WriteLine("Filesize=" + new FileInfo(fwdest).Length);
                    writer.WriteLine("Filename=" + fwdest);
                    writer.WriteLine("LastModified=" + fw.LastModified);
                }
            }
        }

        public static void SetProgress(int value)
        {
            if (progressBar.Line != -1)
            {
                progressBar.oldLine = Console.CursorTop;
                int num = Console.BufferWidth - 2;
                int size = (int) ((num * value) / 100f);
                if (progressBar.Progress != size)
                {
                    Console.CursorTop = progressBar.Line;
                    Console.CursorLeft = 1;
                    Logger.WriteLog(new string(Utility.GetCharArray(size, '=')), true);
                    progressBar.Progress = size;
                }
                Console.CursorTop = progressBar.oldLine;
                Console.CursorLeft = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ProgressBarInfo
        {
            public int Progress;
            public int Line;
            public int oldLine;
        }
    }
}

