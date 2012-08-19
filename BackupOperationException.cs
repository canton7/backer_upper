using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackerUpper
{
    class BackupOperationException : Exception
    {
        public BackupOperationException(string fileOrFolder, string message) : base(String.Format("{0}: {1}", fileOrFolder, message.Trim())) { }
    }
}
