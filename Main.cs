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
        private Timer backupTimer;
        private Timer backupStatusTimer;
        private string backupStatus;
        public int backupTimerElapsed = 0;
        private string[] backups;
        private bool closeAfterFinish;

        private FileScanner currentBackupFilescanner;
        private bool backupInProgress {
            get { return this.currentBackupFilescanner != null; }
        }

        public Main(string backupToRun=null) {
            InitializeComponent();

            this.backupsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPDATA_FOLDER, Constants.BACKUPS_FOLDER);
            this.populateBackupsList();
            this.backupTimer = new Timer();
            this.backupTimer.Interval = 1000;
            this.backupTimer.Tick += new EventHandler(backupTimer_Tick);
            this.backupStatusTimer = new Timer();
            this.backupStatusTimer.Interval = 250;
            this.backupStatusTimer.Tick += new EventHandler(backupStatusTimer_Tick);

            Logger.Purge();

            if (backupToRun != null) {
                if (!this.backupsList.Items.Contains(backupToRun))
                    this.showError("Backup '"+backupToRun+"' doesn't exist");
                else {
                    this.backupsList.SelectedItem = backupToRun;
                    this.performBackup(true);
                }
            }
        }

        void backupTimer_Tick(object sender, EventArgs e) {
            this.backupTimerElapsed += 1;
            this.statusLabelTime.Text = String.Format("{0:d2}:{1:d2}", this.backupTimerElapsed / 60, this.backupTimerElapsed % 60);
        }

        private void populateBackupsList() {
            if (!Directory.Exists(this.backupsPath))
                return;
            this.backups = Directory.GetFiles(this.backupsPath).Where(file => Path.GetExtension(file) == Constants.BACKUP_EXTENSION).Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
            this.backupsList.DataSource = this.backups;
        }

        private void showError(string message, string caption="Error") {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string loadSelectedBackup() {
            if (this.backupsList.SelectedItem == null) {
                string message = (this.backups == null || this.backups.Length == 0) ? "You don't have any backups. Please make one and try again" : 
                    "You haven't selected a backup. Please select one and try again";
                MessageBox.Show(message, "No backup selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            return Path.Combine(this.backupsPath, this.backupsList.SelectedItem.ToString()) + Constants.BACKUP_EXTENSION;
        }

        private void createBackup() {
            string tempFile = Path.GetTempFileName();
            Database database = Database.CreateDatabase(tempFile);
            Settings settings = new Settings(database);
            settings.PopulateInitial("Unnamed Backup");

            PropertiesForm propertiesForm = new PropertiesForm(settings, this.backups);
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
            string backupName = this.loadSelectedBackup();
            if (backupName == null)
                return;
            DialogResult result = MessageBox.Show("Are you sure you want to delete the backup "+backupName+"?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
                return;
            string filepath = this.loadSelectedBackup();
            if (filepath == null)
                return;
            Database database = new Database(filepath);
            Settings settings = new Settings(database);
            string name = settings.Name;
            database.Close();
            try {
                File.Delete(filepath);
            }
            catch (IOException e) {
                this.showError("Could not delete "+backupName+": "+e.Message);
            }
            Scheduler.Delete(name);
            this.populateBackupsList();
        }

        private void importBackup() {
            this.openFileDialogImport.Filter = "Database files|*" + Constants.BACKUP_EXTENSION;
            DialogResult result = this.openFileDialogImport.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            string orgPath = this.openFileDialogImport.FileName;
            // Get a copy of it, so we can change stuff
            string path = Path.GetTempFileName();
            File.Copy(orgPath, path, true);
            // Get its name out
            Database database = new Database(path);
            Settings settings = new Settings(database);
            
            // Invoking the properties form ensures that everything's valid: unique name, etc
            PropertiesForm propertiesForm = new PropertiesForm(settings, this.backups, true);
            propertiesForm.ShowDialog();
            propertiesForm.Close();

            string name = settings.Name;
            database.Close();

            if (!propertiesForm.Saved) {
                File.Delete(path);
                return;
            }

            // Move it to the right dir
            File.Move(path, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPDATA_FOLDER, Constants.BACKUPS_FOLDER, name + Constants.BACKUP_EXTENSION));
            this.populateBackupsList();
        }

        private void performBackup(bool fromScheduler=false, bool performTest=false) {
            string backup = this.loadSelectedBackup();
            if (backup == null)
                return;

            this.buttonBackup.Enabled = false;
            this.buttonTest.Enabled = false;
            this.buttonCancel.Enabled = true;

            this.backupStatus = "Starting...";
            this.backupTimerElapsed = 0;
            this.backupTimer.Start();
            this.backupStatusTimer.Start();

            Database database = new Database(backup);
            database.AutoSyncToDisk = true;
            Settings settings = new Settings(database);
            settings.LastRun = DateTime.Now;
            Logger logger = new Logger(settings.Name);
            this.updateInfoDisplay();
            this.closeAfterFinish = (fromScheduler && settings.Autoclose);

            List<BackendBase> backendBases = new List<BackendBase>();

            if (settings.MirrorEnabled) {
                MirrorBackend backend = new MirrorBackend(settings.MirrorDest);
                backendBases.Add(backend);
            }
            if (settings.S3Enabled) {
                S3Backend backend = new S3Backend(settings.S3Dest, settings.S3PublicKey, settings.S3PrivateKey);
                backendBases.Add(backend);
            }

            this.backgroundWorkerBackup.RunWorkerAsync(new BackupItem(database, settings, logger, backendBases.ToArray(), fromScheduler, performTest));
        }

        private void backgroundWorkerBackup_DoWork(object sender, DoWorkEventArgs e) {
            BackupItem backupArgs = (BackupItem)e.Argument;
            Settings settings = backupArgs.Settings;
            Database database = backupArgs.Database;
            Logger logger = backupArgs.Logger;
            BackendBase[] backends = backupArgs.BackendBases;

            database.Open();
            try {
                database.LoadToMemory();
            }
            catch (Database.DatabaseInUseException ex) {
                database.Close();
                this.showError("The database is currently in use (lockfile exists). Are you running this backup elsewhere?\n\nIf you're certain this is the only instance of the program running, delete "+ex.LockFile);
                this.finishBackup(database, logger, "Error");
                return;
            }

            try {
                foreach (BackendBase backend in backends) {
                    this.InvokeEx(f => f.statusLabelBackupAction.Text = "Setting up "+backend.Name+" backend...");
                    backend.SetupInitial();
                }
            }
            catch (IOException ex) {
                this.showError("Error setting up backends: "+ex.Message);
                database.Close();
                this.finishBackup(database, logger, "Error");
                return;
            }

            this.currentBackupFilescanner = new FileScanner(settings.Source, database, logger, backends);
            this.currentBackupFilescanner.BackupAction += new FileScanner.BackupActionEventHandler(fileScanner_BackupAction);

            this.backupStatus = "Pruning database...";
            this.currentBackupFilescanner.PruneDatabase();
            // Test has to come after prune, but before backup
            if (!this.currentBackupFilescanner.Cancelled && backupArgs.PerformTest) {
                this.backupStatus = "Testing...";
                this.currentBackupFilescanner.TestDest();
            }
            if (!this.currentBackupFilescanner.Cancelled)
                this.currentBackupFilescanner.Backup();
            if (!this.currentBackupFilescanner.Cancelled) {
                this.backupStatus = "Purging...";
                this.currentBackupFilescanner.PurgeDest();
            }

            settings.LastRunCancelled = this.currentBackupFilescanner.Cancelled;
            settings.LastRunErrors = this.currentBackupFilescanner.WarningOccurred;

            // Need to close to actually back up the database
            database.Close();
            if (!this.currentBackupFilescanner.Cancelled) {
                this.backupStatus = "Backing up database...";
                FileScanner.BackupDatabase(database.FilePath, backends);
            }

            if (this.currentBackupFilescanner.WarningOccurred && !(backupArgs.FromScheduler && settings.IgnoreWarnings)) {
                DialogResult result = MessageBox.Show("One or more warnings occurred. Do you want to view the log file?", "Some warnings happened", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) {
                    Process.Start(logger.LogFilePath);
                }
            }

            this.finishBackup(database, logger, this.currentBackupFilescanner.Cancelled ? "Cancelled" : "Completed");
        }

        private void finishBackup(Database database, Logger logger, string status) {
            this.backupTimer.Stop();
            this.backupStatusTimer.Stop();

            this.currentBackupFilescanner = null;
            logger.Close();

            this.InvokeEx(f => f.statusLabelBackupAction.Text = status);
            this.InvokeEx(f => f.buttonBackup.Enabled = true);
            this.InvokeEx(f => f.buttonTest.Enabled = true);
            this.InvokeEx(f => f.buttonCancel.Enabled = false);
            // Files/folders could will have changed
            this.InvokeEx(f => f.updateInfoDisplay());

            if (this.closeAfterFinish)
                this.InvokeEx(f => f.Close());
        }

        private void fileScanner_BackupAction(object sender, FileScanner.BackupActionItem item) {
            string text = item.To;
            if (item.Operation == FileScanner.BackupActionOperation.Hash)
                text = "Hash: " + text;
            if (item.Backend == "S3")
                text = "S3://" + text;
            if (this.currentBackupFilescanner.Cancelled)
                text = "Cancelling: " + text;
            if (item.Percent < 100)
                text += " ("+item.Percent+"%)";
            this.backupStatus = text;
        }

        void backupStatusTimer_Tick(object sender, EventArgs e) {
            this.InvokeEx(f => f.statusLabelBackupAction.Text = this.backupStatus);
        }

        private void cancelBackup() {
            if (this.currentBackupFilescanner == null || this.currentBackupFilescanner.Cancelled)
                return;
            this.currentBackupFilescanner.Cancel();
            // TODO: Remove duplication between this and fileScanner_BackupAction
            this.backupStatus = "Cancelling: " + this.backupStatus;
        }

        private void buttonProperties_Click(object sender, EventArgs e) {
            string fileName = this.loadSelectedBackup();
            if (fileName == null)
                return;
            Database database = new Database(fileName);
            Settings settings = new Settings(database);

            PropertiesForm propertiesForm = new PropertiesForm(settings, this.backups);
            propertiesForm.ShowDialog();
            propertiesForm.Close();

            // need to close DB before move
            string afterName = Path.Combine(this.backupsPath, settings.Name) + Constants.BACKUP_EXTENSION;
            database.Close();

            if (afterName != fileName) {
                File.Move(fileName, afterName);
                this.populateBackupsList();
            }
            this.updateInfoDisplay();
        }

        private void updateInfoDisplay() {
            string fileName = this.loadSelectedBackup();
            if (fileName == null)
                return;
            Database database = new Database(fileName);
            Settings settings = new Settings(database);
            DateTime lastRun = settings.LastRun;
            this.labelLastRun.Text = lastRun == new DateTime(1970, 1, 1) ? "Never" : (lastRun.ToString() + 
                    (settings.LastRunCancelled ? ". Cancelled" : ". Completed") + 
                    (settings.LastRunErrors ? " with warnings" : (settings.LastRunCancelled ? "" : " successfully")));
            this.labelSource.Text = settings.Source;
            int numFiles = database.NumFiles();
            int numFolders = database.NumFolders();
            this.labelStats.Text = String.Format("{0:n0} file{1}, {2:n0} folder{3}", numFiles, numFiles == 1 ? "" : "s", numFolders, numFolders == 1 ? "" : "s");
            List<string> dest = new List<string>();
            if (settings.MirrorEnabled)
                dest.Add(settings.MirrorDest);
            if (settings.S3Enabled)
                dest.Add("S3://"+settings.S3Dest);
            this.labelDest.Text = dest.Count == 0 ? "None" : String.Join("\n", dest);
            database.Close();
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

        private void backupsList_SelectedIndexChanged(object sender, EventArgs e) {
            this.updateInfoDisplay();
        }

        private struct BackupItem
        {
            public Database Database;
            public Settings Settings;
            public Logger Logger;
            public BackendBase[] BackendBases;
            public bool FromScheduler;
            public bool PerformTest;

            public BackupItem(Database database, Settings settings, Logger logger, BackendBase[] backendBases, bool fromScheduler, bool performTest) {
                this.Database = database;
                this.Settings = settings;
                this.Logger = logger;
                this.BackendBases = backendBases;
                this.FromScheduler = fromScheduler;
                this.PerformTest = performTest;
            }
        }

        private void buttonImport_Click(object sender, EventArgs e) {
            this.importBackup();
        }

        private void buttonTest_Click(object sender, EventArgs e) {
            this.performBackup(false, true);
        }

        private void buttonViewLogs_Click(object sender, EventArgs e) {
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPDATA_FOLDER, Constants.LOG_FOLDER));
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
