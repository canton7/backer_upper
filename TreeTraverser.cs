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
            this.stack.Clear();
            this.stack.Push(new FolderEntry(0, this.startDir));

            return new FolderEntry(0, "");
        }

        public FolderEntry NextFolder(bool further) {
            FolderEntry folderEntry = this.stack.Pop();
            if (further) {
                // Directories can disappear from under our feet. Just carry on
                try {
                    string[] directories = Directory.GetDirectories(folderEntry.Name);
                    Array.Sort(directories);
                    // Reverse, so we put the highest in the alphabet on last, so it gets popped first
                    foreach (string child in directories.Reverse()) {
                        this.stack.Push(new FolderEntry(folderEntry.Level + 1, child));
                    }
                }
                // We CAN NOT exit now, as we won't return a new folder, and stuff breaks badly.
                // We'll already get a log entry from when we try and find files in this folder, so stay with that
                catch (DirectoryNotFoundException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            if (this.stack.Count == 0) {
                return new FolderEntry(-1, null);
            }

            FolderEntry folderToReturn = this.stack.Peek();
            return new FolderEntry(folderToReturn.Level, folderToReturn.Name.Substring(this.substringStart));
        }

        public IEnumerable<string> ListFiles(string folder) {
            try {
                return Directory.GetFiles(Path.Combine(this.startDir, folder)).Select(x => x.Substring(this.substringStart));
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
                return File.GetLastWriteTimeUtc(Path.Combine(this.startDir, file));
            }
            catch (IOException e) { throw new BackupOperationException(file, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(file, e.Message); }
        }

        public string FileMd5(string fileName, FileUtils.HashProgress handler = null) {
            try {
                return FileUtils.FileMD5(Path.Combine(this.startDir, fileName), handler);
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

        public void DeleteFile(string file) {
            try {
                File.Delete(Path.Combine(this.startDir, file));
            }
            catch (IOException e) { throw new BackupOperationException(file, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(file, e.Message); }
        }

        public void DeleteFolder(string path) {
            try {
                Directory.Delete(Path.Combine(this.startDir, path), true);
            }
            catch (IOException e) { throw new BackupOperationException(path, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(path, e.Message); }
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
