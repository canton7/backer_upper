using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;

namespace BackerUpper
{
    partial class PropertiesForm : Form
    {
        public bool Saved { get; private set; }
        private Settings settings;
        private string[] takenBackupNames;
        private string initialBackupName;

        const string FORBIDDEN_CHARS = "\\/:*?\"<>|";

        public PropertiesForm(Settings settings, string[] takenBackupNames, bool backupIsNew=false) {
            InitializeComponent();

            this.Saved = false;
            this.settings = settings;
            this.takenBackupNames = takenBackupNames;
            // Only use this info if the backup isn't new. New backups don't yet exist, in the scheduler or in the filesystem
            this.initialBackupName = backupIsNew ? null : settings.Name;
            this.loadValues();
        }

        private void loadValues() {
            this.textBoxName.Text = this.settings.Name;
            this.textBoxSource.Text = this.settings.Source;
            this.textBoxIgnorePattern.Text = this.settings.FileIgnorePattern;
            this.checkBoxMirror.Checked = this.settings.MirrorEnabled;
            this.textBoxMirrorDest.Text = this.settings.MirrorDest;
            this.checkBoxS3.Checked = this.settings.S3Enabled;
            this.textBoxS3Dest.Text = this.settings.S3Dest;
            this.textBoxS3PublicKey.Text = this.settings.S3PublicKey;
            this.textBoxS3PrivateKey.Text = this.settings.S3PrivateKey;
            this.checkBoxUseRRS.Checked = this.settings.S3UseRRS;
            this.checkBoxS3Test.Checked = this.settings.S3Test;
            this.checkBoxAutoclose.Checked = this.settings.Autoclose;
            this.checkBoxIgnoreWarnings.Checked = this.settings.IgnoreWarnings;

            this.enableDisableDests();

            this.loadScheduler();
        }

        private void saveValues() {
            this.settings.Name = this.textBoxName.Text;
            this.settings.Source = this.textBoxSource.Text;
            this.settings.FileIgnorePattern = this.textBoxIgnorePattern.Text;
            this.settings.MirrorEnabled = this.checkBoxMirror.Checked;
            this.settings.MirrorDest = this.textBoxMirrorDest.Text;
            this.settings.S3Enabled = this.checkBoxS3.Checked;
            this.settings.S3Dest = this.textBoxS3Dest.Text;
            this.settings.S3PublicKey = this.textBoxS3PublicKey.Text;
            this.settings.S3PrivateKey = this.textBoxS3PrivateKey.Text;
            this.settings.S3UseRRS = this.checkBoxUseRRS.Checked;
            this.settings.S3Test = this.checkBoxS3Test.Checked;
            this.settings.Autoclose = this.checkBoxAutoclose.Checked;
            this.settings.IgnoreWarnings = this.checkBoxIgnoreWarnings.Checked;

            this.setupTask();
        }


        private void loadScheduler() {
            Scheduler scheduler = Scheduler.Load(this.initialBackupName);
            DaysOfTheWeek dow = scheduler.DaysOfTheWeek;

            this.checkBoxScheduleMon.Checked = dow.HasFlag(DaysOfTheWeek.Monday);
            this.checkBoxScheduleTues.Checked = dow.HasFlag(DaysOfTheWeek.Tuesday);
            this.checkBoxScheduleWeds.Checked = dow.HasFlag(DaysOfTheWeek.Wednesday);
            this.checkBoxScheduleThurs.Checked = dow.HasFlag(DaysOfTheWeek.Thursday);
            this.checkBoxScheduleFri.Checked = dow.HasFlag(DaysOfTheWeek.Friday);
            this.checkBoxScheduleSat.Checked = dow.HasFlag(DaysOfTheWeek.Saturday);
            this.checkBoxScheduleSun.Checked = dow.HasFlag(DaysOfTheWeek.Sunday);

            this.checkBoxUseScheduler.Checked = scheduler.Enabled;
            this.checkBoxSchedulerWhenAvailable.Checked = scheduler.StartWhenAvailable;
            this.checkBoxSchedulerOnBatteries.Checked = scheduler.StartOnBatteries;
        }

        private void setupTask() {
            DateTime start = DateTime.Today + new TimeSpan(this.dateTimePickerScheduleTime.Value.Hour, this.dateTimePickerScheduleTime.Value.Minute, 0);
            // If we're starting in the past, move to tomorrow
            if (start < DateTime.Now)
                start += new TimeSpan(1, 0, 0, 0);

            DaysOfTheWeek dow = 0;
            if (this.checkBoxScheduleMon.Checked) { dow |= DaysOfTheWeek.Monday; }
            if (this.checkBoxScheduleTues.Checked) { dow |= DaysOfTheWeek.Tuesday; }
            if (this.checkBoxScheduleWeds.Checked) { dow |= DaysOfTheWeek.Wednesday; }
            if (this.checkBoxScheduleThurs.Checked) { dow |= DaysOfTheWeek.Thursday; }
            if (this.checkBoxScheduleFri.Checked) { dow |= DaysOfTheWeek.Friday; }
            if (this.checkBoxScheduleSat.Checked) { dow |= DaysOfTheWeek.Saturday; }
            if (this.checkBoxScheduleSun.Checked) { dow |= DaysOfTheWeek.Sunday; }

            if (this.initialBackupName != null && this.initialBackupName != this.settings.Name)
                Scheduler.Delete(this.initialBackupName);
            Scheduler scheduler = new Scheduler(this.checkBoxUseScheduler.Checked, start, dow, this.checkBoxSchedulerWhenAvailable.Checked, this.checkBoxSchedulerOnBatteries.Checked);
            scheduler.Save(this.settings.Name);
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
            this.textBoxS3Dest.Text = this.textBoxS3Dest.Text.Trim().TrimEnd(new char[]{ '/' });
            this.textBoxS3PublicKey.Text = this.textBoxS3PublicKey.Text.Trim();
            this.textBoxS3PrivateKey.Text = this.textBoxS3PrivateKey.Text.Trim();

            if (this.textBoxName.Text == "")
                errors.Add("Name cannot be empty");
            else if (this.textBoxName.Text != this.initialBackupName && this.takenBackupNames.Contains(this.textBoxName.Text))
                errors.Add("A backup with that name exists already");
            else if (this.textBoxName.Text.IndexOfAny(FORBIDDEN_CHARS.ToCharArray()) != -1)
                errors.Add("Your backup name contains forbidden characters ("+FORBIDDEN_CHARS+")");
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

        private void checkBoxUseScheduler_CheckedChanged(object sender, EventArgs e) {
            this.groupBoxScheduler.Enabled = this.checkBoxUseScheduler.Checked;
        }

        private void linkLabelAWSCredientials_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(Constants.AWS_CREDENTIALS_URL);
        }
    }
}
