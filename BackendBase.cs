using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public abstract void CreateFolder(string folder);
        public abstract void DeleteFolder(string folder);
        public abstract bool CreateFile(string file, string source, string fileMD5);
        public abstract bool CreateFromAlternateCopy(string file, string source);
        public abstract void CreateFromAlternateMove(string file, string source);
        public abstract bool FileExists(string file);
        public abstract bool TestFile(string file, DateTime lastModified, string fileMd5);
        public abstract void TouchFile(string file, DateTime lastModified);
        public abstract void DeleteFile(string file);
        public abstract bool FolderExists(string folder);
        public abstract void BackupDatabase(string file, string source);

        public abstract void PurgeFiles(IEnumerable<string> files, IEnumerable<string> folders, PurgeProgressHandler handler=null);

        protected void ReportProcess(int percent) {
            CopyProgress(this, percent);
        }

        public void Cancel() {
            this.Cancelled = true;
        }

        public enum PurgeEntity { File, Folder };
        public delegate bool PurgeProgressHandler(PurgeEntity entity, string file);
    }
}
