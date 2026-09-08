namespace BetaSafeFilter
{
    partial class MonitorPickerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MonitorComboBox = new ComboBox();
            okButton = new Button();
            SuspendLayout();
            // 
            // MonitorComboBox
            // 
            MonitorComboBox.FormattingEnabled = true;
            MonitorComboBox.Location = new Point(108, 37);
            MonitorComboBox.Name = "MonitorComboBox";
            MonitorComboBox.Size = new Size(172, 23);
            MonitorComboBox.TabIndex = 0;
            MonitorComboBox.SelectedIndexChanged += MonitorComboBox_SelectedIndexChanged;
            // 
            // okButton
            // 
            okButton.Location = new Point(130, 145);
            okButton.Name = "okButton";
            okButton.Size = new Size(124, 45);
            okButton.TabIndex = 1;
            okButton.Text = "Confirm Selection";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // MonitorPickerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(419, 232);
            Controls.Add(okButton);
            Controls.Add(MonitorComboBox);
            Name = "MonitorPickerForm";
            Text = "Monitor Picker Form";
            ResumeLayout(false);
        }

        #endregion

        private ComboBox MonitorComboBox;
        private Button okButton;
    }
}