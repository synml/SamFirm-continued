using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SamFirm
{
    internal static class Decrypt
    {
        public static MainForm Form { get; set; }
        private static readonly byte[] IV = new byte[1];
        private static byte[] KEY;

        public static int DecryptFile(string encryptedFile, string outputFile)
        {
            using (FileStream stream1 = new FileStream(encryptedFile, FileMode.Open))
            {
                using (FileStream stream2 = new FileStream(outputFile, FileMode.Create))
                {
                    using (RijndaelManaged managed = new RijndaelManaged())
                    {
                        managed.Mode = CipherMode.ECB;
                        managed.BlockSize = 0x80;
                        managed.Padding = PaddingMode.PKCS7;

                        using (ICryptoTransform transform = managed.CreateDecryptor(KEY, IV))
                        {
                            try
                            {
                                Utility.PreventDeepSleep(Utility.PDSMode.Start);
                                using (CryptoStream stream3 = new CryptoStream(stream1, transform, CryptoStreamMode.Read))
                                {
                                    byte[] buffer = new byte[0x1000];
                                    long num = 0L;
                                    int count = 0;
                                    do
                                    {
                                        Utility.PreventDeepSleep(Utility.PDSMode.Continue);
                                        count = stream3.Read(buffer, 0, buffer.Length);
                                        num += count;
                                        stream2.Write(buffer, 0, count);
                                        Form.SetProgressBar(Utility.GetProgress(num, stream1.Length));
                                    }
                                    while (count > 0);
                                }
                            }
                            catch (CryptographicException)
                            {
                                Logger.WriteLine("Error DecryptFile(): Wrong key.");
                                return 2;
                            }
                            catch (Exception e)
                            {
                                Logger.WriteLine("Error DecryptFile() -> " + e);
                                return 1;
                            }
                            finally
                            {
                                Utility.PreventDeepSleep(Utility.PDSMode.Stop);
                            }
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