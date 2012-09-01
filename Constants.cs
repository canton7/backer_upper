using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackerUpper
{
    static class Constants
    {
        public const string APPDATA_FOLDER = "BackerUpper";
        public const string BACKUPS_FOLDER = "Backups";
        public const string BACKUP_EXTENSION = ".sqlite";
        public const string LOG_FOLDER = "Logs";
        public const string TASK_SCHEDULER_FOLDER = "BackerUpper";
        public const int LOGS_KEEP_NUM = 5;
        public const string AWS_CREDENTIALS_URL = "http://aws.amazon.com/security-credentials";
    }
}
