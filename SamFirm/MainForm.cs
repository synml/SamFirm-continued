using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SamFirm
{
    internal partial class MainForm : Form
    {
        //기본 생성자
        public MainForm()
        {
            InitializeComponent();
        }

        //필드
        private string destinationFile;
        private Command.Firmware FW;
        public bool PauseDownload { get; set; }


        //폼을 로드하면 호출하는 메소드
        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.Form = this;
            Web.Form = this;
            Decrypt.Form = this;

            //각 컨트롤에 설정파일에서 불러온 값을 적용한다.
            this.model_textbox.Text = Settings.ReadSetting("Model");
            this.region_textbox.Text = Settings.ReadSetting("Region");
            if (Settings.ReadSetting("BinaryNature") == "True")
            {
                this.binary_checkbox.Checked = true;
            }
            if (Settings.ReadSetting("AutoDecrypt") == "False")
            {
                this.autoDecrypt_checkbox.Checked = false;
            }
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Logger.WriteLine("SamFirm v" + versionInfo.FileVersion);
            ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;
        }

        //폼을 닫으면 호출하는 메소드
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //설정값을 저장한다.
            Settings.SetSetting("Model", model_textbox.Text.ToUpper());
            Settings.SetSetting("Region", region_textbox.Text.ToUpper());
            Settings.SetSetting("BinaryNature", binary_checkbox.Checked.ToString());
            Settings.SetSetting("AutoDecrypt", autoDecrypt_checkbox.Checked.ToString());

            //다운로드 일시정지 필드를 true로 한다.
            this.PauseDownload = true;

            //모듈을 메모리에서 내리고, 로그를 파일로 저장한다.
            Imports.FreeModule();
            Logger.SaveLog();
        }

        //Update 버튼 클릭시 실행하는 메소드
        private void Update_button_Click(object sender, EventArgs e)
        {
            //예외 처리
            if (string.IsNullOrEmpty(model_textbox.Text))
            {
                Logger.WriteLine("Error: Please specify a model");
                return;
            }
            else if (string.IsNullOrEmpty(region_textbox.Text))
            {
                Logger.WriteLine("Error: Please specify a region");
                return;
            }

            //백그라운드 작업 등록
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate {
                try
                {
                    SetProgressBar(0);
                    ControlsEnabled(false);
                    Utility.ReconnectDownload = false;

                    //업데이트를 검사한다.
                    FW = Command.UpdateCheckAuto(model_textbox.Text, region_textbox.Text, binary_checkbox.Checked);

                    //FW 구조체의 Filename멤버가 비어있으면 빈 글자를 출력하고,
                    //비어있지 않으면 FW의 멤버를 출력한다.
                    if (string.IsNullOrEmpty(FW.Filename))
                    {
                        file_textbox.Invoke(new Action(() => file_textbox.Text = string.Empty));
                        version_textbox.Invoke(new Action(() => version_textbox.Text = string.Empty));
                        size_label.Invoke(new Action(() => size_label.Text = string.Empty));
                    }
                    else
                    {
                        file_textbox.Invoke(new Action(() => file_textbox.Text = FW.Filename));
                        version_textbox.Invoke(new Action(() => version_textbox.Text = FW.Version));
                        double size = Math.Round(long.Parse(FW.Size) / 1024.0 / 1024.0 / 1024.0, 3);
                        size_label.Invoke(new Action(() => size_label.Text = size.ToString() + " GB"));
                    }

                    //출력을 완료하면 컨트롤을 활성화한다.
                    ControlsEnabled(true);
                }
                catch (Exception exception)
                {
                    Logger.WriteLine("Error Update_button_Click() -> " + exception);
                }
            };
            worker.RunWorkerAsync();
        }

        //Download 버튼 클릭시 실행하는 메소드
        private void Download_button_Click(object sender, EventArgs e)
        {
            //다운로드를 일시정지한다.
            if (download_button.Text == "Pause")
            {
                Utility.TaskBarProgressState(true);
                PauseDownload = true;
                Utility.ReconnectDownload = false;
                download_button.Text = "Download";
                return;
            }

            //예외 처리
            if ((e.GetType() == typeof(DownloadEventArgs)) && ((DownloadEventArgs)e).isReconnect)
            {
                if (download_button.Text == "Pause" || !Utility.ReconnectDownload)
                {
                    return;
                }
            }
            if (PauseDownload)
            {
                Logger.WriteLine("Download thread is still running. Please wait.");
                return;
            }
            else if (string.IsNullOrEmpty(file_textbox.Text))
            {
                Logger.WriteLine("No file to download. Please check for update first.");
                return;
            }

            //다운로드 작업
            if ((e.GetType() != typeof(DownloadEventArgs)) || !((DownloadEventArgs)e).isReconnect)
            {
                //.zip + .enc4
                string extension = Path.GetExtension(Path.GetFileNameWithoutExtension(FW.Filename)) + Path.GetExtension(FW.Filename);
                saveFileDialog1.OverwritePrompt = false;
                saveFileDialog1.FileName = FW.Filename.Replace(extension, "");
                saveFileDialog1.Filter = "Firmware|*" + extension;
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    Logger.WriteLine("Download aborted.");
                    return;
                }
                if (!saveFileDialog1.FileName.EndsWith(extension))
                {
                    saveFileDialog1.FileName = saveFileDialog1.FileName + extension;
                }
                else
                {
                    saveFileDialog1.FileName = saveFileDialog1.FileName.Replace(extension + extension, extension);
                }
                Logger.WriteLine("Filename: " + saveFileDialog1.FileName);

                destinationFile = saveFileDialog1.FileName;
                if (File.Exists(destinationFile))
                {
                    switch (new AppendDialogBox().ShowDialog())
                    {
                        case DialogResult.Yes:
                            break;

                        case DialogResult.No:
                            File.Delete(destinationFile);
                            break;

                        case DialogResult.Cancel:
                            Logger.WriteLine("Download aborted.");
                            return;

                        default:
                            Logger.WriteLine("Error: Wrong DialogResult");
                            return;
                    }
                }
            }
            Utility.TaskBarProgressState(false);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate (object o, DoWorkEventArgs _e)
            {
                try
                {
                    ControlsEnabled(false);
                    Utility.ReconnectDownload = false;
                    MethodInvoker invoker1 = delegate
                    {
                        download_button.Enabled = true;
                        download_button.Text = "Pause";
                    };
                    download_button.Invoke(invoker1);
                    if (FW.Filename == destinationFile)
                    {
                        Logger.WriteLine("Download " + FW.Filename);
                    }
                    else
                    {
                        Logger.WriteLine("Download " + FW.Filename + " to " + destinationFile);
                    }
                    Command.Download(FW.Path, FW.Filename, FW.Version, FW.Region, FW.Model_Type, destinationFile, FW.Size, true);
                    if (PauseDownload == true)
                    {
                        Logger.WriteLine("Download paused.");
                        PauseDownload = false;
                        if (Utility.ReconnectDownload)
                        {
                            Logger.WriteLine("Reconnecting...");
                            Utility.Reconnect(new Action<object, EventArgs>(Download_button_Click));
                        }
                    }
                    else
                    {
                        Logger.WriteLine("Download finished.");
                        if (FW.CRC == null)
                        {
                            Logger.WriteLine("Error: Unable to check CRC. Value not set by Samsung");
                        }
                        else
                        {
                            Logger.WriteLine("\nChecking CRC32...");
                            if (!Utility.CRCCheck(destinationFile, FW.CRC))
                            {
                                Logger.WriteLine("Error: CRC does not match. Please redownload the file.");
                                File.Delete(destinationFile);
                                goto Label_01C9;
                            }
                            Logger.WriteLine("CRC matched.");
                        }
                        decrypt_button.Invoke(new Action(() => decrypt_button.Enabled = true));
                        if (autoDecrypt_checkbox.Checked)
                        {
                            Decrypt_button_Click(o, null);
                        }
                    }
                Label_01C9:
                    if (!Utility.ReconnectDownload)
                    {
                        ControlsEnabled(true);
                    }
                    download_button.Invoke(new Action(() => download_button.Text = "Download"));
                }
                catch (Exception exception)
                {
                    Logger.WriteLine("Error Download_button_Click() -> " + exception);
                }
            };
            worker.RunWorkerAsync();
        }

        //Decrypt 버튼 클릭시 실행하는 메소드
        private void Decrypt_button_Click(object sender, EventArgs e)
        {
            //목적경로에 파일이 없으면 실행안함.
            if (!File.Exists(destinationFile))
            {
                Logger.WriteLine("Error Decrypt_button_Click(): File " + destinationFile + " does not exist");
                return;
            }

            //백그라운드 작업 등록
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                Logger.WriteLine("\nDecrypting firmware...");
                ControlsEnabled(false);
                decrypt_button.Invoke(new Action(() => decrypt_button.Enabled = false));
                if (destinationFile.EndsWith(".enc2"))
                {
                    Decrypt.SetDecryptKey(FW.Region, FW.Model, FW.Version);
                }
                else if (destinationFile.EndsWith(".enc4"))
                {
                    if (FW.BinaryNature == 1)
                    {
                        Decrypt.SetDecryptKey(FW.Version, FW.LogicValueFactory);
                    }
                    else
                    {
                        Decrypt.SetDecryptKey(FW.Version, FW.LogicValueHome);
                    }
                }
                if (Decrypt.DecryptFile(destinationFile, Path.Combine(Path.GetDirectoryName(destinationFile), Path.GetFileNameWithoutExtension(destinationFile))) == 0)
                {
                    File.Delete(destinationFile);
                }
                Logger.WriteLine("Decryption finished.");
                ControlsEnabled(true);
            };
            worker.RunWorkerAsync();
        }

        //컨트롤 활성화/비활성화 설정 메소드
        private void ControlsEnabled(bool boolValue)
        {
            model_textbox.Invoke(new Action(() => model_textbox.Enabled = boolValue));
            region_textbox.Invoke(new Action(() => region_textbox.Enabled = boolValue));
            binary_checkbox.Invoke(new Action(() => binary_checkbox.Enabled = boolValue));
            update_button.Invoke(new Action(() => update_button.Enabled = boolValue));
            download_button.Invoke(new Action(() => download_button.Enabled = boolValue));
            decrypt_button.Invoke(new Action(() => decrypt_button.Enabled = boolValue));
            autoDecrypt_checkbox.Invoke(new Action(() => autoDecrypt_checkbox.Enabled = boolValue));
        }

        //작업 진행바를 설정하는 메소드
        public void SetProgressBar(int progress)
        {
            MethodInvoker invoker1 = delegate
            {
                if (progress > 100)
                {
                    this.progressBar1.Value = 100;
                }
                else
                {
                    this.progressBar1.Value = progress;
                }
                TaskbarManager.Instance.SetProgressValue(progress, 100);
            };
            this.progressBar1.Invoke(invoker1);
        }

        public class DownloadEventArgs : EventArgs
        {
            internal bool isReconnect;
        }
    }
}