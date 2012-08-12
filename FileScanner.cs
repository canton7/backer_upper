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
        private BackendBase backend;
        public Database Database;

        public delegate void BackupActionEventHandler(object sender, BackupActionItem item);
        public event BackupActionEventHandler BackupAction;

        public FileScanner(string startDir, Database database, BackendBase backend) {
            this.startDir = startDir;
            this.Database = database;
            this.treeTraverser = new TreeTraverser(startDir);
            this.fileDatabase = new FileDatabase(database);
            this.backend = backend;
        }

        public void PruneDatabase() {
            // Remove all database entries where the file doesn't exist on the disk
            // This is an inconsistency we can't recover from otherwise, as we never check whether we need to re-copy the file

            // Start with the files
            FileDatabase.FileRecord[] files = this.fileDatabase.RecordedFiles();
            foreach (FileDatabase.FileRecord file in files) {
                if (!this.backend.FileExists(file.Path)) {
                    this.fileDatabase.DeleteFile(file.Id);
                }
            }

            // Then the folders
            FileDatabase.FolderRecord[] folders = this.fileDatabase.RecordedFolders();
            foreach (FileDatabase.FolderRecord folder in folders) {
                if (!this.backend.FolderExists(folder.Path)) {
                    this.fileDatabase.DeleteFolder(folder.Id);
                }
            }
        }

        public void PurgeDest() {
            // Remove all entries in the destination filesystem which aren't in the database or the source filesystem
            // WARNING: Could potentially destroy information
            IEnumerable<string> files = this.fileDatabase.RecordedFiles().Select(x => x.Path);
            IEnumerable<string> folders = this.fileDatabase.RecordedFolders().Select(x => x.Path);

            this.backend.PurgeFiles(files, folders);
        }

        public void Backup() {
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

            while (folder.Level >= 0) {
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
                            nextFolder = true;
                            curFolderId = folderStatus.Id;
                            break;
                    }
                }

                // Check for files in this folder
                files = this.treeTraverser.ListFiles(folder.Name);
                foreach (string file in files) {
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
                            this.BackupAction(this, new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Nothing));
                            break;
                    }
                }

                // Move onto the next folder
                prevLevel = folder.Level;
                folder = this.treeTraverser.NextFolder(nextFolder);
            }

            // Now we look for file deletions
            FileDatabase.FileRecord[] recordedFiles = this.fileDatabase.RecordedFiles();
            foreach (FileDatabase.FileRecord fileToCheck in recordedFiles) {
                if (!this.treeTraverser.FileExists(fileToCheck.Path)) {
                    this.deleteFile(fileToCheck.Id, fileToCheck.Path);
                }
            }

            // And finally folder deletions
            FileDatabase.FolderRecord[] recordedFolders = this.fileDatabase.RecordedFolders();
            foreach (FileDatabase.FolderRecord folderToCheck in recordedFolders) {
                if (!this.treeTraverser.FolderExists(folderToCheck.Path)) {
                    this.deleteFolder(folderToCheck.Id, folderToCheck.Path);
                }
            }

            // Sync the database back to disk
            this.fileDatabase.SyncToDisk();
        }

        private int addFolder(string folder) {
            this.backend.CreateFolder(folder);
            int insertedId = this.fileDatabase.AddFolder(folder);
            this.BackupAction(this, new BackupActionItem(null, folder, BackupActionEntity.Folder, BackupActionOperation.Add));
            return insertedId;
        }

        private void deleteFolder(int folderId, string folder) {
            this.backend.DeleteFolder(folder);
            this.fileDatabase.DeleteFolder(folderId);
            this.BackupAction(this, new BackupActionItem(null, folder, BackupActionEntity.Folder, BackupActionOperation.Delete));
        }

        private void addFile(int folderId, string file, DateTime lastModified) {
            string fileMD5 = this.treeTraverser.FileMd5(file);
            // Just do a search for alternates (files in a different place on the remote location with the same hash)
            // as this will speed up copying
            FileDatabase.FileRecord alternate = this.fileDatabase.SearchForAlternates(fileMD5);
            if (alternate.Id > 0) {
                // Aha!
                this.alternateFile(folderId, file, fileMD5, lastModified, alternate.Path, alternate.Id);
                return;
            }

            this.backend.CreateFile(file, this.treeTraverser.GetFileSource(file), fileMD5);
            this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);

            this.BackupAction(this, new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Add));
        }

        private void alternateFile(int folderId, string file, string fileMD5, DateTime lastModified, string alternatePath, int alternateId) {
            // First: Is is a copy or a move?
            if (this.treeTraverser.FileExists(alternatePath)) {
                // It's a copy
                this.backend.CreateFromAlternateCopy(file, alternatePath);
                this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
                this.BackupAction(this, new BackupActionItem(alternatePath, file, BackupActionEntity.File, BackupActionOperation.Copy));
            }
            else {
                // It's a move
                this.backend.CreateFromAlternateMove(file, alternatePath);
                this.fileDatabase.AddFile(folderId, file, lastModified, fileMD5);
                this.fileDatabase.DeleteFile(alternateId);
                this.BackupAction(this, new BackupActionItem(alternatePath, file, BackupActionEntity.File, BackupActionOperation.Move));
            }
        }

        private void updatefile(int folderId, string file, string remoteMD5, DateTime lastModified) {
            // Only copy if the file has actually changed
            string fileMD5 = this.treeTraverser.FileMd5(file);
            if (remoteMD5 == fileMD5) {
                this.backend.UpdateFile(file, this.treeTraverser.GetFileSource(file));
            }
            // But update the last modified time either way
            this.fileDatabase.UpdateFile(folderId, file, lastModified, fileMD5);
            this.BackupAction(this, new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Update));
        }

        private void deleteFile(int fileId, string file) {
            this.backend.DeleteFile(file);
            this.fileDatabase.DeleteFile(fileId);
            this.BackupAction(this, new BackupActionItem(null, file, BackupActionEntity.File, BackupActionOperation.Delete));
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
        public enum BackupActionOperation { Add, Delete, Copy, Move, Update, Nothing };
        public struct BackupActionItem
        {
            public string From;
            public string To;
            public BackupActionEntity Entity;
            public BackupActionOperation Operation;

            public BackupActionItem(string from, string to, BackupActionEntity entity, BackupActionOperation operation) {
                this.From = from;
                this.To = to;
                this.Entity = entity;
                this.Operation = operation;
            }
        }


    }
}
