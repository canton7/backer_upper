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
            string[] files = Directory.GetFiles(this.backupsPath).Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
            this.backupsList.DataSource = files;
        }

        private Database loadSelectedBackup() {
            string databaseFile = Path.Combine(this.backupsPath, this.backupsList.SelectedItem.ToString()) + Constants.BACKUP_EXTENSION;
            return new Database(databaseFile);
        }

        private void createBackup() {
            string tempFile = Path.GetTempFileName();
            Database database = Database.CreateDatabase(tempFile);
            Settings settings = new Settings(database);
            settings.PopulateInitial("Unnamed Backup");

            Properties propertiesForm = new Properties(settings);
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

            this.backupTimerElapsed = 0;
            this.backupTimer.Start();

            this.lastStatusUpdate = DateTime.Now;

            Database database = this.loadSelectedBackup();
            Settings settings = new Settings(database);

            MirrorBackend backend = new MirrorBackend(settings.Dest);
            FileScanner fileScanner = new FileScanner(settings.Source, database, backend);
            fileScanner.BackupAction += new FileScanner.BackupActionEventHandler(fileScanner_BackupAction);

            this.backgroundWorkerBackup.RunWorkerAsync(fileScanner);
        }

        private void backgroundWorkerBackup_DoWork(object sender, DoWorkEventArgs e) {
            FileScanner fileScanner = (FileScanner)e.Argument;
            fileScanner.Database.LoadToMemory();

            fileScanner.PruneDatabase();
            fileScanner.Backup();
            fileScanner.PurgeDest();

            fileScanner.Database.Close();

            this.backupTimer.Stop();
            this.InvokeEx(f => f.statusLabelBackupAction.Text = "Completed");

            this.InvokeEx(f => f.buttonBackup.Enabled = true);
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
            Database database = this.loadSelectedBackup();
            Settings settings = new Settings(database);

            Properties propertiesForm = new Properties(settings);
            propertiesForm.ShowDialog();

            database.Close();

        }

        private void buttonCreate_Click(object sender, EventArgs e) {
            this.createBackup();
        }

        private void buttonBackup_Click(object sender, EventArgs e) {
            this.performBackup();
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
