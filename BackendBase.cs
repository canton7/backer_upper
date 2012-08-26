using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackerUpper
{
    abstract class BackendBase : IBackend
    {
        public string Dest;
        public abstract string Name { get; }
        public abstract bool StripFilesFoldersOnDBBackup { get; }
        private bool cancelled;
        public bool Cancelled {
            get { return this.cancelled; }
        }

        public delegate void CopyProgressEventHandler(object sender, int percent);
        public event CopyProgressEventHandler CopyProgress;

        public BackendBase(string dest) {
            this.Dest = dest;
            this.cancelled = false;
        }

        public abstract void SetupInitial();
        public abstract void CreateFolder(string folder);
        public abstract void DeleteFolder(string folder);
        public abstract void CreateFile(string file, string source, string fileMD5);
        public abstract bool CreateFromAlternateCopy(string file, string source);
        public abstract void CreateFromAlternateMove(string file, string source);
        public abstract bool FileExists(string file);
        public abstract void UpdateFile(string file, string source, string fileMD5);
        public abstract void DeleteFile(string file);
        public abstract bool FolderExists(string folder);

        public abstract void PurgeFiles(IEnumerable<string> files, IEnumerable<string> folders);

        protected void ReportProcess(int percent) {
            CopyProgress(this, percent);
        }

        public void Cancel() {
            this.cancelled = true;
        }
    }
}
