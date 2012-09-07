using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace BackerUpper
{
    class TreeTraverser
    {
        public string StartDir{ get; private set; }
        private int substringStart;
        public Regex FileIgnoreRules { get; private set; }

        public TreeTraverser(string startDir, string ignoreRules=null) {
            this.StartDir = startDir;
            this.substringStart = this.StartDir.Length + 1;
            if (ignoreRules == null)
                this.FileIgnoreRules = null;
            else
                this.FileIgnoreRules = new Regex("^"+String.Join("|", ignoreRules.Split(new char[] { '|' }).Select(x => "("+Regex.Escape(x.Trim()).Replace(@"\*", ".*").Replace(@"\?", ".")+")"))+"$");
        }

        public IEnumerable<FolderEntry> ListFolders() {
            Stack<FolderEntry> stack = new Stack<FolderEntry>();
            stack.Push(new FolderEntry(this, 0, ""));
            FolderEntry item;

            while (stack.Count > 0) {
                item = stack.Pop();
                yield return item;
                try {
                    foreach (string dir in Directory.EnumerateDirectories(item.FullPath).Reverse().Select(x => x.Substring(this.StartDir.Length + 1))) {
                        stack.Push(new FolderEntry(this, item.Level + 1, dir));
                    }
                }
                // We CAN NOT exit now, as we won't return a new folder, and stuff breaks badly.
                // We'll already get a log entry from when we try and find files in this folder, so stay with that
                catch (DirectoryNotFoundException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        public FolderEntry CreateFolderEntry(string path) {
            return new FolderEntry(this, 0, path);
        }

        public FileEntry CreateFileEntry(string file) {
            return new FileEntry(this, file);
        }

        public bool FileExists(string file) {
            return File.Exists(Path.Combine(this.StartDir, file));
        }

        public bool FolderExists(string path) {
            return Directory.Exists(Path.Combine(this.StartDir, path));
        }

        public void DeleteFile(string file) {
            try {
                File.Delete(Path.Combine(this.StartDir, file));
            }
            catch (IOException e) { throw new BackupOperationException(file, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(file, e.Message); }
        }

        public void DeleteFolder(string path) {
            try {
                Directory.Delete(Path.Combine(this.StartDir, path), true);
            }
            catch (IOException e) { throw new BackupOperationException(path, e.Message); }
            catch (UnauthorizedAccessException e) { throw new BackupOperationException(path, e.Message); }
        }

        public class FolderEntry
        {
            private TreeTraverser parent;
            public int Level {get; private set; }
            public string Name {get; private set; }
            public string FullPath {
                get { return Path.Combine(this.parent.StartDir, this.Name); }
            }
            
            public FolderEntry(TreeTraverser parent, int level, string name) {
                this.parent = parent;
                this.Level = level;
                this.Name = name;
            }

            public IEnumerable<FileEntry> GetFiles() {
                foreach (string file in Directory.EnumerateFiles(Path.Combine(this.parent.StartDir, this.Name)).Select(x => x.Substring(this.parent.StartDir.Length + 1))) {
                    if (this.parent.FileIgnoreRules != null && this.parent.FileIgnoreRules.IsMatch(file))
                        continue;
                    yield return new FileEntry(this.parent, file);
                }
            }
        }

        public class FileEntry
        {
            private TreeTraverser parent;
            public string Name { get; private set; }
            public string FullPath {
                get { return Path.Combine(this.parent.StartDir, this.Name); }
            }
            private Lazy<DateTime> lastModified;
            public DateTime LastModified {
                get { return this.lastModified.Value; }
                set {
                    // WARNING doesn't update lazy
                    File.SetLastWriteTimeUtc(this.FullPath, value);
                }
            }
            public string Filename {
                get { return Path.GetFileName(this.Name); }
            }

            public FileEntry(TreeTraverser parent, string name) {
                this.parent = parent;
                this.Name = name;
                this.lastModified = new Lazy<DateTime>(() => File.GetLastWriteTimeUtc(this.FullPath));
            }

            public string GetMD5(FileUtils.HashProgress handler = null) {
                try {
                    return FileUtils.FileMD5(this.FullPath, handler);
                }
                catch (IOException e) { throw new BackupOperationException(this.Name, e.Message); }
                catch (UnauthorizedAccessException e) { throw new BackupOperationException(this.Name, e.Message); }
            }
        }
    }
}
