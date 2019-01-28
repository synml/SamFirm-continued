using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
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
        private string destinationFile;     //다운로드하는 파일의 경로와 이름을 저장한다.
        private Command.Firmware FW;        //다운로드할 펌웨어의 정보를 저장하는 구조체 객체.
        public bool PauseDownload { get; set; }     //다운로드가 일시정지 되었는지 여부를 저장한다.


        //폼을 로드하면 호출하는 메소드
        private void MainForm_Load(object sender, EventArgs e)
        {
            //각 클래스의 form필드에 이 클래스를 붙여준다.
            Logger.Form = this;
            Web.Form = this;
            Decrypt.Form = this;

            //각 컨트롤에 설정파일에서 불러온 값을 적용한다.
            model_textbox.Text = Settings.ReadSetting("Model");
            region_textbox.Text = Settings.ReadSetting("Region");
            binary_checkbox.Checked = bool.Parse(Settings.ReadSetting("BinaryNature"));
            autoDecrypt_checkbox.Checked = bool.Parse(Settings.ReadSetting("AutoDecrypt"));

            //버전정보를 출력한다.
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Logger.WriteLine("SamFirm v" + versionInfo.FileVersion);

            //서버 인증서의 유효성을 검사하는 콜백을 설정한다.
            ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;
        }

        //폼을 닫으면 호출하는 메소드
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //설정값을 저장한다.
            Settings.SetSetting("Model", model_textbox.Text);
            Settings.SetSetting("Region", region_textbox.Text);
            Settings.SetSetting("BinaryNature", binary_checkbox.Checked.ToString());
            Settings.SetSetting("AutoDecrypt", autoDecrypt_checkbox.Checked.ToString());

            //다운로드 일시정지 필드를 true로 한다.
            PauseDownload = true;

            //모듈을 메모리에서 내리고, 로그를 파일로 저장한다.
            Imports.FreeModule();
            Logger.SaveLog();
        }

        //Update 버튼 클릭시 실행하는 메소드
        private void Update_button_Click(object sender, EventArgs e)
        {
            //예외 처리
            if (string.IsNullOrEmpty(model_textbox.Text) || string.IsNullOrEmpty(region_textbox.Text))
            {
                Logger.WriteLine("Error: Please specify a model or a region.");
                return;
            }

            //백그라운드 작업 등록
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
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
                        size_label.Invoke(new Action(() => size_label.Text = size.ToString(CultureInfo.InvariantCulture) + " GB"));
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
                Utility.TaskBarProgressPaused(true);
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
            if (PauseDownload == true)
            {
                Logger.WriteLine("Download thread is still running. Please wait.");
                return;
            }
            else if (string.IsNullOrEmpty(FW.Filename))
            {
                Logger.WriteLine("No file to download. Please check for update first.");
                return;
            }

            //다운로드 작업
            if ((e.GetType() != typeof(DownloadEventArgs)) || !((DownloadEventArgs)e).isReconnect)
            {
                //extension = .zip.enc4
                string extension = Path.GetExtension(Path.GetFileNameWithoutExtension(FW.Filename)) + Path.GetExtension(FW.Filename);

                //파일이름의 기본값을 FW구조체의 파일이름으로 설정한다.
                saveFileDialog1.FileName = FW.Filename;

                //확인 버튼을 누르지 않으면 다운로드를 취소한다.
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    Logger.WriteLine("Download canceled.");
                    return;
                }

                //확장자가 .zip.enc4로 끝나지 않으면 따로 붙여준다.
                if (saveFileDialog1.FileName.EndsWith(extension) != true)
                {
                    destinationFile = saveFileDialog1.FileName + extension;
                }
                else
                {
                    destinationFile = saveFileDialog1.FileName;
                }

                //파일명을 출력한다.
                Logger.WriteLine("\nFilename: " + FW.Filename);

                //목적지에 파일이 존재하면 대화상자를 보여준다.
                if (File.Exists(destinationFile))
                {
                    switch (new AppendDialogBox().ShowDialog())
                    {
                        case DialogResult.Yes:  //append
                            break;

                        case DialogResult.No:   //overwrite
                            File.Delete(destinationFile);
                            break;

                        case DialogResult.Cancel:
                            Logger.WriteLine("Download canceled.");
                            return;

                        default:
                            Logger.WriteLine("Error: Wrong DialogResult");
                            return;
                    }
                }
            }

            //백그라운드 작업 등록
            Utility.TaskBarProgressPaused(false);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate (object o, DoWorkEventArgs _e)
            {
                try
                {
                    ControlsEnabled(false);
                    Utility.ReconnectDownload = false;

                    //다운로드 버튼을 일시정지 버튼으로 바꾼다.
                    MethodInvoker invoker1 = delegate
                    {
                        download_button.Enabled = true;
                        download_button.Text = "Pause";
                    };
                    download_button.Invoke(invoker1);

                    //다운로드 경로와 파일명을 출력한다.
                    Logger.WriteLine("Download: " + destinationFile);

                    //펌웨어 다운로드를 시작한다.
                    Command.Download(FW.Path, FW.Filename, FW.Version, FW.Region, FW.Model_Type, destinationFile, FW.Size);
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

                        //자동 복호화가 체크되어 있으면 복호화 버튼 클릭 이벤트를 호출한다.
                        if (autoDecrypt_checkbox.Checked)
                        {
                            decrypt_button.Invoke(new Action(() => decrypt_button.Enabled = true));
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
            //목적경로에 파일이 없으면 파일열기 대화상자를 보여준다.
            if (!File.Exists(destinationFile))
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    Logger.WriteLine("Decrypt canceled.");
                    return;
                }
                destinationFile = openFileDialog1.FileName;
            }

            //백그라운드 작업 등록
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                ControlsEnabled(false);
                Logger.WriteLine("\nDecrypting firmware...");

                //복호화 키를 설정한다.
                if (destinationFile.EndsWith(".enc4") != true)
                {
                    Logger.WriteLine("Error: Wrong extension.");
                    ControlsEnabled(true);
                    return;
                }
                if (FW.BinaryNature == 1)
                {
                    Decrypt.SetDecryptKey(FW.Version, FW.LogicValueFactory);
                }
                else
                {
                    Decrypt.SetDecryptKey(FW.Version, FW.LogicValueHome);
                }

                //복호화를 실행한다.
                Decrypt.DecryptFile(destinationFile, Path.Combine(Path.GetDirectoryName(destinationFile), Path.GetFileNameWithoutExtension(destinationFile)));
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
            //폼의 진행바 컨트롤의 값을 변경한다.
            progressBar1.Invoke(new Action(() => progressBar1.Value = progress > 100 ? 100 : progress));

            //작업 표시줄의 진행바의 값을 변경한다.
            TaskbarManager.Instance.SetProgressValue(progress, 100);
        }

        public class DownloadEventArgs : EventArgs
        {
            internal bool isReconnect;
        }
    }
}