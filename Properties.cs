using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BackerUpper
{
    partial class Properties : Form
    {
        public bool Saved = false;
        private Settings settings;

        public Properties(Settings settings) {
            InitializeComponent();

            this.settings = settings;
            this.loadValues();
        }

        private void loadValues() {
            this.textBoxName.Text = this.settings.Name;
            this.textBoxSource.Text = this.settings.Source;
            this.textBoxDest.Text = this.settings.Dest;
        }

        private bool checkValues() {
            List<String> errors = new List<string>();

            if (this.textBoxName.Text == "")
                errors.Add("Name cannot be empty");
            if (this.textBoxSource.Text == "")
                errors.Add("Source cannot be empty");
            else if (!Directory.Exists(this.textBoxSource.Text))
                errors.Add("Source directory doesn't exist");
            if (this.textBoxDest.Text == "")
                errors.Add("Dest cannot be empty");
            else if (!Directory.Exists(this.textBoxDest.Text))
                errors.Add("Dest folder doesn't exist");
            if (this.textBoxSource.Text != "" && this.textBoxSource.Text == this.textBoxDest.Text)
                errors.Add("Source folder can't be the same as Dest folder"); 

            if (errors.Count == 0)
                return true;

            StringBuilder message = new StringBuilder("The following errors were encountered:\n");
            foreach (string error in errors)
                message.Append("  - " + error + "\n");
            MessageBox.Show(message.ToString(), "Errors encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void saveValues() {
            this.settings.Name = this.textBoxName.Text;
            this.settings.Source = this.textBoxSource.Text;
            this.settings.Dest = this.textBoxDest.Text;
        }

        private void buttonSourceBrowser_Click(object sender, EventArgs e) {
            this.sourceBrowser.SelectedPath = this.textBoxSource.Text;
            this.sourceBrowser.ShowDialog();
            this.textBoxSource.Text = this.sourceBrowser.SelectedPath;
        }

        private void buttonDestBrowser_Click(object sender, EventArgs e) {
            this.destBrowser.SelectedPath = this.textBoxDest.Text;
            this.destBrowser.ShowDialog();
            this.textBoxDest.Text = this.destBrowser.SelectedPath;
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            if (!this.checkValues())
                return;
            this.saveValues();
            this.Saved = true;
            this.Close();
        }


    }
}
