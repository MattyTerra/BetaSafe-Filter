namespace BetaSafeFilter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            UploadButton = new Button();
            CensorImg = new PictureBox();
            CensorButton = new Button();
            checkedListBox1 = new CheckedListBox();
            FileNameLabel = new Label();
            VidButton = new Button();
            CensorVid = new Button();
            ((System.ComponentModel.ISupportInitialize)CensorImg).BeginInit();
            SuspendLayout();
            // 
            // UploadButton
            // 
            UploadButton.Location = new Point(46, 108);
            UploadButton.Name = "UploadButton";
            UploadButton.Size = new Size(213, 44);
            UploadButton.TabIndex = 0;
            UploadButton.Text = "Click This Button to Upload Picture";
            UploadButton.UseVisualStyleBackColor = true;
            UploadButton.Click += UploadButton_Click;
            // 
            // CensorImg
            // 
            CensorImg.Location = new Point(383, 30);
            CensorImg.Name = "CensorImg";
            CensorImg.Size = new Size(640, 480);
            CensorImg.SizeMode = PictureBoxSizeMode.Zoom;
            CensorImg.TabIndex = 1;
            CensorImg.TabStop = false;
            // 
            // CensorButton
            // 
            CensorButton.Location = new Point(41, 242);
            CensorButton.Name = "CensorButton";
            CensorButton.Size = new Size(218, 50);
            CensorButton.TabIndex = 2;
            CensorButton.Text = "Click This Button to Censor";
            CensorButton.UseVisualStyleBackColor = true;
            CensorButton.Click += CensorButton_Click;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "\"Female Breasts\"", "\"Female Privates\"" });
            checkedListBox1.Location = new Point(41, 381);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(156, 40);
            checkedListBox1.TabIndex = 3;
            checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            // 
            // FileNameLabel
            // 
            FileNameLabel.AutoSize = true;
            FileNameLabel.Location = new Point(46, 153);
            FileNameLabel.Name = "FileNameLabel";
            FileNameLabel.Size = new Size(38, 15);
            FileNameLabel.TabIndex = 4;
            FileNameLabel.Text = "label1";
            // 
            // VidButton
            // 
            VidButton.Location = new Point(46, 437);
            VidButton.Name = "VidButton";
            VidButton.Size = new Size(213, 41);
            VidButton.TabIndex = 5;
            VidButton.Text = "UpLoad Video";
            VidButton.UseVisualStyleBackColor = true;
            VidButton.Click += VidButton_Click;
            // 
            // CensorVid
            // 
            CensorVid.Location = new Point(37, 489);
            CensorVid.Name = "CensorVid";
            CensorVid.Size = new Size(259, 44);
            CensorVid.TabIndex = 6;
            CensorVid.Text = "Export Loser Video";
            CensorVid.UseVisualStyleBackColor = true;
            CensorVid.Click += CensorVid_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 555);
            Controls.Add(CensorVid);
            Controls.Add(VidButton);
            Controls.Add(FileNameLabel);
            Controls.Add(checkedListBox1);
            Controls.Add(CensorButton);
            Controls.Add(CensorImg);
            Controls.Add(UploadButton);
            Name = "Form1";
            Text = "BetaSafeFilter";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)CensorImg).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button UploadButton;
        private PictureBox CensorImg;
        private Button CensorButton;
        private CheckedListBox checkedListBox1;
        private Label FileNameLabel;
        private Button VidButton;
        private Button CensorVid;
    }
}
