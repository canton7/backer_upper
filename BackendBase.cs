﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackerUpper
{
    abstract class BackendBase : IBackend
    {
        public string Dest;

        public BackendBase(string dest) {
            this.Dest = dest;
        }

        public abstract void CreateFolder(string folder);
        public abstract void DeleteFolder(string folder);
        public abstract void CreateFile(string file, string source, string fileMD5);
        public abstract void CreateFromAlternateCopy(string file, string source);
        public abstract void CreateFromAlternateMove(string file, string source);
        public abstract bool FileExists(string file);
        public abstract void UpdateFile(string file, string source);
        public abstract void DeleteFile(string file);
        public abstract bool FolderExists(string folder);

        public abstract void PurgeFiles(IEnumerable<string> files, IEnumerable<string> folders);
    }
}
