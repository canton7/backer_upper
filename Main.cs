using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace BackerUpper
{
    partial class Main : Form
    {
        private string backupsPath;
        public Timer backupTimer;
        public int backupTimerElapsed = 0;
        private DateTime lastStatusUpdate;
        private string[] backups;
        private bool closeAfterFinish;
        private Queue<FileScanner> backupQueue;

        private FileScanner currentBackupFilescanner;
        private bool backupInProgress {
            get { return this.currentBackupFilescanner != null; }
        }

        public Main() {
            InitializeComponent();

            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //Database database = new Database("test.sqlite");

            //MirrorBackend backend = new MirrorBackend(@"D:\Users\Antony\Documents\projects\backer_upper\test_dest");
            //FileScanner fileScanner = new FileScanner(@"D:\Users\Antony\Documents\projects\backer_upper\test_src", database, backend);
            //FileScanner fileScanner = new FileScanner(@"D:\Users\Antony\Music", database, backend);

            //database.LoadToMemory();

            //fileScanner.PruneDatabase();
            //fileScanner.Backup();
            //fileScanner.PurgeDest();

            //database.Close();

            //sw.Stop();
            //MessageBox.Show(String.Format("Took {0}", sw.Elapsed));

            this.backupsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPDATA_FOLDER, Constants.BACKUPS_FOLDER);
            this.populateBackupsList();
            this.backupTimer = new Timer();
            this.backupTimer.Interval = 1000;
            this.backupTimer.Tick += new EventHandler(backupTimer_Tick);
            this.backupQueue = new Queue<FileScanner>();
        }

        void backupTimer_Tick(object sender, EventArgs e) {
            this.backupTimerElapsed += 1;
            this.statusLabelTime.Text = String.Format("{0:d2}:{1:d2}", this.backupTimerElapsed / 60, this.backupTimerElapsed % 60);
        }

        private void populateBackupsList() {
            if (!Directory.Exists(this.backupsPath))
                return;
            this.backups = Directory.GetFiles(this.backupsPath).Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
            this.backupsList.DataSource = this.backups;
        }

        private void showError(string message, string caption="Error") {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string loadSelectedBackup() {
            return Path.Combine(this.backupsPath, this.backupsList.SelectedItem.ToString()) + Constants.BACKUP_EXTENSION;
        }

        private void createBackup() {
            string tempFile = Path.GetTempFileName();
            Database database = Database.CreateDatabase(tempFile);
            Settings settings = new Settings(database);
            settings.PopulateInitial("Unnamed Backup");

            Properties propertiesForm = new Properties(settings, this.backups);
            propertiesForm.ShowDialog();

            if (propertiesForm.Saved == false)
                return;

            propertiesForm.Close();

            if (!Directory.Exists(this.backupsPath))
                Directory.CreateDirectory(this.backupsPath);
            string destFile = Path.Combine(this.backupsPath, settings.Name + Constants.BACKUP_EXTENSION);
            database.Close();

            File.Move(tempFile, destFile);
            this.populateBackupsList();
        }

        private void deleteBackup() {
            string backupName = this.backupsList.SelectedItem.ToString();
            DialogResult result = MessageBox.Show("Are you sure you want to delete the backup "+backupName+"?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
                return;
            string filepath = this.loadSelectedBackup();
            try {
                File.Delete(filepath);
            }
            catch (IOException e) {
                this.showError("Could not delete "+backupName+": "+e.Message);
            }
            this.populateBackupsList();
        }

        private void performBackup() {
            this.closeAfterFinish = false;

            this.buttonBackup.Enabled = false;
            this.buttonCancel.Enabled = true;

            this.backupTimerElapsed = 0;
            this.backupTimer.Start();

            this.lastStatusUpdate = DateTime.Now;

            Database database = new Database(this.loadSelectedBackup());
            Settings settings = new Settings(database);

            if (settings.MirrorEnabled) {
                MirrorBackend backend = new MirrorBackend(settings.MirrorDest);
                FileScanner fileScanner = new FileScanner(settings.Source, database, backend);
                fileScanner.BackupAction += new FileScanner.BackupActionEventHandler(fileScanner_BackupAction);
                this.backupQueue.Enqueue(fileScanner);
            }
            if (settings.S3Enabled) {
                S3Backend backend = new S3Backend(settings.S3Dest, settings.S3PublicKey, settings.S3PrivateKey);
                FileScanner fileScanner = new FileScanner(settings.Source, database, backend);
                fileScanner.BackupAction += new FileScanner.BackupActionEventHandler(fileScanner_BackupAction);
                this.backupQueue.Enqueue(fileScanner);
            }

            this.backgroundWorkerBackup.RunWorkerAsync();
        }

        private void backgroundWorkerBackup_DoWork(object sender, DoWorkEventArgs e) {
            while (this.backupQueue.Count > 0) {
                this.currentBackupFilescanner = this.backupQueue.Dequeue();
                this.InvokeEx(f => f.statusLabelBackupAction.Text = "Setting up "+this.currentBackupFilescanner.Backend.Name+" backend...");

                this.currentBackupFilescanner.Backend.SetupInitial();
                // Although databases auto-open, we can run multiple backends for a single db, and that can close the db
                // Therefore, ensure it's actually open
                this.currentBackupFilescanner.Database.Open(); 
                this.currentBackupFilescanner.Database.LoadToMemory();

                this.currentBackupFilescanner.PruneDatabase();
                this.currentBackupFilescanner.Backup();
                this.currentBackupFilescanner.PurgeDest();
                this.currentBackupFilescanner.Database.Close();   
            }

            this.backupTimer.Stop();
            this.InvokeEx(f => f.statusLabelBackupAction.Text = this.currentBackupFilescanner.Cancelled ? "Cancelled": "Completed");

            this.currentBackupFilescanner = null;

            this.InvokeEx(f => f.buttonBackup.Enabled = true);
            this.InvokeEx(f => f.buttonCancel.Enabled = false);

            if (this.closeAfterFinish)
                this.InvokeEx(f => f.Close());
        }

        private void fileScanner_BackupAction(object sender, FileScanner.BackupActionItem item) {
            // Don't update *too* frequently, as this actually slows us down considerably
            //if (DateTime.Now - this.lastStatusUpdate < new TimeSpan(0, 0, 0, 0, 100))
            //    return;
            this.InvokeEx(f => f.statusLabelBackupAction.Text = item.To);
            this.lastStatusUpdate = DateTime.Now;
        }

        private void cancelBackup() {
            if (this.currentBackupFilescanner == null)
                return;
            this.currentBackupFilescanner.Cancel();
            this.statusLabelBackupAction.Text = "Cancelling: "+this.statusLabelBackupAction.Text;
        }

        private void buttonProperties_Click(object sender, EventArgs e) {
            string fileName = this.loadSelectedBackup();
            Database database = new Database(fileName);
            Settings settings = new Settings(database);

            Properties propertiesForm = new Properties(settings, this.backups);
            propertiesForm.ShowDialog();
            propertiesForm.Close();

            // need to close DB before move
            string afterName = Path.Combine(this.backupsPath, settings.Name) + Constants.BACKUP_EXTENSION;
            database.Close();

            if (afterName != fileName) {
                File.Move(fileName, afterName);
                this.populateBackupsList();
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e) {
            this.createBackup();
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            this.deleteBackup();
        }

        private void buttonBackup_Click(object sender, EventArgs e) {
            this.performBackup();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.cancelBackup();
        }

        private void buttonExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.backupInProgress) {
                this.closeAfterFinish = true;
                e.Cancel = true;
                this.cancelBackup();
            }
        }
    }

    public static class ISynchronizeInvokeExtensions
    {
        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke {
            if (@this.InvokeRequired) {
                @this.Invoke(action, new object[] { @this });
            }
            else {
                action(@this);
            }
        }
    }

}
