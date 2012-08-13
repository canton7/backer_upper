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
            Directory.CreateDirectory(Path.Combine(this.Dest, folder));
        }

        public override void DeleteFolder(string folder) {
            Directory.Delete(Path.Combine(this.Dest, folder));
        }

        public override bool FolderExists(string folder) {
            return Directory.Exists(Path.Combine(this.Dest, folder));
        }

        public override void CreateFile(string file, string source, string fileMD5) {
            // If fileMD5 passed, check whether file already exists with this hash. If so, don't copy
            string dest = Path.Combine(this.Dest, file);

            if (File.Exists(dest)) {
                string destMD5 = FileUtils.FileMD5(dest);
                if (destMD5 == fileMD5)
                    return;
            }

            File.Copy(source, dest, true);
            FileInfo fileInfo = new FileInfo(dest);
            fileInfo.IsReadOnly = false;
        }

        public override void UpdateFile(string file, string source, string fileMD5) {
            File.Copy(source, Path.Combine(this.Dest, file), true);
        }

        public override void DeleteFile(string file) {
            File.Delete(Path.Combine(this.Dest, file));
        }

        public override bool FileExists(string file) {
            return File.Exists(Path.Combine(this.Dest, file));
        }

        public override void CreateFromAlternateCopy(string file, string source) {
            File.Copy(Path.Combine(this.Dest, source), Path.Combine(this.Dest, file), true);
        }

        public override void CreateFromAlternateMove(string file, string source) {
            File.Move(Path.Combine(this.Dest, source), Path.Combine(this.Dest, file));
        }

        public override void PurgeFiles(IEnumerable<string> filesIn, IEnumerable<string> foldersIn) {
            HashSet<string> files = new HashSet<string>(filesIn);
            HashSet<string> folders = new HashSet<string>(foldersIn);
            // Just use a TreeTraverser here
            TreeTraverser treeTraverser = new TreeTraverser(this.Dest);
            TreeTraverser.FolderEntry folder = treeTraverser.FirstFolder();

            string[] filesInDir;

            while (folder.Level >= 0) {
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
        }
    }
}
