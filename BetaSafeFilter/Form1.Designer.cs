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
            FileNameLabel = new Label();
            VidButton = new Button();
            CensorVid = new Button();
            ImageTab = new TabControl();
            MainPage = new TabPage();
            SettingsPage = new TabPage();
            PixelLabel = new Label();
            PixelationDensity = new TrackBar();
            groupBox1 = new GroupBox();
            PixelateButton = new RadioButton();
            CensorBoxButton = new RadioButton();
            BlurButton = new RadioButton();
            CensorsChecklist = new CheckedListBox();
            GaussianBlurLabel = new Label();
            GaussianBlurrSlider = new TrackBar();
            CensorBoxColor = new ColorDialog();
            BoxColorButton = new Button();
            ((System.ComponentModel.ISupportInitialize)CensorImg).BeginInit();
            ImageTab.SuspendLayout();
            MainPage.SuspendLayout();
            SettingsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PixelationDensity).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GaussianBlurrSlider).BeginInit();
            SuspendLayout();
            // 
            // UploadButton
            // 
            UploadButton.Location = new Point(56, 46);
            UploadButton.Name = "UploadButton";
            UploadButton.Size = new Size(213, 44);
            UploadButton.TabIndex = 0;
            UploadButton.Text = "Click This Button to Upload Picture";
            UploadButton.UseVisualStyleBackColor = true;
            UploadButton.Click += UploadButton_Click;
            // 
            // CensorImg
            // 
            CensorImg.Location = new Point(380, 26);
            CensorImg.Name = "CensorImg";
            CensorImg.Size = new Size(640, 480);
            CensorImg.SizeMode = PictureBoxSizeMode.Zoom;
            CensorImg.TabIndex = 1;
            CensorImg.TabStop = false;
            // 
            // CensorButton
            // 
            CensorButton.Location = new Point(51, 129);
            CensorButton.Name = "CensorButton";
            CensorButton.Size = new Size(218, 50);
            CensorButton.TabIndex = 2;
            CensorButton.Text = "Click This Button to Censor Image";
            CensorButton.UseVisualStyleBackColor = true;
            CensorButton.Click += CensorButton_Click;
            // 
            // FileNameLabel
            // 
            FileNameLabel.AutoSize = true;
            FileNameLabel.Location = new Point(56, 91);
            FileNameLabel.Name = "FileNameLabel";
            FileNameLabel.Size = new Size(38, 15);
            FileNameLabel.TabIndex = 4;
            FileNameLabel.Text = "label1";
            // 
            // VidButton
            // 
            VidButton.Location = new Point(56, 217);
            VidButton.Name = "VidButton";
            VidButton.Size = new Size(213, 41);
            VidButton.TabIndex = 5;
            VidButton.Text = "UpLoad Video";
            VidButton.UseVisualStyleBackColor = true;
            VidButton.Click += VidButton_Click;
            // 
            // CensorVid
            // 
            CensorVid.Location = new Point(56, 301);
            CensorVid.Name = "CensorVid";
            CensorVid.Size = new Size(213, 44);
            CensorVid.TabIndex = 6;
            CensorVid.Text = "Export Loser Video";
            CensorVid.UseVisualStyleBackColor = true;
            CensorVid.Click += CensorVid_Click;
            // 
            // ImageTab
            // 
            ImageTab.Controls.Add(MainPage);
            ImageTab.Controls.Add(SettingsPage);
            ImageTab.Location = new Point(0, 1);
            ImageTab.Name = "ImageTab";
            ImageTab.SelectedIndex = 0;
            ImageTab.Size = new Size(1071, 560);
            ImageTab.TabIndex = 7;
            ImageTab.SelectedIndexChanged += ImageTab_SelectedIndexChanged;
            // 
            // MainPage
            // 
            MainPage.Controls.Add(UploadButton);
            MainPage.Controls.Add(CensorImg);
            MainPage.Controls.Add(CensorVid);
            MainPage.Controls.Add(CensorButton);
            MainPage.Controls.Add(VidButton);
            MainPage.Controls.Add(FileNameLabel);
            MainPage.Location = new Point(4, 24);
            MainPage.Name = "MainPage";
            MainPage.Padding = new Padding(3);
            MainPage.Size = new Size(1063, 532);
            MainPage.TabIndex = 0;
            MainPage.Text = "Image Censor";
            MainPage.UseVisualStyleBackColor = true;
            // 
            // SettingsPage
            // 
            SettingsPage.Controls.Add(BoxColorButton);
            SettingsPage.Controls.Add(GaussianBlurLabel);
            SettingsPage.Controls.Add(GaussianBlurrSlider);
            SettingsPage.Controls.Add(PixelLabel);
            SettingsPage.Controls.Add(PixelationDensity);
            SettingsPage.Controls.Add(groupBox1);
            SettingsPage.Controls.Add(CensorsChecklist);
            SettingsPage.Location = new Point(4, 24);
            SettingsPage.Name = "SettingsPage";
            SettingsPage.Padding = new Padding(3);
            SettingsPage.Size = new Size(1063, 532);
            SettingsPage.TabIndex = 1;
            SettingsPage.Text = "Settings";
            SettingsPage.UseVisualStyleBackColor = true;
            // 
            // PixelLabel
            // 
            PixelLabel.AutoSize = true;
            PixelLabel.Location = new Point(433, 130);
            PixelLabel.Name = "PixelLabel";
            PixelLabel.Size = new Size(38, 15);
            PixelLabel.TabIndex = 11;
            PixelLabel.Text = "label1";
            // 
            // PixelationDensity
            // 
            PixelationDensity.Location = new Point(238, 129);
            PixelationDensity.Maximum = 100;
            PixelationDensity.Name = "PixelationDensity";
            PixelationDensity.Size = new Size(189, 45);
            PixelationDensity.TabIndex = 10;
            PixelationDensity.TabStop = false;
            PixelationDensity.TickStyle = TickStyle.None;
            PixelationDensity.Value = 30;
            PixelationDensity.Scroll += PixelationDensity_Scroll;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(PixelateButton);
            groupBox1.Controls.Add(CensorBoxButton);
            groupBox1.Controls.Add(BlurButton);
            groupBox1.Location = new Point(25, 104);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(200, 197);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // PixelateButton
            // 
            PixelateButton.AutoSize = true;
            PixelateButton.Checked = true;
            PixelateButton.Location = new Point(6, 35);
            PixelateButton.Name = "PixelateButton";
            PixelateButton.Size = new Size(65, 19);
            PixelateButton.TabIndex = 6;
            PixelateButton.TabStop = true;
            PixelateButton.Text = "Pixelate";
            PixelateButton.UseVisualStyleBackColor = true;
            // 
            // CensorBoxButton
            // 
            CensorBoxButton.AutoSize = true;
            CensorBoxButton.Location = new Point(6, 133);
            CensorBoxButton.Name = "CensorBoxButton";
            CensorBoxButton.Size = new Size(55, 19);
            CensorBoxButton.TabIndex = 8;
            CensorBoxButton.Text = "Boxes";
            CensorBoxButton.UseVisualStyleBackColor = true;
            CensorBoxButton.CheckedChanged += CensorBoxButton_CheckedChanged;
            // 
            // BlurButton
            // 
            BlurButton.AutoSize = true;
            BlurButton.Location = new Point(0, 81);
            BlurButton.Name = "BlurButton";
            BlurButton.Size = new Size(96, 19);
            BlurButton.TabIndex = 7;
            BlurButton.Text = "Gaussian Blur";
            BlurButton.UseVisualStyleBackColor = true;
            BlurButton.CheckedChanged += BlurButton_CheckedChanged;
            // 
            // CensorsChecklist
            // 
            CensorsChecklist.CheckOnClick = true;
            CensorsChecklist.FormattingEnabled = true;
            CensorsChecklist.Items.AddRange(new object[] { "Nudes", "Non-Nude" });
            CensorsChecklist.Location = new Point(25, 40);
            CensorsChecklist.Name = "CensorsChecklist";
            CensorsChecklist.Size = new Size(156, 40);
            CensorsChecklist.TabIndex = 4;
            CensorsChecklist.SelectedIndexChanged += CensorsChecklist_SelectedIndexChanged;
            // 
            // GaussianBlurLabel
            // 
            GaussianBlurLabel.AutoSize = true;
            GaussianBlurLabel.Location = new Point(433, 181);
            GaussianBlurLabel.Name = "GaussianBlurLabel";
            GaussianBlurLabel.Size = new Size(38, 15);
            GaussianBlurLabel.TabIndex = 13;
            GaussianBlurLabel.Text = "label1";
            // 
            // GaussianBlurrSlider
            // 
            GaussianBlurrSlider.Location = new Point(238, 180);
            GaussianBlurrSlider.Maximum = 100;
            GaussianBlurrSlider.Name = "GaussianBlurrSlider";
            GaussianBlurrSlider.Size = new Size(189, 45);
            GaussianBlurrSlider.TabIndex = 12;
            GaussianBlurrSlider.TabStop = false;
            GaussianBlurrSlider.TickStyle = TickStyle.None;
            GaussianBlurrSlider.Value = 30;
            // 
            // BoxColorButton
            // 
            BoxColorButton.Location = new Point(231, 237);
            BoxColorButton.Name = "BoxColorButton";
            BoxColorButton.Size = new Size(200, 28);
            BoxColorButton.TabIndex = 14;
            BoxColorButton.Text = "Click for censor color select";
            BoxColorButton.UseVisualStyleBackColor = true;
            BoxColorButton.Click += BoxColorButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 555);
            Controls.Add(ImageTab);
            Name = "Form1";
            Text = "BetaSafeFilter";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)CensorImg).EndInit();
            ImageTab.ResumeLayout(false);
            MainPage.ResumeLayout(false);
            MainPage.PerformLayout();
            SettingsPage.ResumeLayout(false);
            SettingsPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PixelationDensity).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)GaussianBlurrSlider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button UploadButton;
        private PictureBox CensorImg;
        private Button CensorButton;
        private Label FileNameLabel;
        private Button VidButton;
        private Button CensorVid;
        private TabControl ImageTab;
        private TabPage MainPage;
        private TabPage SettingsPage;
        private CheckedListBox CensorsChecklist;
        private RadioButton PixelateButton;
        private RadioButton CensorBoxButton;
        private RadioButton BlurButton;
        private GroupBox groupBox1;
        private TrackBar PixelationDensity;
        private Label PixelLabel;
        private Label GaussianBlurLabel;
        private TrackBar GaussianBlurrSlider;
        private ColorDialog CensorBoxColor;
        private Button BoxColorButton;
    }
}
