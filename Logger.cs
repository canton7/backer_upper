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

        private string logFilePath;
        public string LogFilePath {
            get { return this.logFilePath; }
        }

        public Logger(string type) {
            string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPDATA_FOLDER, Constants.LOG_FOLDER);
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);
            string fileName = String.Format("{0}-{1:yyyyMMdd-HHmmss}.log", type, DateTime.Now);
            this.logFilePath = Path.Combine(logsPath, fileName);
            this.writer = new StreamWriter(this.logFilePath);

            this.writer.WriteLine("To find warnings or errors, search for [{0}] or [{1}]\n", levelStrings[Level.WARN], levelStrings[Level.ERROR]);
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
