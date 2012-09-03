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
        public Database Database { get; private set; }
        private BackupActionItem lastBackupActionItem;
        public Logger Logger { get; private set; }
        public string Name { get; private set; }

        public bool Cancelled { get; private set; }

        public bool WarningOccurred { get; private set; }

        public delegate void BackupActionEventHandler(object sender, BackupActionItem item);
        public event BackupActionEventHandler BackupAction;

        public FileScanner(string startDir, Database database, Logger logger, string name, BackendBase[] backends) {
            this.startDir = startDir;
            this.Database = database;
            this.treeTraverser = new TreeTraverser(startDir);
            this.fileDatabase = new FileDatabase(database);
            this.backends = backends;
            foreach (BackendBase backend in this.backends) {
                backend.CopyProgress += new BackendBase.CopyProgressEventHandler(Backend_CopyProgress);
            }
            this.Logger = logger;
            this.Name = name;
        }

        public void Cancel() {
            this.Cancelled = true;
            foreach (BackendBase backend in this.backends)
                backend.Cancel();
        }

        public void PurgeDest() {
            // Remove all entries in the destination filesystem which aren't in the database or the source filesystem
            // WARNING: Could potentially destroy information
            IEnumerable<string> files = this.fileDatabase.RecordedFiles().Select(x => x.Path);
            IEnumerable<string> folders = this.fileDatabase.RecordedFolders().Select(x => x.Path);

            foreach (BackendBase backend in this.backends) {
                this.Logger.Info("{0}: Removing files from the destination which aren't in the database or source filesystem", backend.Name);
                backend.PurgeFiles(files, folders, (entity, file) => {
                    this.reportBackupAction(new BackupActionItem(null, file, entity == BackendBase.PurgeEntity.File ? BackupActionEntity.File : BackupActionEntity.Folder,
                        BackupActionOperation.Purge));
                    return !this.Cancelled;
                });
            }
        }

        public static void BackupDatabase(string databasePath, BackendBase[] backends) {
            foreach (BackendBase backend in backends) {
                string dbFile = Database.GetExportableFile(databasePath, backend.StripFilesFoldersOnDBBackup);
                backend.BackupDatabase(Path.GetFileName(databasePath), dbFile);
            }
        }

        public void Backup() {
            this.Cancelled = false;
            this.WarningOccurred = false;

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

            while (folder.Level >= 0 && !this.Cancelled) {
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
                            case FileDatabase.FolderModStatus.Unmodified:
                                nextFolder = true;
                                curFolderId = folderStatus.Id;
                                this.checkFolder(folder.Name);
                                break;
                        }
                    }

                    // Check for files in this folder
                    files = this.treeTraverser.ListFiles(folder.Name);
                }
                catch (BackupOperationException e) { this.handleOperationException(e); }

                foreach (string file in files) {
                    if (this.Cancelled)
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
                            case FileDatabase.FileModStatus.Unmodified:
                                // We don't think anything's changed... but make sure the file exists on the backends
                                this.checkFile(curFolderId, file, fileStatus.MD5, fileLastModified);
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

            if (!this.Cancelled) {
                // Now we look for file deletions
                FileDatabase.FileRecord[] recordedFiles = this.fileDatabase.RecordedFiles();
                foreach (FileDatabase.FileRecord fileToCheck in recordedFiles) {
                    if (this.Cancelled)
                        break;
                    if (!this.treeTraverser.FileExists(fileToCheck.Path)) {
                        this.deleteFile(fileToCheck.Id, fileToCheck.Path);
                    }
                }
            }

            if (!this.Cancelled) {
                // And finally folder deletions
                FileDatabase.FolderRecord[] recordedFolders = this.fileDatabase.RecordedFolders();
                foreach (FileDatabase.FolderRecord folderToCheck in recordedFolders) {
                    if (this.Cancelled)
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

        private void checkFolder(string folder) {
            foreach (BackendBase backend in this.backends) {
                if (!backend.FolderExists(folder)) {
                    this.reportBackupAction(new BackupActionItem(null, folder, BackupActionEntity.Folder, BackupActionOperation.Add, backend.Name));
                    backend.CreateFolder(folder);
                    this.Logger.Info("{0}: Folder missing from backend, so re-creating: {1}", backend.Name, folder);
                }
            }
        }

        private void addFile(int folderId, string file, DateTime lastModified) {
            string fileMD5 = this.treeTraverser.FileMd5(file, (percent) => {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Hash, null, percent));
                return !this.Cancelled;
            });
            if (this.Cancelled)
                return;

            // Just do a search for alternates (files in a different place on the remote location with the same hash)
            // as this will speed up copying
            if (this.alternateFile(folderId, file, fileMD5, lastModified, false))
                return;

            foreach (BackendBase backend in this.backends) {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Add, backend.Name));
                backend.CreateFile(file, this.treeTraverser.GetFileSource(file), fileMD5);
                this.Logger.Info("{0}: Added file: {1}", backend.Name, file);
            }
            this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
        }

        private bool alternateFile(int folderId, string file, string fileMD5, DateTime lastModified, bool update) {
            // if update is true, we're updating the dest file. otherwise we're adding it
            // Return true if we made use of an alternate, or false if we did nothing
            string logAction = update ? "Updated" : "Added";
            FileDatabase.FileRecord alternate = this.fileDatabase.SearchForAlternates(fileMD5);
            if (alternate.Id == 0)
                return false;

            // First: Is is a copy or a move?
            if (this.treeTraverser.FileExists(alternate.Path)) {
                // It's a copy
                foreach (BackendBase backend in this.backends) {
                    this.reportBackupAction(new BackupActionItem(alternate.Path, file, BackupActionEntity.File, BackupActionOperation.Copy, backend.Name));
                    if (backend.CreateFromAlternateCopy(file, alternate.Path))
                        this.Logger.Info("{0}: {1} file: {2} from alternate {3} (copy)", backend.Name, logAction, file, alternate.Path);
                    else {
                        backend.CreateFile(file, this.treeTraverser.GetFileSource(file), fileMD5);
                        this.Logger.Info("{0}: {1} file: {2} (backend refused alternate {3})", backend.Name, logAction, file, alternate.Path);
                    }
                }
                if (update)
                    this.fileDatabase.UpdateFile(folderId, file, lastModified, fileMD5);
                else
                    this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
            }
            else {
                // It's a move
                foreach (BackendBase backend in this.backends) {
                    this.reportBackupAction(new BackupActionItem(alternate.Path, file, BackupActionEntity.File, BackupActionOperation.Move, backend.Name));
                    backend.CreateFromAlternateMove(file, alternate.Path);
                    this.Logger.Info("{0}: {1} file: {2} from alternate {3} (move)", backend.Name, logAction, file, alternate.Path); 
                }
                if (update)
                    this.fileDatabase.UpdateFile(folderId, file, lastModified, fileMD5);
                else
                    this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
                this.fileDatabase.DeleteFile(alternate.Id);
            }

            return true;
        }

        private void updatefile(int folderId, string file, string remoteMD5, DateTime lastModified) {
            this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Hash));
            // Only copy if the file has actually changed
            string fileMD5 = this.treeTraverser.FileMd5(file, (percent) => {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Hash, null, percent));
                return !this.Cancelled;
            });
            if (this.Cancelled)
                return;

            if (remoteMD5 == fileMD5) {
                foreach (BackendBase backend in this.backends) {
                    backend.TouchFile(file, lastModified);
                }
                this.Logger.Info("File mtime changed, but file unchanged. Touching: {0}", file);
                return;
            }

            if (this.alternateFile(folderId, file, fileMD5, lastModified, true))
                return;

            foreach (BackendBase backend in this.backends) {
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Update, backend.Name));
                if (backend.CreateFile(file, this.treeTraverser.GetFileSource(file), fileMD5))
                    this.Logger.Info("{0}: Updated file: {1}", backend.Name, file);
                else
                    this.Logger.Info("{0}: Skipped file {1} (mtime changed but file up-to-date)", backend.Name, file);
            }
            // But update the last modified time either way
            this.fileDatabase.UpdateFile(folderId, file, lastModified, fileMD5);
        }

        private void checkFile(int folderId, string file, string fileMD5, DateTime lastModified) {
            foreach (BackendBase backend in this.backends) {
                if (!backend.TestFile(file, lastModified, fileMD5)) {
                    // Aha! File's gone missing from the backend
                    this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Add, backend.Name));
                    if (backend.CreateFile(file, this.treeTraverser.GetFileSource(file), fileMD5))
                        this.Logger.Info("{0}: File on backend missing or modified, so re-creating: {1}", backend.Name, file);
                }
                else
                    this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Nothing));
            }
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
            this.WarningOccurred = true;
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
        public enum BackupActionOperation { Add, Delete, Copy, Move, Update, Hash, Prune, Purge, Test, Nothing };
        public struct BackupActionItem
        {
            public string From;
            public string To;
            public BackupActionEntity Entity;
            public BackupActionOperation Operation;
            public int Percent;
            public string Backend;

            public BackupActionItem(string from, string to, BackupActionEntity entity, BackupActionOperation operation, string backend=null, int percent=100) {
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
