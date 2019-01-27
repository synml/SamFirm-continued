using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SamFirm
{
    public partial class MainForm2 : Form
    {
        //기본 생성자
        public MainForm2()
        {
            InitializeComponent();
        }

        //필드
        private string destinationfile;
        private Command.Firmware FW;
        public bool PauseDownload { get; set; }


        //폼을 로드하면 호출하는 메소드
        private void MainForm2_Load(object sender, EventArgs e)
        {
            //각 필드의 클래스 변경 필요함!
            //Logger.Form = this;
            //Web.Form = this;
            //Decrypt.Form = this;

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
        private void MainForm2_FormClosing(object sender, FormClosingEventArgs e)
        {
            //설정값을 저장한다.
            Settings.SetSetting("Model", model_textbox.Text.ToUpper());
            Settings.SetSetting("Region", region_textbox.Text.ToUpper());
            Settings.SetSetting("BinaryNature", binary_checkbox.Checked.ToString());
            Settings.SetSetting("AutoDecrypt", autoDecrypt_checkbox.Checked.ToString());

            //다운로드 일시정지 필드를 true로 한다.
            this.PauseDownload = true;

            //모듈을 메모리에서 내리고, 로그를 파일로 저장한다.
            Thread.Sleep(100);
            Imports.FreeModule();
            Logger.SaveLog();
        }

        //Update 버튼 클릭시 실행하는 메소드
        private void update_button_Click(object sender, EventArgs e)
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

                    //업데이트 검사
                    FW = Command.UpdateCheckAuto(model_textbox.Text, region_textbox.Text, binary_checkbox.Checked);

                    //FW 구조체의 Filename멤버가 비어있지 않으면 FW의 멤버를 출력하고,
                    //비어있으면 빈 글자를 출력한다.
                    if (!string.IsNullOrEmpty(FW.Filename))
                    {
                        file_textbox.Invoke(new Action(() => file_textbox.Text = FW.Filename));
                        version_textbox.Invoke(new Action(() => version_textbox.Text = FW.Version));
                        size_label.Invoke(new Action(() => size_label.Text = (long.Parse(FW.Size) / 1024L / 1024L) + " MB"));
                    }
                    else
                    {
                        file_textbox.Invoke(new Action(() => file_textbox.Text = string.Empty));
                        version_textbox.Invoke(new Action(() => version_textbox.Text = string.Empty));
                        size_label.Invoke(new Action(() => size_label.Text = string.Empty));
                    }

                    //출력을 완료하면 컨트롤을 활성화한다.
                    ControlsEnabled(true);
                }
                catch (Exception exception)
                {
                    Logger.WriteLine(exception.Message);
                    Logger.WriteLine(exception.ToString());
                }
            };
            worker.RunWorkerAsync();
        }

        //Download 버튼 클릭시 실행하는 메소드
        private void download_button_Click(object sender, EventArgs e)
        {

        }

        //Decrypt 버튼 클릭시 실행하는 메소드
        private void decrypt_button_Click(object sender, EventArgs e)
        {

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