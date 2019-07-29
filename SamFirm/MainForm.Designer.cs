using System.Windows.Forms;

namespace SamFirm
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.size_label = new System.Windows.Forms.Label();
            this.update_button = new System.Windows.Forms.Button();
            this.binary_checkbox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.speed_label = new System.Windows.Forms.Label();
            this.region_textbox = new System.Windows.Forms.TextBox();
            this.model_textbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.decrypt_button = new System.Windows.Forms.Button();
            this.download_button = new System.Windows.Forms.Button();
            this.autoDecrypt_checkbox = new System.Windows.Forms.CheckBox();
            this.version_textbox = new System.Windows.Forms.TextBox();
            this.file_textbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.log_textbox = new System.Windows.Forms.RichTextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.size_label);
            this.groupBox1.Controls.Add(this.update_button);
            this.groupBox1.Controls.Add(this.binary_checkbox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.speed_label);
            this.groupBox1.Controls.Add(this.region_textbox);
            this.groupBox1.Controls.Add(this.model_textbox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // size_label
            // 
            resources.ApplyResources(this.size_label, "size_label");
            this.size_label.Name = "size_label";
            // 
            // update_button
            // 
            resources.ApplyResources(this.update_button, "update_button");
            this.update_button.Name = "update_button";
            this.update_button.UseVisualStyleBackColor = true;
            this.update_button.Click += new System.EventHandler(this.Update_button_Click);
            // 
            // binary_checkbox
            // 
            resources.ApplyResources(this.binary_checkbox, "binary_checkbox");
            this.binary_checkbox.Name = "binary_checkbox";
            this.binary_checkbox.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // speed_label
            // 
            resources.ApplyResources(this.speed_label, "speed_label");
            this.speed_label.Name = "speed_label";
            // 
            // region_textbox
            // 
            resources.ApplyResources(this.region_textbox, "region_textbox");
            this.region_textbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.region_textbox.Name = "region_textbox";
            // 
            // model_textbox
            // 
            resources.ApplyResources(this.model_textbox, "model_textbox");
            this.model_textbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.model_textbox.Name = "model_textbox";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.decrypt_button);
            this.groupBox2.Controls.Add(this.download_button);
            this.groupBox2.Controls.Add(this.autoDecrypt_checkbox);
            this.groupBox2.Controls.Add(this.version_textbox);
            this.groupBox2.Controls.Add(this.file_textbox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // decrypt_button
            // 
            resources.ApplyResources(this.decrypt_button, "decrypt_button");
            this.decrypt_button.Name = "decrypt_button";
            this.decrypt_button.UseVisualStyleBackColor = true;
            this.decrypt_button.Click += new System.EventHandler(this.Decrypt_button_Click);
            // 
            // download_button
            // 
            resources.ApplyResources(this.download_button, "download_button");
            this.download_button.Name = "download_button";
            this.download_button.UseVisualStyleBackColor = true;
            this.download_button.Click += new System.EventHandler(this.Download_button_Click);
            // 
            // autoDecrypt_checkbox
            // 
            resources.ApplyResources(this.autoDecrypt_checkbox, "autoDecrypt_checkbox");
            this.autoDecrypt_checkbox.Checked = true;
            this.autoDecrypt_checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoDecrypt_checkbox.Name = "autoDecrypt_checkbox";
            this.autoDecrypt_checkbox.UseVisualStyleBackColor = true;
            // 
            // version_textbox
            // 
            resources.ApplyResources(this.version_textbox, "version_textbox");
            this.version_textbox.Name = "version_textbox";
            this.version_textbox.ReadOnly = true;
            this.version_textbox.TabStop = false;
            // 
            // file_textbox
            // 
            resources.ApplyResources(this.file_textbox, "file_textbox");
            this.file_textbox.Name = "file_textbox";
            this.file_textbox.ReadOnly = true;
            this.file_textbox.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // log_textbox
            // 
            resources.ApplyResources(this.log_textbox, "log_textbox");
            this.log_textbox.Name = "log_textbox";
            this.log_textbox.ReadOnly = true;
            this.log_textbox.TabStop = false;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.AddExtension = false;
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            this.saveFileDialog1.OverwritePrompt = false;
            this.saveFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // MainForm
            // 
            this.AcceptButton = this.update_button;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.log_textbox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        //필드 (컨트롤)
        private System.Windows.Forms.Button update_button;
        private System.Windows.Forms.Button download_button;
        private System.Windows.Forms.Button decrypt_button;
        private System.Windows.Forms.CheckBox binary_checkbox;
        private System.Windows.Forms.CheckBox autoDecrypt_checkbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label size_label;
        private System.Windows.Forms.Label speed_label;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox log_textbox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox model_textbox;
        private System.Windows.Forms.TextBox region_textbox;
        private System.Windows.Forms.TextBox file_textbox;
        private System.Windows.Forms.TextBox version_textbox;

        //Getter, Setter 메소드
        public RichTextBox Log_textbox { get => log_textbox; set => log_textbox = value; }
        public Label Speed_label { get => speed_label; set => speed_label = value; }
    }
}