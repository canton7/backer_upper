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
        void CreateFromAlternateCopy(string file, string source);
        void CreateFromAlternateMove(string file, string source);
        bool FileExists(string file);
        void CreateFolder(string folder);
        void DeleteFolder(string folder);
        bool FolderExists(string folder);

        void PurgeFiles(IEnumerable<string> files, IEnumerable<string> folders);
    }
}
