using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackerUpper
{
    static class Constants
    {
        public static string APPDATA {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
        }
        public const string HELP_URL = "https://github.com/canton7/backer_upper#backer-upper";
        public const string GITHUB_DOWNLOADS_URL = "https://api.github.com/repos/canton7/backer_upper/downloads";
        public const string VERSION_FILE = "last_alerted_version.txt";

        public static string APPDATA_FOLDER {
            get { return Path.Combine(APPDATA, "BackerUpper"); }
        }

        public static string BACKUPS_FOLDER {
            get { return Path.Combine(APPDATA_FOLDER, "Backups"); }
        }

        public const string BACKUP_EXTENSION = ".baup";

        public static string LOG_FOLDER {
            get { return Path.Combine(APPDATA_FOLDER, "Logs"); }
        }

        public const string TASK_SCHEDULER_FOLDER = "BackerUpper";
        public const int LOGS_KEEP_NUM = 5;
        public const string AWS_CREDENTIALS_URL = "http://aws.amazon.com/security-credentials";
    }
}
