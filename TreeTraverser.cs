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
            this.StartDir = startDir.TrimEnd('\\');
            this.substringStart = this.StartDir.Length + 1;
            if (ignoreRules == null)
                this.FileIgnoreRules = null;
            else
                this.FileIgnoreRules = new Regex("^"+String.Join("|", ignoreRules.Split(new char[] { '|' }).Select(x => "("+Regex.Escape(x.Trim()).Replace(@"\*", ".*").Replace(@"\?", ".")+")"))+"$");
        }

        public IEnumerable<FolderEntry> ListFolders(int maxDepth=-1) {
            Stack<FolderEntry> stack = new Stack<FolderEntry>();
            stack.Push(new FolderEntry(this, 0, ""));
            FolderEntry item;

            while (stack.Count > 0) {
                item = stack.Pop();
                yield return item;
                if (maxDepth > 0 && item.Level < maxDepth) {
                    try {
                        foreach (string dir in Directory.EnumerateDirectories(item.FullPath).Reverse().Select(x => x.Substring(this.substringStart))) {
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
            public string RelPath {get; private set; }
            public string FullPath {
                get { return Path.Combine(this.parent.StartDir, this.RelPath); }
            }
            public string Name {
                get { return Path.GetFileName(this.RelPath); }
            }
            
            public FolderEntry(TreeTraverser parent, int level, string relPath) {
                this.parent = parent;
                this.Level = level;
                this.RelPath = relPath;
            }

            public IEnumerable<FileEntry> GetFiles() {
                IEnumerable<string> files = null;
                try {
                    files = Directory.EnumerateFiles(Path.Combine(this.parent.StartDir, this.RelPath));
                }
                catch (DirectoryNotFoundException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }

                if (files != null) {
                    foreach (string file in files.Select(x => x.Substring(this.parent.substringStart))) {
                        if (this.parent.FileIgnoreRules != null && this.parent.FileIgnoreRules.IsMatch(file))
                            continue;
                        yield return new FileEntry(this.parent, file);
                    }
                }
            }

            public IEnumerable<FolderEntry> GetFolders() {
                // This only lists one level, as the technique of recursively calling GetFolders only requires one level
                IEnumerable<string> dirs = null;
                try {
                    dirs = Directory.EnumerateDirectories(this.FullPath);
                }
                catch (DirectoryNotFoundException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }

                if (dirs != null) {
                    foreach (string dir in dirs.Select(x => x.Substring(this.parent.substringStart))) {
                        yield return new FolderEntry(this.parent, this.Level + 1, dir);
                    }
                }
            }
        }

        public class FileEntry
        {
            private TreeTraverser parent;
            public string RelPath { get; private set; }
            public string FullPath {
                get { return Path.Combine(this.parent.StartDir, this.RelPath); }
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
                get { return Path.GetFileName(this.RelPath); }
            }
            public string Extension {
                get { return Path.GetExtension(this.RelPath); }
            }

            public FileEntry(TreeTraverser parent, string relPath) {
                this.parent = parent;
                this.RelPath = relPath;
                this.lastModified = new Lazy<DateTime>(() => File.GetLastWriteTimeUtc(this.FullPath));
            }

            public string GetMD5(FileUtils.HashProgress handler = null) {
                try {
                    return FileUtils.FileMD5(this.FullPath, handler);
                }
                catch (IOException e) { throw new BackupOperationException(this.RelPath, e.Message); }
                catch (UnauthorizedAccessException e) { throw new BackupOperationException(this.RelPath, e.Message); }
            }
        }
    }
}
