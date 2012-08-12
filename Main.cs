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

        private FileScanner currentBackupFilescanner;

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

        private void performBackup() {
            this.buttonBackup.Enabled = false;
            this.buttonCancel.Enabled = true;

            this.backupTimerElapsed = 0;
            this.backupTimer.Start();

            this.lastStatusUpdate = DateTime.Now;

            Database database = new Database(this.loadSelectedBackup());
            Settings settings = new Settings(database);

            MirrorBackend backend = new MirrorBackend(settings.Dest);
            this.currentBackupFilescanner = new FileScanner(settings.Source, database, backend);
            this.currentBackupFilescanner.BackupAction += new FileScanner.BackupActionEventHandler(fileScanner_BackupAction);

            this.backgroundWorkerBackup.RunWorkerAsync();
        }

        private void backgroundWorkerBackup_DoWork(object sender, DoWorkEventArgs e) {
            this.currentBackupFilescanner.Database.LoadToMemory();

            this.currentBackupFilescanner.PruneDatabase();
            this.currentBackupFilescanner.Backup();
            this.currentBackupFilescanner.PurgeDest();
            this.currentBackupFilescanner.Database.Close();

            this.backupTimer.Stop();
            this.InvokeEx(f => f.statusLabelBackupAction.Text = this.currentBackupFilescanner.Cancelled ? "Cancelled": "Completed");

            this.currentBackupFilescanner = null;

            this.InvokeEx(f => f.buttonBackup.Enabled = true);
            this.InvokeEx(f => f.buttonCancel.Enabled = false);
        }

        void fileScanner_BackupAction(object sender, FileScanner.BackupActionItem item) {
            // Don't update *too* frequently, as this actually slows us down considerably
            if (DateTime.Now - this.lastStatusUpdate < new TimeSpan(0, 0, 0, 0, 100))
                return;
            this.InvokeEx(f => f.statusLabelBackupAction.Text = item.To);
            this.lastStatusUpdate = DateTime.Now;
            //this.Update();
        }

        private void buttonProperties_Click(object sender, EventArgs e) {
            string fileName = this.loadSelectedBackup();
            Database database = new Database(fileName);
            Settings settings = new Settings(database);

            Properties propertiesForm = new Properties(settings, this.backups);
            propertiesForm.ShowDialog();
            propertiesForm.Close();

            // need to close DB before move
            string afterName = settings.Name + Constants.BACKUP_EXTENSION;
            database.Close();

            if (afterName != fileName) {
                File.Move(Path.Combine(this.backupsPath, fileName), Path.Combine(this.backupsPath, afterName));
                this.populateBackupsList();
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e) {
            this.createBackup();
        }

        private void buttonBackup_Click(object sender, EventArgs e) {
            this.performBackup();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            if (this.currentBackupFilescanner == null)
                return;
            this.currentBackupFilescanner.Cancel();
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
