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
        private int substringStart;

        public TreeTraverser(string startDir) {
            this.startDir = startDir;
            this.substringStart = this.startDir.Length + 1;
        }

        public IEnumerable<FolderEntry> ListFolders() {
            Stack<FolderEntry> stack = new Stack<FolderEntry>();
            stack.Push(new FolderEntry(this.startDir, 0, ""));
            FolderEntry item;

            while (stack.Count > 0) {
                item = stack.Pop();
                yield return item;
                try {
                    foreach (string dir in Directory.EnumerateDirectories(item.FullPath).Select(x => x.Substring(this.startDir.Length + 1))) {
                        stack.Push(new FolderEntry(this.startDir, item.Level + 1, dir));
                    }
                }
                // We CAN NOT exit now, as we won't return a new folder, and stuff breaks badly.
                // We'll already get a log entry from when we try and find files in this folder, so stay with that
                catch (DirectoryNotFoundException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
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

        public class FolderEntry
        {
            private string startDir;
            public int Level {get; private set; }
            public string Name {get; private set; }
            public string FullPath {
                get { return Path.Combine(this.startDir, this.Name); }
            }
            
            public FolderEntry(string startDir, int level, string name) {
                this.startDir = startDir;
                this.Level = level;
                this.Name = name;
            }

            public IEnumerable<string> GetFiles() {
                return Directory.EnumerateFiles(Path.Combine(this.startDir, this.Name)).Select(x => x.Substring(this.startDir.Length + 1));
            }
        }
    }
}
