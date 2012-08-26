using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackerUpper
{
    class FileScanner
    {
        private string startDir;
        private TreeTraverser treeTraverser;
        private FileDatabase fileDatabase;
        private BackendBase[] backends;
        public Database Database;
        private BackupActionItem lastBackupActionItem;
        public Logger Logger;

        private bool cancel = false;
        public bool Cancelled {
            get { return this.cancel; }
        }

        private bool warningOccurred = false;
        public bool WarningOccurred {
            get { return this.warningOccurred; }
        }

        public delegate void BackupActionEventHandler(object sender, BackupActionItem item);
        public event BackupActionEventHandler BackupAction;

        public FileScanner(string startDir, Database database, Logger logger, BackendBase[] backends) {
            this.startDir = startDir;
            this.Database = database;
            this.treeTraverser = new TreeTraverser(startDir);
            this.fileDatabase = new FileDatabase(database);
            this.backends = backends;
            foreach (BackendBase backend in this.backends) {
                backend.CopyProgress += new BackendBase.CopyProgressEventHandler(Backend_CopyProgress);
            }
            this.Logger = logger;
        }

        public void Cancel() {
            this.cancel = true;
            foreach (BackendBase backend in this.backends)
                backend.Cancel();
        }

        public void PruneDatabase() {
            // Remove all database entries where the file doesn't exist on the disk
            // This is an inconsistency we can't recover from otherwise, as we never check whether we need to re-copy the file

            // Start with the files
            FileDatabase.FileRecord[] files = this.fileDatabase.RecordedFiles();
            foreach (FileDatabase.FileRecord file in files) {
                if (this.cancel)
                    break;
                if (this.backends.Any(b => !b.FileExists(file.Path))) {
                    this.Logger.Info("Pruning database entry: file {0}", file.Path);
                    this.fileDatabase.DeleteFile(file.Id);
                }
            }

            // Then the folders
            FileDatabase.FolderRecord[] folders = this.fileDatabase.RecordedFolders();
            foreach (FileDatabase.FolderRecord folder in folders) {
                if (this.cancel)
                    break;
                if (this.backends.Any(b => !b.FolderExists(folder.Path))) {
                    this.Logger.Info("Pruning database entry: folder {0}", folder.Path);
                    this.fileDatabase.DeleteFolder(folder.Id);
                }
            }
        }

        public void PurgeDest() {
            // Remove all entries in the destination filesystem which aren't in the database or the source filesystem
            // WARNING: Could potentially destroy information
            IEnumerable<string> files = this.fileDatabase.RecordedFiles().Select(x => x.Path);
            IEnumerable<string> folders = this.fileDatabase.RecordedFolders().Select(x => x.Path);

            foreach (BackendBase backend in this.backends) {
                this.Logger.Info("{0}: Removing files from the destination which aren't in the database or source filesystem", backend.Name);
                backend.PurgeFiles(files, folders);
            }
        }

        public void TestDest() {
            // WE ASSUME THE FILES EXIST. (handled) errors happen if not
            // Check the dest against the database.
            // If the mtime or md5 (whichever is available) doesn't match, then we have a problem
            // What do we do? Remove it from the DB! Slightly drastic, but safe

            FileDatabase.FileRecordExtended[] files = this.fileDatabase.RecordedFilesExtended();
            foreach (FileDatabase.FileRecordExtended file in files) {
                if (this.cancel)
                    break;
                try {
                    if (this.backends.Any(b => !b.TestFile(file.Path, file.LastModified, file.FileMd5))) {
                        this.Logger.Info("File {0} modified on a backend. Removing from database", file.Path);
                        this.fileDatabase.DeleteFile(file.Id);
                    }
                }
                catch (BackupOperationException e) { this.handleOperationException(e); }
            }
        }

        public static void BackupDatabase(string databasePath, BackendBase[] backends) {
            foreach (BackendBase backend in backends) {
                string dbFile = Database.GetExportableFile(databasePath, backend.StripFilesFoldersOnDBBackup);
                backend.BackupDatabase(Path.GetFileName(databasePath), dbFile);
            }
        }

        public void Backup() {
            this.cancel = false;
            this.warningOccurred = false;

            TreeTraverser.FolderEntry folder = this.treeTraverser.FirstFolder();
            FileDatabase.FolderStatus folderStatus;
            int newFolderLevel = -1;
            int prevLevel = -1;
            bool nextFolder = true;
            int curFolderId = -1;
            string[] files;
            FileDatabase.FileStatus fileStatus;
            DateTime fileLastModified;

            // Do all the additions, then go through and do the deletions separately
            // This gives us a change to do renames, and makes sure that we empty folders before deleting them

            while (folder.Level >= 0 && !this.cancel) {
                // May not get assigned, due to try/catch, so assign empty as a default
                files = new string[0];

                try {
                    if (newFolderLevel >= 0 && folder.Level > newFolderLevel) {
                        // Just automatically add it, as a parent somewhere is new
                        curFolderId = this.addFolder(folder.Name);
                    }
                    else {
                        newFolderLevel = -1;
                        folderStatus = this.fileDatabase.InspectFolder(folder.Name);

                        switch (folderStatus.FolderModStatus) {
                            case FileDatabase.FolderModStatus.New:
                                newFolderLevel = folder.Level;
                                nextFolder = true;
                                curFolderId = this.addFolder(folder.Name);
                                break;
                            case FileDatabase.FolderModStatus.StillThere:
                                //this.Logger.Info("Skipping folder: {0}", folder.Name);
                                nextFolder = true;
                                curFolderId = folderStatus.Id;
                                break;
                        }
                    }

                    // Check for files in this folder
                    files = this.treeTraverser.ListFiles(folder.Name);
                }
                catch (BackupOperationException e) { this.handleOperationException(e); }

                foreach (string file in files) {
                    if (this.cancel)
                        break;
                    try {
                        fileLastModified = this.treeTraverser.GetFileLastModified(file);
                        fileStatus = this.fileDatabase.InspectFile(curFolderId, file, fileLastModified);

                        switch (fileStatus.FileModStatus) {
                            case FileDatabase.FileModStatus.New:
                                this.addFile(curFolderId, file, fileLastModified);
                                break;
                            case FileDatabase.FileModStatus.Modified:
                                this.updatefile(curFolderId, file, fileStatus.MD5, fileLastModified);
                                break;
                            default:
                                //this.Logger.Info("Skipping file: {0}", file);
                                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Nothing, null));
                                break;
                        }
                    }
                    catch (BackupOperationException e) { this.handleOperationException(e); }
                }

                try {
                    // Move onto the next folder
                    prevLevel = folder.Level;
                    folder = this.treeTraverser.NextFolder(nextFolder);
                }
                catch (BackupOperationException e) { this.handleOperationException(e); }
            }

            if (!this.cancel) {
                // Now we look for file deletions
                FileDatabase.FileRecord[] recordedFiles = this.fileDatabase.RecordedFiles();
                foreach (FileDatabase.FileRecord fileToCheck in recordedFiles) {
                    if (this.cancel)
                        break;
                    if (!this.treeTraverser.FileExists(fileToCheck.Path)) {
                        this.deleteFile(fileToCheck.Id, fileToCheck.Path);
                    }
                }
            }

            if (!this.cancel) {
                // And finally folder deletions
                FileDatabase.FolderRecord[] recordedFolders = this.fileDatabase.RecordedFolders();
                foreach (FileDatabase.FolderRecord folderToCheck in recordedFolders) {
                    if (this.cancel)
                        break;
                    try {
                        if (!this.treeTraverser.FolderExists(folderToCheck.Path)) {
                            this.deleteFolder(folderToCheck.Id, folderToCheck.Path);
                        }
                    }
                    catch (BackupOperationException e) { this.handleOperationException(e); }
                }
            }

            // Sync the database back to disk
            this.fileDatabase.SyncToDisk();
        }

        private int addFolder(string folder) {
            foreach (BackendBase backend in this.backends) {
                this.reportBackupAction(new BackupActionItem(null, folder, BackupActionEntity.Folder, BackupActionOperation.Add, backend.Name));
                backend.CreateFolder(folder);
                this.Logger.Info("{0}: Added folder: {1}", backend.Name, folder);
            }
            int insertedId = this.fileDatabase.AddFolder(folder);
            return insertedId;
        } 

        private void deleteFolder(int folderId, string folder) {
            foreach (BackendBase backend in this.backends) {
                this.reportBackupAction(new BackupActionItem(null, folder, BackupActionEntity.Folder, BackupActionOperation.Delete, backend.Name));
                backend.DeleteFolder(folder);
                this.Logger.Info("{0}: Deleted folder: {1}", backend.Name, folder);
            }
            this.fileDatabase.DeleteFolder(folderId);
        }

        private void addFile(int folderId, string file, DateTime lastModified) {
            string fileMD5 = this.treeTraverser.FileMd5(file, (percent) => {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Hash, null, percent));
                return !this.cancel;
            });
            if (this.cancel)
                return;

            // Just do a search for alternates (files in a different place on the remote location with the same hash)
            // as this will speed up copying
            FileDatabase.FileRecord alternate = this.fileDatabase.SearchForAlternates(fileMD5);
            if (alternate.Id > 0) {
                // Aha!
                this.alternateFile(folderId, file, fileMD5, lastModified, alternate.Path, alternate.Id);
                return;
            }

            foreach (BackendBase backend in this.backends) {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Add, backend.Name));
                backend.CreateFile(file, this.treeTraverser.GetFileSource(file), fileMD5);
                this.Logger.Info("{0}: Added file: {1}", backend.Name, file);
            }
            this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
        }

        private void alternateFile(int folderId, string file, string fileMD5, DateTime lastModified, string alternatePath, int alternateId) {
            // First: Is is a copy or a move?
            if (this.treeTraverser.FileExists(alternatePath)) {
                // It's a copy
                foreach (BackendBase backend in this.backends) {
                    this.reportBackupAction(new BackupActionItem(alternatePath, file, BackupActionEntity.File, BackupActionOperation.Copy, backend.Name));
                    if (backend.CreateFromAlternateCopy(file, alternatePath))
                        this.Logger.Info("{0}: Added file: {1} from alternate {2} (copy)", backend.Name, file, alternatePath);
                    else {
                        backend.CreateFile(file, this.treeTraverser.GetFileSource(file), fileMD5);
                        this.Logger.Info("{0}: Added file: {1} (backend refused alternate {2})", backend.Name, file, alternatePath);
                    }
                }
                this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
            }
            else {
                // It's a move
                foreach (BackendBase backend in this.backends) {
                    this.reportBackupAction(new BackupActionItem(alternatePath, file, BackupActionEntity.File, BackupActionOperation.Move, backend.Name));
                    backend.CreateFromAlternateMove(file, alternatePath);
                    this.Logger.Info("{0}: Added file: {1} from alternate {2} (move)", backend.Name, file, alternatePath); 
                }
                this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
                this.fileDatabase.DeleteFile(alternateId);
            }
        }

        private void updatefile(int folderId, string file, string remoteMD5, DateTime lastModified) {
            this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Hash, null));
            // Only copy if the file has actually changed
            string fileMD5 = this.treeTraverser.FileMd5(file, (percent) => {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Hash, null, percent));
                return !this.cancel;
            });
            if (this.cancel)
                return;

            if (remoteMD5 != fileMD5) {
                foreach (BackendBase backend in this.backends) {
                    this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Update, backend.Name));
                    backend.UpdateFile(file, this.treeTraverser.GetFileSource(file), fileMD5);
                    this.Logger.Info("{0}: Updated file: {1}", backend.Name, file);
                }
            }
            else {
                this.Logger.Info("Skipped file: {0} (mtime changed but file unchanged)", file);
            }
            // But update the last modified time either way
            this.fileDatabase.UpdateFile(folderId, file, lastModified, fileMD5);
        }

        private void deleteFile(int fileId, string file) {
            foreach (BackendBase backend in this.backends) {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Delete, backend.Name));
                backend.DeleteFile(file);
                this.Logger.Info("{0}: Deleted file: {1}", backend.Name, file);
            }
            this.fileDatabase.DeleteFile(fileId);
        }

        private void reportBackupAction(BackupActionItem item) {
            this.lastBackupActionItem = item;
            BackupAction(this, item);
        }

        private void handleOperationException(BackupOperationException e) {
            this.warningOccurred = true;
            this.Logger.Warn(e.Message);
        }

        private void Backend_CopyProgress(object sender, int percent) {
            this.lastBackupActionItem.Percent = percent;
            BackupAction(this, this.lastBackupActionItem);
        }

        private struct FoundFolderItem
        {
            public string Path;
            public List<string> Folders;

            public FoundFolderItem(string path) {
                this.Path = path;
                this.Folders = new List<string>();
            }
        }

        public enum BackupActionEntity { File, Folder };
        public enum BackupActionOperation { Add, Delete, Copy, Move, Update, Hash, Nothing };
        public struct BackupActionItem
        {
            public string From;
            public string To;
            public BackupActionEntity Entity;
            public BackupActionOperation Operation;
            public int Percent;
            public string Backend;

            public BackupActionItem(string from, string to, BackupActionEntity entity, BackupActionOperation operation, string backend, int percent=100) {
                this.From = from;
                this.To = to;
                this.Entity = entity;
                this.Operation = operation;
                this.Backend = backend;
                this.Percent = percent;
            }
        }


    }
}
