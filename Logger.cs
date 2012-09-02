using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BackerUpper
{
    class Logger
    {
        public enum Level { DEBUG, INFO, WARN, ERROR };

        private StreamWriter writer;
        private readonly Dictionary<Level, string> levelStrings = new Dictionary<Level,string>(){ 
            {Level.DEBUG, "Debug"},
            {Level.INFO, null},
            {Level.WARN, "Warning"},
            {Level.ERROR, "ERROR"},
        };

        public string LogFilePath { get; private set; }

        public Logger(string type) {
            string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPDATA_FOLDER, Constants.LOG_FOLDER);
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);
            string fileName = String.Format("{0}-{1:yyyyMMddTHHmmss}.log", type, DateTime.Now);
            this.LogFilePath = Path.Combine(logsPath, fileName);
            this.writer = new StreamWriter(this.LogFilePath);
            this.writer.AutoFlush = true;

            this.writer.WriteLine("To find warnings or errors, search for [{0}] or [{1}]\n", levelStrings[Level.WARN], levelStrings[Level.ERROR]);
        }

        public static void Purge() {
            // Keep only a certain number of each type of backup
            string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPDATA_FOLDER, Constants.LOG_FOLDER);
            // If there's no folder, there probably aren't any logs
            if (!Directory.Exists(logsPath))
                return;
            string[] files = Directory.GetFiles(logsPath, "*.log");
            Array.Sort(files);

            string currentBackup = null;
            string backup;
            List<string> logsForBackup = new List<string>();

            foreach (string file in files) {
                backup = Path.GetFileNameWithoutExtension(file);
                backup = backup.Remove(backup.LastIndexOf('-'));
                if (backup != currentBackup) {
                    // Delete all but the last n files currently in the list
                    if (logsForBackup.Count > Constants.LOGS_KEEP_NUM) {
                        foreach (string fileToDelete in logsForBackup.GetRange(0, logsForBackup.Count - Constants.LOGS_KEEP_NUM)) {
                            File.Delete(fileToDelete);
                        }
                    }
                    logsForBackup.Clear();
                    currentBackup = backup;
                }
                logsForBackup.Add(file);
            }
            // And the final lot
            if (logsForBackup.Count > Constants.LOGS_KEEP_NUM) {
                foreach (string fileToDelete in logsForBackup.GetRange(0, logsForBackup.Count - Constants.LOGS_KEEP_NUM)) {
                    File.Delete(fileToDelete);
                }
            }
        }

        public void Debug(string message) {
            this.Log(Level.DEBUG, message);
        }

        public void Info(string message) {
            this.Log(Level.INFO, message);
        }
        public void Info(string format, params string[] args) {
            this.Log(Level.INFO, String.Format(format, args));
        }

        public void Warn(string message) {
            this.Log(Level.WARN, message);
        }

        public void Error(string message) {
            this.Log(Level.ERROR, message);
        }

        public void Log(Level level, string message) {
            this.writer.WriteLine("[{0:G}] {1}{2}", DateTime.Now, levelStrings[level] == null ? "" : "["+levelStrings[level]+"] ", message);
        }

        public void Close() {
            this.writer.Close();
        }
    }
}
