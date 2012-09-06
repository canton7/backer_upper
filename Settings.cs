using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackerUpper
{
    class Settings
    {
        const string INITIAL_IGNORE = "*.tmp | *.temp";

        Database db;
        public Database Database {
            get { return this.db; }
        }

        public string Source {
            get { return this.getKey("source"); }
            set { this.setKey("source", value); }
        }

        public bool MirrorEnabled {
            get { return (this.getKey("mirrorEnabled") == "1"); }
            set { this.setKey("mirrorEnabled", value ? "1" : "0"); }
        }

        public string MirrorDest {
            get { return this.getKey("mirrorDest"); }
            set { this.setKey("mirrorDest", value); }
        }

        public bool S3Enabled {
            get { return (this.getKey("s3Enabled") == "1"); }
            set { this.setKey("s3Enabled", value ? "1" : "0"); }
        }

        public string S3Dest {
            get { return this.getKey("s3Dest"); }
            set { this.setKey("s3Dest", value); }
        }

        public string S3PublicKey {
            get { return this.getKey("s3PublicKey"); }
            set { this.setKey("s3PublicKey", value); }
        }

        public string S3PrivateKey {
            get { return this.getKey("s3PrivateKey"); }
            set { this.setKey("s3PrivateKey", value); }
        }

        public bool S3UseRRS {
            get { return this.getKey("s3UseRRS") == "1"; }
            set { this.setKey("s3UseRRS", value ? "1" : "0"); }
        }

        public bool S3Test {
            get { return this.getKey("s3Test") == "1"; }
            set { this.setKey("s3Test", value ? "1" : "0"); }
        }

        public string Name {
            get { return this.getKey("name"); }
            set { this.setKey("name", value); }
        }

        public string FileIgnorePattern {
            get { return this.getKey("fileIgnorePattern"); }
            set { this.setKey("fileIgnorePattern", value); }
        }

        public DateTime LastRun {
            get { return new DateTime(1970, 1, 1).AddSeconds(Convert.ToInt32(this.getKey("lastRun"))); }
            set { this.setKey("lastRun", ((int)(value - new DateTime(1970, 1, 1)).TotalSeconds).ToString()); }
        }

        public bool LastRunCancelled {
            get { return (this.getKey("lastRunCancelled") == "1"); }
            set { this.setKey("lastRunCancelled", value ? "1" : "0"); }
        }

        public bool LastRunErrors {
            get { return (this.getKey("lastRunErrors") == "1"); }
            set { this.setKey("lastRunErrors", value ? "1" : "0"); }
        }

        public bool Autoclose {
            get { return (this.getKey("autoclose") == "1"); }
            set { this.setKey("autoclose", value ? "1" : "0"); }
        }

        public bool IgnoreWarnings {
            get { return (this.getKey("ignoreWarnings") == "1"); }
            set { this.setKey("ignoreWarnings", value ? "1" : "0"); }
        }

        public Settings(Database db) {
            this.db = db;
        }

        private void setKey(string name, string value) {
            // Handle settings being added in on-the-fly (e.g. from upgrade)
            if (this.db.Execute("UPDATE settings SET value = @value WHERE name = @name", "@name", name, "@value", value) == 0)
                this.db.Execute("INSERT INTO settings (name, value) VALUES (@name, @value)", "@name", name, "@value", value);
        }

        private string getKey(string name) {
            return this.db.ExecuteScalar("SELECT value FROM settings WHERE name = @name", "@name", name);
        }

        public void PopulateInitial(string name) {
            this.db.Execute(@"INSERT INTO settings(name, value) VALUES
                ('name', @name), ('source', ''),
                ('mirrorEnabled', '0'), ('mirrorDest', ''),
                ('s3Enabled', '0'), ('s3Dest', ''), ('s3PublicKey', ''), ('s3PrivateKey', ''), ('s3UseRRS', '0'), ('s3Test', '0'),
                ('lastRun', '0'), ('lastRunCancelled', '0'), ('lastRunErrors', '0'),
                ('fileIgnorePattern', @ignore), ('autoclose', '1'), ('ignoreWarnings', '0'); ", "@name", name, "@ignore", INITIAL_IGNORE);
        }
    }
}
