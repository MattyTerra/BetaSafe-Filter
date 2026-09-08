using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetaSafeFilter
{

    public partial class MonitorPickerForm : Form
    {
        public Screen? ScreenSelection { get; private set; }
        public MonitorPickerForm()
        {
            InitializeComponent();

            foreach (Screen screen in Screen.AllScreens)
            {
                MonitorComboBox.Items.Add(screen);
            }
            MonitorComboBox.SelectedIndex = 0;
        }

        private void MonitorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScreenSelection = MonitorComboBox.SelectedItem as Screen;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (MonitorComboBox.SelectedItem is Screen item)
            {
                ScreenSelection = MonitorComboBox.SelectedItem as Screen;
            }
                
            DialogResult = DialogResult.OK;
        }
    }
}
