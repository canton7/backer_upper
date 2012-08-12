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
        private string[] takenBackupNames;
        private string initialBackupName;

        public Properties(Settings settings, string[] takenBackupNames) {
            InitializeComponent();

            this.settings = settings;
            this.takenBackupNames = takenBackupNames;
            this.initialBackupName = settings.Name;
            this.loadValues();
        }

        private void loadValues() {
            this.textBoxName.Text = this.settings.Name;
            this.textBoxSource.Text = this.settings.Source;
            this.checkBoxMirror.Checked = this.settings.MirrorEnabled;
            this.textBoxMirrorDest.Text = this.settings.MirrorDest;
            this.checkBoxS3.Checked = this.settings.S3Enabled;
            this.textBoxS3Dest.Text = this.settings.S3Dest;
            this.textBoxS3PublicKey.Text = this.settings.S3PublicKey;
            this.textBoxS3PrivateKey.Text = this.settings.S3PrivateKey;

            this.enableDisableDests();
        }

        private void saveValues() {
            this.settings.Name = this.textBoxName.Text;
            this.settings.Source = this.textBoxSource.Text;
            this.settings.MirrorEnabled = this.checkBoxMirror.Checked;
            this.settings.MirrorDest = this.textBoxMirrorDest.Text;
            this.settings.S3Enabled = this.checkBoxS3.Checked;
            this.settings.S3Dest = this.textBoxS3Dest.Text;
            this.settings.S3PublicKey = this.textBoxS3PublicKey.Text;
            this.settings.S3PrivateKey = this.textBoxS3PrivateKey.Text;
        }

        private void enableDisableDests() {
            this.groupBoxMirror.Enabled = this.checkBoxMirror.Checked;
            this.groupBoxS3.Enabled = this.checkBoxS3.Checked;
        }

        private bool checkValues() {
            List<String> errors = new List<string>();

            this.textBoxName.Text = this.textBoxName.Text.Trim();
            this.textBoxSource.Text = this.textBoxSource.Text.Trim();
            this.textBoxMirrorDest.Text = this.textBoxMirrorDest.Text.Trim();
            this.textBoxS3Dest.Text = this.textBoxS3Dest.Text.Trim();
            this.textBoxS3PublicKey.Text = this.textBoxS3PublicKey.Text.Trim();
            this.textBoxS3PrivateKey.Text = this.textBoxS3PrivateKey.Text.Trim();

            if (this.textBoxName.Text == "")
                errors.Add("Name cannot be empty");
            else if (this.textBoxName.Text != this.initialBackupName && this.takenBackupNames.Contains(this.textBoxName.Text))
                errors.Add("A backup with that name exists already");
            if (this.textBoxSource.Text == "")
                errors.Add("Source cannot be empty");
            else if (!Directory.Exists(this.textBoxSource.Text))
                errors.Add("Source directory doesn't exist");
            if (!this.checkBoxMirror.Checked && !this.checkBoxS3.Checked)
                errors.Add("You must select at least one backend");
            if (this.checkBoxMirror.Checked) {
                if (this.textBoxMirrorDest.Text == "")
                    errors.Add("Mirror Dest cannot be empty");
                else if (!Directory.Exists(this.textBoxMirrorDest.Text))
                    errors.Add("Mirror Dest folder doesn't exist");
                if (this.textBoxSource.Text != "" && this.textBoxSource.Text == this.textBoxMirrorDest.Text)
                    errors.Add("Source folder can't be the same as Mirror Dest folder");
            }
            if (this.checkBoxS3.Checked) {
                if (this.textBoxS3Dest.Text == "")
                    errors.Add("S3 Destination cannot be empty");
                if (this.textBoxS3PublicKey.Text == "")
                    errors.Add("S3 Public Key cannot be empty");
                if (this.textBoxS3PrivateKey.Text == "")
                    errors.Add("S3 Private Key cannot be empty");
            }

            if (errors.Count == 0)
                return true;

            StringBuilder message = new StringBuilder("The following errors were encountered:\n");
            foreach (string error in errors)
                message.Append("  - " + error + "\n");
            MessageBox.Show(message.ToString(), "Errors encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void buttonSourceBrowser_Click(object sender, EventArgs e) {
            this.sourceBrowser.SelectedPath = this.textBoxSource.Text;
            this.sourceBrowser.ShowDialog();
            this.textBoxSource.Text = this.sourceBrowser.SelectedPath;
        }

        private void buttonDestBrowser_Click(object sender, EventArgs e) {
            this.destBrowser.SelectedPath = this.textBoxMirrorDest.Text;
            this.destBrowser.ShowDialog();
            this.textBoxMirrorDest.Text = this.destBrowser.SelectedPath;
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            if (!this.checkValues())
                return;
            this.saveValues();
            this.Saved = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void checkBoxMirror_CheckedChanged(object sender, EventArgs e) {
            this.enableDisableDests();
        }

        private void checkBoxS3_CheckedChanged(object sender, EventArgs e) {
            this.enableDisableDests();
        }
    }
}
