using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackerUpper
{
    class MirrorBackend : BackendBase, IBackend
    {
        public override string Name {
            get { return "Mirror"; }
        }

        public MirrorBackend(string dest)
            : base(dest) {
        }

        public override void SetupInitial() {
            // Don't need to do anything
        }

        public override void CreateFolder(string folder) {
            this.withHandling(() => Directory.CreateDirectory(Path.Combine(this.Dest, folder)), folder);
        }

        public override void DeleteFolder(string folder) {
            this.withHandling(() => Directory.Delete(Path.Combine(this.Dest, folder)), folder);
        }

        public override bool FolderExists(string folder) {
            return Directory.Exists(Path.Combine(this.Dest, folder));
        }

        public override void CreateFile(string file, string source, string fileMD5) {
            // If fileMD5 passed, check whether file already exists with this hash. If so, don't copy
            string dest = Path.Combine(this.Dest, file);

            try {
                if (File.Exists(dest)) {
                    string destMD5 = FileUtils.FileMD5(dest);
                    if (destMD5 == fileMD5)
                        return;
                }
            }
            catch (IOException e) { throw new BackupOperationException(dest, e.Message); }

            this.withHandling(() => XCopy.Copy(source, dest, true, true, (percent) => {
                this.ReportProcess(percent);
                return this.Cancelled ? XCopy.CopyProgressResult.PROGRESS_CANCEL : XCopy.CopyProgressResult.PROGRESS_CONTINUE;
            }), file);
            if (this.Cancelled)
                return;
            FileInfo fileInfo = new FileInfo(dest);
            fileInfo.IsReadOnly = false;
        }

        public override void UpdateFile(string file, string source, string fileMD5) {
            this.withHandling(() => XCopy.Copy(source, Path.Combine(this.Dest, file), true, true, (percent) => {
                this.ReportProcess(percent);
                return this.Cancelled ? XCopy.CopyProgressResult.PROGRESS_CANCEL : XCopy.CopyProgressResult.PROGRESS_CONTINUE;
            }), file);
        }

        public override void DeleteFile(string file) {
            this.withHandling(() => File.Delete(Path.Combine(this.Dest, file)), file);
        }

        public override bool FileExists(string file) {
            return File.Exists(Path.Combine(this.Dest, file));
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

        public override void PurgeFiles(IEnumerable<string> filesIn, IEnumerable<string> foldersIn) {
            HashSet<string> files = new HashSet<string>(filesIn);
            HashSet<string> folders = new HashSet<string>(foldersIn);
            // Just use a TreeTraverser here
            TreeTraverser treeTraverser = new TreeTraverser(this.Dest);
            TreeTraverser.FolderEntry folder = treeTraverser.FirstFolder();

            string[] filesInDir;

            while (folder.Level >= 0) {
                try {
                    if (folders.Contains(folder.Name)) {
                        filesInDir = treeTraverser.ListFiles(folder.Name);
                        foreach (string file in filesInDir) {
                            if (!files.Contains(file)) {
                                File.Delete(treeTraverser.GetFileSource(file));
                            }
                        }
                    }
                    else {
                        Directory.Delete(treeTraverser.GetFolderSource(folder.Name), true);
                    }

                    folder = treeTraverser.NextFolder(true);
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
        }
    }
}
