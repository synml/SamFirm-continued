namespace SamFirm
{
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;

    internal class Crypto
    {
        public static Form1 form;
        private static readonly byte[] IV = new byte[1];
        private static byte[] KEY;

        public static int Decrypt(string encryptedFile, string outputFile, bool GUI = true)
        {
            using (FileStream stream = new FileStream(encryptedFile, FileMode.Open))
            {
                using (FileStream stream2 = new FileStream(outputFile, FileMode.Create))
                {
                    RijndaelManaged managed = new RijndaelManaged {
                        Mode = CipherMode.ECB,
                        BlockSize = 0x80,
                        Padding = PaddingMode.PKCS7
                    };
                    using (ICryptoTransform transform = managed.CreateDecryptor(KEY, IV))
                    {
                        try
                        {
                            Utility.PreventDeepSleep(Utility.PDSMode.Start);
                            using (CryptoStream stream3 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                            {
                                byte[] buffer = new byte[0x1000];
                                long num = 0L;
                                int count = 0;
                                do
                                {
                                    Utility.PreventDeepSleep(Utility.PDSMode.Continue);
                                    num += count = stream3.Read(buffer, 0, buffer.Length);
                                    stream2.Write(buffer, 0, count);
                                    if (GUI)
                                    {
                                        form.SetProgressBar(Utility.GetProgress(num, stream.Length));
                                    }
                                    else
                                    {
                                        CmdLine.SetProgress(Utility.GetProgress(num, stream.Length));
                                    }
                                }
                                while (count > 0);
                            }
                        }
                        catch (CryptographicException)
                        {
                            Logger.WriteLog("Error decrypting file: Wrong key.", false);
                            return 3;
                        }
                        catch (TargetInvocationException)
                        {
                            Logger.WriteLog("Error decrypting file: Please turn off FIPS compliance checking.", false);
                            return 800;
                        }
                        catch (IOException exception)
                        {
                            Logger.WriteLog("Error decrypting file: IOException: " + exception.Message, false);
                            return 3;
                        }
                        finally
                        {
                            Utility.PreventDeepSleep(Utility.PDSMode.Stop);
                        }
                    }
                }
            }
            return 0;
        }

        public static void SetDecryptKey(string version, string LogicValue)
        {
            string logicCheck = Utility.GetLogicCheck(version, LogicValue);
            byte[] bytes = Encoding.ASCII.GetBytes(logicCheck);
            using (MD5 md = MD5.Create())
            {
                KEY = md.ComputeHash(bytes);
            }
        }

        public static void SetDecryptKey(string region, string model, string version)
        {
            StringBuilder builder = new StringBuilder(region);
            builder.Append(':');
            builder.Append(model);
            builder.Append(':');
            builder.Append(version);
            byte[] bytes = Encoding.ASCII.GetBytes(builder.ToString());
            using (MD5 md = MD5.Create())
            {
                KEY = md.ComputeHash(bytes);
            }
        }
    }
}

