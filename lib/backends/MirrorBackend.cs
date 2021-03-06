﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackerUpper
{
    class MirrorBackend : BackendBase
    {
        public override string Name {
            get { return "Mirror"; }
        }

        public override bool StripFilesFoldersOnDBBackup {
            get { return false; }
        }

        public MirrorBackend(string dest)
            : base(dest) {
        }

        public override void SetupInitial() {
            if (!Directory.Exists(this.Dest))
                throw new IOException("Destination folder "+this.Dest+" doesn't exist!");
        }

        public override void CreateFolder(string folder, FileAttributes attributes) {
            this.withHandling(() => {
                DirectoryInfo destInfo = Directory.CreateDirectory(Path.Combine(this.Dest, folder));
                destInfo.Attributes = attributes;
            }, folder);
        }

        public override void DeleteFolder(string folder) {
            this.withHandling(() => Directory.Delete(Path.Combine(this.Dest, folder)), folder);
        }

        public override bool FolderExists(string folder) {
            return Directory.Exists(Path.Combine(this.Dest, folder));
        }

        public override void RestoreFolder(string folder, string dest) {
            this.withHandling(() => {
                DirectoryInfo info = Directory.CreateDirectory(dest);
                DirectoryInfo sourceInfo = new DirectoryInfo(Path.Combine(this.Dest, folder));
                info.Attributes = sourceInfo.Attributes;
            }, folder);
        }

        public override bool CreateFile(string file, string source, DateTime lastModified, string fileMD5, FileAttributes attributes, bool reportProgress=true) {
            string dest = Path.Combine(this.Dest, file);

            // It's almost always quicker just to re-copy the file, rather than checking
            // md5 of remote file. With USB, read/write times are the same. With internal
            // HDD, times don't differ by enough to justify md5 computational overhead. Plus other overheads
            // for small files... 

            this.withHandling(() => XCopy.Copy(source, dest, true, true, (percent) => {
                if (reportProgress)
                    this.ReportProcess(percent);
                return this.Cancelled ? XCopy.CopyProgressResult.PROGRESS_CANCEL : XCopy.CopyProgressResult.PROGRESS_CONTINUE;
            }), file);
            if (this.Cancelled)
                return true;
            FileInfo fileInfo = new FileInfo(dest);
            fileInfo.Attributes = attributes;
            fileInfo.IsReadOnly = false;
            this.withHandling(() => File.SetLastWriteTimeUtc(dest, lastModified), file);
            return true;
        }

        public override void BackupDatabase(string file, string source) {
            this.CreateFile(Path.Combine(this.Dest, file), source, DateTime.UtcNow, null, new FileInfo(source).Attributes, false);
        }

        public override void DeleteFile(string file) {
            this.withHandling(() => File.Delete(Path.Combine(this.Dest, file)), file);
        }

        public override bool TestFile(string file, DateTime lastModified, string fileMd5) {
            // Don't bother testing the MD% (too slow); just look at the last modified
            if (!this.FileExists(file))
                return false;
            try {
                // Remove milliseconds
                DateTime fileLastMod = File.GetLastWriteTimeUtc(Path.Combine(this.Dest, file));
                return Math.Abs((fileLastMod - lastModified).TotalSeconds) < 1;
            }
            catch (IOException e) {
                throw new BackupOperationException(file, e.Message);
            }
        }

        public override void TouchFile(string file, DateTime lastModified) {
            this.withHandling(() => File.SetLastWriteTimeUtc(Path.Combine(this.Dest, file), lastModified), file);
        }

        public override void RestoreFile(string file, string dest, DateTime lastModified) {
            string fullPath = Path.Combine(this.Dest, file);
            
            if (!File.Exists(fullPath))
                throw new BackupOperationException(file, "File could not be found on backend, so can't restore");

            this.withHandling(() => XCopy.Copy(fullPath, dest, true, true, (percent) => {
                this.ReportProcess(percent);
                return this.Cancelled ? XCopy.CopyProgressResult.PROGRESS_CANCEL : XCopy.CopyProgressResult.PROGRESS_CONTINUE;
            }), file);
            if (!this.Cancelled) {
                this.withHandling(() => {
                    File.SetLastWriteTimeUtc(dest, lastModified);
                    FileInfo fileInfo = new FileInfo(dest);
                    fileInfo.Attributes = new FileInfo(fullPath).Attributes;
                }, file);
            }
        }

        public override bool FileExists(string file) {
            return File.Exists(Path.Combine(this.Dest, file));
        }

        public override string FileMD5(string file) {
            // We don't hold this sort of information in the backend, so return null
            return null;
        }

        public override DateTime FileLastModified(string file) {
            // Default to now
            string path = Path.Combine(this.Dest, file);
            if (!File.Exists(path))
                return DateTime.UtcNow;
            return File.GetLastWriteTimeUtc(path);
        }

        public override bool CreateFromAlternateCopy(string file, string source) {
            // Copy time unaffected by whether are on the same drive; More by how many drives are USB, etc
            return false;
        }

        public override void CreateFromAlternateMove(string file, string source) {
            string dest = Path.Combine(this.Dest, file);
            if (File.Exists(dest))
                this.withHandling(() => File.Delete(dest), dest);
            this.withHandling(() => File.Move(Path.Combine(this.Dest, source), dest), file);
        }

        public override IEnumerable<EntityRecord> ListFilesFolders() {
            // Just use a TreeTraverser here
            TreeTraverser treeTraverser = new TreeTraverser(this.Dest);

            foreach(TreeTraverser.FolderEntry folder in treeTraverser.ListFolders()) {
                yield return new EntityRecord(folder.RelPath, Entity.Folder);
                foreach (TreeTraverser.FileEntry file in folder.GetFiles()) {
                    yield return new EntityRecord(file.RelPath, Entity.File);
                }
            }
        }

        public override void PurgeFiles(IEnumerable<string> filesIn, IEnumerable<string> foldersIn, PurgeProgressHandler handler=null) {
            HashSet<string> files = new HashSet<string>(filesIn);
            HashSet<string> folders = new HashSet<string>(foldersIn);
            // Just use a TreeTraverser here
            TreeTraverser treeTraverser = new TreeTraverser(this.Dest);

            foreach (TreeTraverser.FolderEntry folder in treeTraverser.ListFolders()) {
                try {
                    if (folders.Contains(folder.RelPath)) {
                        if (handler != null && !handler(Entity.Folder, folder.RelPath, false))
                            return;
                        foreach (TreeTraverser.FileEntry file in folder.GetFiles()) {
                            if (!files.Contains(file.RelPath)) {
                                File.Delete(file.FullPath);
                                if (handler != null && !handler(Entity.File, file.RelPath, true))
                                    return;
                            }
                            else {
                                if (handler != null && !handler(Entity.File, file.RelPath, false))
                                    return;
                            }
                        }
                    }
                    else {
                        Directory.Delete(folder.FullPath);
                        if (handler != null && !handler(Entity.Folder, folder.RelPath, true))
                            return;
                    }
                }
                // Don't yet have a way of logging from in here
                catch (BackupOperationException) { }
            }
        }

        private void withHandling(Action action, string errorFile) {
            try {
                action();
            }
            catch (IOException e) { throw new BackupOperationException(errorFile, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(errorFile, e.Message); }
            // Can get these from XCopy
            catch (System.ComponentModel.Win32Exception e) { throw new BackupOperationException(errorFile, e.Message); }
        }
    }
}
