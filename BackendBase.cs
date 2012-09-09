using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackerUpper
{
    abstract class BackendBase
    {
        public string Dest;
        public abstract string Name { get; }
        public abstract bool StripFilesFoldersOnDBBackup { get; }
        public bool Cancelled { get; private set; }

        public delegate void CopyProgressEventHandler(object sender, int percent);
        public event CopyProgressEventHandler CopyProgress;

        public BackendBase(string dest) {
            this.Dest = dest;
            this.Cancelled = false;
        }

        // This is the place to make sure the dest actually exists, etc
        public abstract void SetupInitial();
        public abstract void CreateFolder(string folder, FileAttributes attributes);
        public abstract void DeleteFolder(string folder);
        public abstract void RestoreFolder(string folder, string dest);
        public abstract bool CreateFile(string file, string source, DateTime lastModified, string fileMD5, FileAttributes atttributes, bool reportProgress=false);
        public abstract bool CreateFromAlternateCopy(string file, string source);
        public abstract void CreateFromAlternateMove(string file, string source);
        public abstract bool FileExists(string file);
        public abstract string FileMD5(string file);
        public abstract DateTime FileLastModified(string file);
        public abstract bool TestFile(string file, DateTime lastModified, string fileMd5);
        public abstract void TouchFile(string file, DateTime lastModified);
        public abstract void DeleteFile(string file);
        public abstract void RestoreFile(string file, string dest, DateTime lastModified);
        public abstract bool FolderExists(string folder);
        public abstract void BackupDatabase(string file, string source);
        public abstract IEnumerable<EntityRecord> ListFilesFolders();
        public abstract void PurgeFiles(IEnumerable<string> files, IEnumerable<string> folders, PurgeProgressHandler handler=null);

        protected void ReportProcess(int percent) {
            CopyProgress(this, percent);
        }

        public void Cancel() {
            this.Cancelled = true;
        }

        public enum Entity { File, Folder };
        public struct EntityRecord
        {
            public string Path;
            public Entity Type;

            public EntityRecord(string path, Entity type) {
                this.Path = path;
                this.Type = type;
            }
        }
        public delegate bool PurgeProgressHandler(Entity entity, string file, bool deleted);
    }
}
