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
                backend.PurgeFiles(files, folders, (entity, file, deleted) => {
                    this.reportBackupAction(new BackupActionItem(null, file, entity == BackendBase.Entity.File ? BackupActionEntity.File : BackupActionEntity.Folder,
                        BackupActionOperation.Purge));
                    // Slightly hacky: database files are purged, but the user doesn't need to know this
                    if (deleted && !(entity == BackendBase.Entity.File && file == Path.GetFileName(this.Database.FilePath)))
                        this.Logger.Info("{0}: Purged {1} from backend as it's old: {2}", backend.Name, entity == BackendBase.Entity.File ? "file" : "folder", file);
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
            IEnumerable<string> files;
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
                            case FileDatabase.FileModStatus.Newer:
                            case FileDatabase.FileModStatus.Older:
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
                IEnumerable<FileDatabase.FileRecord> recordedFiles = this.fileDatabase.RecordedFiles();
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
                foreach (FileDatabase.FolderRecord folderToCheck in this.fileDatabase.RecordedFolders()) {
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

            foreach (BackendBase backend in this.backends) {
                // Search for alternates
                if (this.alternateFile(folderId, file, fileMD5, lastModified, false, backend))
                    continue;
                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Add, backend.Name));
                backend.CreateFile(file, this.treeTraverser.GetFileSource(file), lastModified, fileMD5);
                this.Logger.Info("{0}: Added file: {1}", backend.Name, file);
            }
            this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
        }

        private bool alternateFile(int folderId, string file, string fileMD5, DateTime lastModified, bool update, BackendBase backend) {
            // if update is true, we're updating the dest file. otherwise we're adding it
            // Return true if we made use of an alternate, or false if we did nothing
            string logAction = update ? "Updated" : "Added";

            FileDatabase.FileRecord[] alternates = this.fileDatabase.SearchForAlternates(fileMD5);
            if (alternates.Length == 0)
                return false;

            // Now, each alternate may not in fact exist on the backend, or may be changed. We can't assume stuff like this
            // So, loop through them all, and attempt to use it. Bail after the first successful one

            bool foundGoodAlternate = false;

            foreach (FileDatabase.FileRecord alternate in alternates) {
                // First, does it even exist on the backend? Skip to the next one if not
                if (!backend.TestFile(alternate.Path, lastModified, fileMD5))
                    continue;

                // Next: Is is a copy or a move?
                if (this.treeTraverser.FileExists(alternate.Path)) {
                    // It's a copy
                    this.reportBackupAction(new BackupActionItem(alternate.Path, file, BackupActionEntity.File, BackupActionOperation.Copy, backend.Name));
                    if (backend.CreateFromAlternateCopy(file, alternate.Path))
                        this.Logger.Info("{0}: {1} file: {2} from alternate {3} (copy)", backend.Name, logAction, file, alternate.Path);
                    else {
                        backend.CreateFile(file, this.treeTraverser.GetFileSource(file), lastModified, fileMD5);
                        this.Logger.Info("{0}: {1} file: {2} (backend refused alternate {3})", backend.Name, logAction, file, alternate.Path);
                    }
                }
                else {
                    // It's a move
                    this.reportBackupAction(new BackupActionItem(alternate.Path, file, BackupActionEntity.File, BackupActionOperation.Move, backend.Name));
                    backend.CreateFromAlternateMove(file, alternate.Path);
                    this.Logger.Info("{0}: {1} file: {2} from alternate {3} (move)", backend.Name, logAction, file, alternate.Path);
                    this.fileDatabase.DeleteFile(alternate.Id);
                }

                // We're all golden
                foundGoodAlternate = true;
                break;
            }

            return foundGoodAlternate;
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
                //this.Logger.Info("File mtime changed, but file unchanged. Touching: {0}", file);
                return;
            }

            foreach (BackendBase backend in this.backends) {
                if (this.alternateFile(folderId, file, fileMD5, lastModified, true, backend))
                    continue;

                this.reportBackupAction(new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Update, backend.Name));
                if (backend.CreateFile(file, this.treeTraverser.GetFileSource(file), lastModified, fileMD5))
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
                    if (!this.alternateFile(folderId, file, fileMD5, lastModified, true, backend)) {
                        if (backend.CreateFile(file, this.treeTraverser.GetFileSource(file), lastModified, fileMD5))
                            this.Logger.Info("{0}: File on backend missing or modified, so re-creating: {1}", backend.Name, file);
                    }
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

        public void Restore(bool overwrite, bool overwriteOnlyIfOlder, bool purge) {
            // We can only restore from one backend...
            if (this.backends.Length != 1)
                throw new ArgumentException("Cannot perform a restore when more than one backend given");
            BackendBase backend = this.backends[0];

            string fullPath;
            int curFolderId = 0;
            FileDatabase.FolderStatus folderStatus;
            FileDatabase.FileStatusWithLastModified fileStatus;
            DateTime lastModified;
            bool copyFile = false;
            string fileMd5;
            string destMd5;
            DateTime copyFileLastModified;

            foreach (BackendBase.EntityRecord entity in backend.ListFilesFolders()) {
                if (this.Cancelled)
                    return;

                if (entity.Type == BackendBase.Entity.Folder) {
                    this.reportBackupAction(new BackupActionItem(null, entity.Path, BackupActionEntity.Folder, BackupActionOperation.Add));
                    fullPath = this.treeTraverser.GetFolderSource(entity.Path);
                    if (!Directory.Exists(fullPath)) {
                        this.Logger.Info("Restoring folder: {0}", entity.Path);
                        try {
                            Directory.CreateDirectory(fullPath);
                        }
                        catch (IOException e) { this.handleOperationException(new BackupOperationException(entity.Path, e.Message)); }
                    }
                    // Regardless of whether the folder was created or not, we need some data for the file bit
                    // Also, if the folder didn't exist in the database, now's the time to add it
                    folderStatus = this.fileDatabase.InspectFolder(entity.Path);
                    if (folderStatus.Id >= 0)
                        curFolderId = folderStatus.Id;
                    else {
                        curFolderId = this.fileDatabase.AddFolder(entity.Path);
                    }
                }
                else {
                    // Ignore this: we don't want to restore our own backup DB
                    if (entity.Path == Path.GetFileName(Database.FilePath))
                        continue;
                    destMd5 = null;

                    fullPath = this.treeTraverser.GetFileSource(entity.Path);
                    this.reportBackupAction(new BackupActionItem(null, entity.Path, BackupActionEntity.File, BackupActionOperation.Add));
                    if (File.Exists(fullPath)) {
                        lastModified = File.GetLastWriteTimeUtc(fullPath);
                        // Right, now we need to find some info about the file, if we can
                        fileStatus = this.fileDatabase.InspectFileWithLastModified(curFolderId, entity.Path, lastModified);

                        if (overwrite) {
                            switch (fileStatus.FileModStatus) {
                                case FileDatabase.FileModStatus.New:
                                    // No record of the file in the database. Therefore we're copying it if the md5's don't match
                                    copyFile = true;
                                    break;
                                case FileDatabase.FileModStatus.Older:
                                    // File on filesystem is older than in the database. Therefore we're copying it if the md5's don't match
                                    copyFile = true;
                                    break;
                                case FileDatabase.FileModStatus.Newer:
                                case FileDatabase.FileModStatus.Unmodified:
                                    // File on the filesystem is newer or the same age as in the database. Therefore copy only if !overwriteOnlyIfOlder and the md5s don't match
                                    copyFile = !overwriteOnlyIfOlder;
                                    break;
                            }
                        }
                        else {
                            copyFile = false;
                        }

                        if (copyFile) {
                            fileMd5 = fileStatus.MD5 == null ? backend.FileMD5(entity.Path) : fileStatus.MD5;
                            // If it's still null, we have no way of getting the md5 from anywhere, so just copy the damn thing
                            // If not, test the file
                            if (fileMd5 != null) {
                                destMd5 = this.treeTraverser.FileMd5(entity.Path, (percent) => {
                                    this.reportBackupAction(new BackupActionItem(null, entity.Path, BackupActionEntity.File, BackupActionOperation.Hash, null, percent));
                                    return !this.Cancelled;
                                });
                                if (this.Cancelled)
                                    return;
                                if (fileMd5 == destMd5) {
                                    copyFile = false;
                                    this.Logger.Info("Skipping file as identical: {0}", entity.Path);
                                }
                                else
                                    this.Logger.Info("File exists but is being overwritten: {0}", entity.Path);
                            }
                            else
                                this.Logger.Info("File exists but is being overwritten: {0}", entity.Path);
                        }
                    }
                    else {
                        // No file in dest, so automatically copy
                        copyFile = true;
                        // Still need to see if we can get its last modified time, if the record exists at all
                        // We don't care about the modified status, so just feed in anything
                        fileStatus = this.fileDatabase.InspectFileWithLastModified(curFolderId, entity.Path, DateTime.Now);
                        this.Logger.Info("Restoring file: {0}", entity.Path);
                    }

                    // We need to set the utime of the existing/new file.
                    // If we've got a DB record, use that. otherwise ask the backend. Otherwise now
                    // The backend's FileLastModified defaults to DateTime.UtcNow, which is the default we want, so keep that
                    copyFileLastModified = fileStatus.MD5 == null ? backend.FileLastModified(entity.Path) : fileStatus.LastModified;
                    if (!copyFile) {
                        // If the file exists, but we didn't write it, touch it instead
                        if (File.Exists(fullPath))
                            File.SetLastWriteTimeUtc(fullPath, copyFileLastModified);
                    }
                    else {
                        // So. we definitely decided to copy.
                        try {
                            backend.RestoreFile(entity.Path, fullPath, copyFileLastModified);
                        }
                        catch (BackupOperationException e) { this.handleOperationException(e); }
                    }

                    // The only condition we need to update the DB is for insertions, as if the record exists already we match the dest to the DB
                    // Therefore we need a file hash. Yay. 
                    if (fileStatus.MD5 == null) {
                        // If we haven't found the MD5 of the file yet, calculate it now. Look at the file on the dest, as it's likely quicker
                        if (destMd5 == null) {
                            destMd5 = this.treeTraverser.FileMd5(entity.Path, (percent) => {
                                this.reportBackupAction(new BackupActionItem(null, entity.Path, BackupActionEntity.File, BackupActionOperation.Hash, null, percent));
                                return !this.Cancelled;
                            });
                            if (this.Cancelled)
                                return;
                        }

                        this.fileDatabase.AddFile(curFolderId, entity.Path, copyFileLastModified, destMd5);
                    }
                }
            }

            if (purge) {
                // Run off the database for this one, as it'll be up-to-date by now
            }
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
