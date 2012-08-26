using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackerUpper
{
    interface IBackend
    {
        string Name{ get; }
        void SetupInitial();
        void CreateFile(string file, string source, string fileMD5);
        void UpdateFile(string file, string source, string fileMD5);
        void DeleteFile(string file);
        bool CreateFromAlternateCopy(string file, string source);
        void CreateFromAlternateMove(string file, string source);
        bool FileExists(string file);
        bool TestFile(string file, DateTime lastModified, string fileMd5);
        void CreateFolder(string folder);
        void DeleteFolder(string folder);
        bool FolderExists(string folder);
        void BackupDatabase(string file, string source);

        void PurgeFiles(IEnumerable<string> files, IEnumerable<string> folders);
    }
}
