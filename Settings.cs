using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackerUpper
{
    class Settings
    {
        Database db;

        public string Source {
            get { return this.getKey("source"); }
            set { this.setKey("source", value); }
        }

        public string Dest {
            get { return this.getKey("dest"); }
            set { this.setKey("dest", value); }
        }

        public string Name {
            get { return this.getKey("name"); }
            set { this.setKey("name", value); }
        }

        public Settings(Database db) {
            this.db = db;
        }

        private void setKey(string name, string value) {
            this.db.Execute("UPDATE settings SET value = @value WHERE name = @name", "@name", name, "@value", value);
        }

        private string getKey(string name) {
            return this.db.ExecuteScalar("SELECT value FROM settings WHERE name = @name", "@name", name);
        }

        public void PopulateInitial(string name) {
            this.db.Execute("INSERT INTO settings(name, value) VALUES('name', @name), ('source', ''), ('dest', '');", "@name", name);
        }
    }
}
