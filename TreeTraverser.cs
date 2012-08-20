using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackerUpper
{
    class TreeTraverser
    {
        private string startDir;
        private Stack<FolderEntry> stack;
        private int substringStart;

        public TreeTraverser(string startDir) {
            this.startDir = startDir;
            this.stack = new Stack<FolderEntry>();
            this.substringStart = this.startDir.Length + 1;
        }

        public FolderEntry FirstFolder() {
            this.stack.Push(new FolderEntry(0, this.startDir));

            return new FolderEntry(0, "");
        }

        public FolderEntry NextFolder(bool further) {
            FolderEntry folderEntry = this.stack.Pop();
            if (further) {
                // Directories can disappear from under our feet. Just carry on
                try {
                    foreach (string child in Directory.GetDirectories(folderEntry.Name)) {
                        this.stack.Push(new FolderEntry(folderEntry.Level + 1, child));
                    }
                }
                catch (IOException e) { throw new BackupOperationException(folderEntry.Name, e.Message); }
                catch (UnauthorizedAccessException e) { throw new BackupOperationException(folderEntry.Name, e.Message); }
            }

            if (this.stack.Count == 0) {
                return new FolderEntry(-1, null);
            }

            FolderEntry folderToReturn = this.stack.Peek();
            return new FolderEntry(folderToReturn.Level, folderToReturn.Name.Substring(this.substringStart));
        }

        public string[] ListFiles(string folder) {
            try {
                return Directory.GetFiles(Path.Combine(this.startDir, folder)).Select(x => x.Substring(this.substringStart)).ToArray();
            }
            catch (IOException e) { throw new BackupOperationException(folder, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(folder, e.Message); }
        }

        public string GetFileSource(string file) {
            return Path.Combine(this.startDir, file);
        }

        public string GetFolderSource(string folder) {
            return Path.Combine(this.startDir, folder);
        }

        public DateTime GetFileLastModified(string file) {
            try {
                return File.GetLastWriteTime(Path.Combine(this.startDir, file));
            }
            catch (IOException e) { throw new BackupOperationException(file, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(file, e.Message); }
        }

        public string FileMd5(string fileName) {
            try {
                return FileUtils.FileMD5(Path.Combine(this.startDir, fileName));
            }
            catch (IOException e) { throw new BackupOperationException(fileName, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(fileName, e.Message); }
        }

        public bool FileExists(string file) {
            return File.Exists(Path.Combine(this.startDir, file));
        }

        public bool FolderExists(string path) {
            return Directory.Exists(Path.Combine(this.startDir, path));
        }

        public struct FolderEntry
        {
            public int Level;
            public string Name;
            public FolderEntry(int level, string name) {
                this.Level = level;
                this.Name = name;
            }
        }
    }
}
