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
    partial class RestoreForm : Form
    {

        private Settings settings;
        public bool Saved { get; private set; }
        public string RestoreTo { get; private set; }
        public bool Overwrite { get; private set; }
        public bool OverwriteOnlyIfOlder { get; private set; }
        public string Backend { get; private set; }

        public RestoreForm(Settings settings) {
            InitializeComponent();

            this.settings = settings;
            this.Saved = false;

            this.labelBackupName.Text = settings.Name;
            if (settings.MirrorEnabled)
                this.comboBoxBackends.Items.Add("Mirror");
            if (settings.S3Enabled)
                this.comboBoxBackends.Items.Add("S3");
            this.comboBoxBackends.SelectedIndex = 0;
            if (comboBoxBackends.Items.Count == 1)
                this.comboBoxBackends.Enabled = false;
            this.textBoxRestoreTo.Text = settings.Source;
        }

        private bool checkValues() {
            List<String> errors = new List<string>();

            this.textBoxRestoreTo.Text = this.textBoxRestoreTo.Text.Trim();

            if (this.textBoxRestoreTo.Text == "")
                errors.Add("Restore to field cannot be empty");
            else if (!Directory.Exists(this.textBoxRestoreTo.Text))
                errors.Add("Restore to folder doesn't exist");

            if (errors.Count == 0)
                return true;

            StringBuilder message = new StringBuilder("The following errors were encountered:\n");
            foreach (string error in errors)
                message.Append("  - " + error + "\n");
            MessageBox.Show(message.ToString(), "Errors encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void comboBoxBackends_SelectedIndexChanged(object sender, EventArgs e) {
            switch (this.comboBoxBackends.Text) {
                case "Mirror":
                    this.labelBackupSource.Text = this.settings.MirrorDest;
                    break;
                case "S3":
                    this.labelBackupSource.Text = "S3://" + this.settings.S3Dest;
                    break;
            }
            
        }

        private void buttonDest_Click(object sender, EventArgs e) {
            this.folderBrowserRestoreTo.SelectedPath = this.textBoxRestoreTo.Text;
            this.folderBrowserRestoreTo.ShowDialog();
            this.textBoxRestoreTo.Text = this.folderBrowserRestoreTo.SelectedPath;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void buttonRestore_Click(object sender, EventArgs e) {
            if (!this.checkValues())
                return;
            this.RestoreTo = this.textBoxRestoreTo.Text;
            this.Overwrite = this.checkBoxOverwrite.Checked;
            this.OverwriteOnlyIfOlder = this.Overwrite && this.checkBoxOverwriteOnlyIfOlder.Checked;
            this.Saved = true;
            this.Backend = this.comboBoxBackends.Text;
            this.Close();
        }

        private void checkBoxOverwrite_CheckedChanged(object sender, EventArgs e) {
            this.checkBoxOverwriteOnlyIfOlder.Enabled = this.checkBoxOverwrite.Checked;
            if (!this.checkBoxOverwrite.Checked)
                this.checkBoxOverwriteOnlyIfOlder.Checked = false;
        }
    }
}
