using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SamFirm
{
    public class customMessageBox : Form
    {
        private Button button1;
        private Button button2;
        private Button button3;
        private Label lbltext;
        private PictureBox pictureBox1;

        public customMessageBox()
        { }

        public customMessageBox(string message, string button1txt, DialogResult result1, string button2txt, DialogResult result2, string button3txt, DialogResult result3, Image img)
        {
            this.InitializeComponent();
            this.lbltext.Text = message;
            this.button1.Text = button1txt;
            this.button1.DialogResult = result1;
            if (result1 == DialogResult.Cancel)
            {
                base.CancelButton = this.button1;
            }
            this.button2.Text = button2txt;
            this.button2.DialogResult = result2;
            if (result2 == DialogResult.Cancel)
            {
                base.CancelButton = this.button2;
            }
            this.button3.Text = button3txt;
            this.button3.DialogResult = result3;
            if (result3 == DialogResult.Cancel)
            {
                base.CancelButton = this.button3;
            }
            this.pictureBox1.Image = img;
            this.Font = SystemFonts.DefaultFont;
        }

        private void InitializeComponent()
        {
            this.button1 = new Button();
            this.button2 = new Button();
            this.button3 = new Button();
            this.lbltext = new Label();
            this.pictureBox1 = new PictureBox();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.button1.Location = new Point(100, 60);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x17);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button2.Location = new Point(0xc1, 60);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x4b, 0x17);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button3.Location = new Point(0x11c, 60);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x4b, 0x17);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.lbltext.AutoSize = true;
            this.lbltext.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.lbltext.Location = new Point(0x61, 12);
            this.lbltext.Name = "lbltext";
            this.lbltext.Size = new Size(0x1a, 15);
            this.lbltext.TabIndex = 3;
            this.lbltext.Text = "text";
            this.pictureBox1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.pictureBox1.Location = new Point(0x18, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x43, 0x3a);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x173, 0x5e);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.lbltext);
            base.Controls.Add(this.button3);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            this.MaximumSize = new Size(0x183, 0x85);
            this.MinimumSize = new Size(0x183, 0x85);
            base.Name = "customMessageBox";
            this.Text = "SamFirm";
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}