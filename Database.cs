using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace BackerUpper
{
    class Database
    {
        // If we're working from on-disk, diskConn is null and conn contains the disk database
        // If we're working from memoroy, conn contains the memory db and diskConn the disk db
        private SQLiteConnection conn;
        private SQLiteConnection diskConn;

        public Database(string path) {
            if (!File.Exists(path))
                throw new FileNotFoundException("Could not find database "+path);

            this.conn = new SQLiteConnection("Data Source="+path);
            this.conn.Open();

            this.diskConn = null;

            this.Execute("PRAGMA foreign_keys = ON;");
        }

        public void LoadToMemory() {
            if (this.diskConn != null)
                return;

            SQLiteConnection memoryConn = new SQLiteConnection("Data Source=:memory:");
            memoryConn.Open();
            this.conn.BackupDatabase(memoryConn, "main", "main", -1, null, 0);
            this.diskConn = this.conn;
            this.conn = memoryConn;
        }

        public void SyncToDisk() {
            this.conn.BackupDatabase(this.diskConn, "main", "main", -1, null, 0);
        }

        public void UnloadFromMemory() {
            if (this.diskConn == null)
                return;

            this.SyncToDisk();
            this.conn.Close();
            this.conn = this.diskConn;
            this.diskConn = null;
        }

        public void Close() {
            this.UnloadFromMemory();
            this.conn.Close();
        }

        public DataTable ExecuteReader(string sql, params object[] parameters) {
            DataTable table = new DataTable();
            SQLiteCommand com = new SQLiteCommand(sql, this.conn);

            for (int i=0; i<parameters.Length; i+=2) {
                com.Parameters.AddWithValue(parameters[i].ToString(), parameters[i+1]);
            }

            SQLiteDataReader reader = com.ExecuteReader();
            table.Load(reader);
            reader.Close();
            return table;
        }

        public string[] ExecuteRow(string sql, params object[] parameters) {
            SQLiteCommand com = new SQLiteCommand(sql, this.conn);

            for (int i=0; i<parameters.Length; i+=2) {
                com.Parameters.AddWithValue(parameters[i].ToString(), parameters[i+1]);
            }

            SQLiteDataReader reader = com.ExecuteReader();
            if (!reader.Read()) {
                return new string[0];
            }

            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++) {
                row[i] = reader.GetValue(i).ToString();
            }
            reader.Close();

            return row;
        }

        /*
        private string[] executeCol(string sql, params object[] parameters) {
            SQLiteCommand com = new SQLiteCommand(sql, this.conn);

            for (int i=0; i<parameters.Length; i+=2) {
                com.Parameters.AddWithValue(parameters[i].ToString(), parameters[i+1]);
            }

            SQLiteDataReader reader = com.ExecuteReader();
            List<string> cols = new List<string>();

            while (reader.Read()) {
                cols.Add(reader.GetValue(0).ToString());
            }
            reader.Close();

            return cols.ToArray();
        }
         */

        public int Execute(string sql, params object[] parameters) {
            SQLiteCommand com = new SQLiteCommand(sql, this.conn);

            for (int i=0; i<parameters.Length; i+=2) {
                com.Parameters.AddWithValue(parameters[i].ToString(), parameters[i+1]);
            }

            return com.ExecuteNonQuery();
        }

        public string ExecuteScalar(string sql, params object[] parameters) {
            SQLiteCommand com = new SQLiteCommand(sql, this.conn);

            for (int i=0; i<parameters.Length; i+=2) {
                com.Parameters.AddWithValue(parameters[i].ToString(), parameters[i+1]);
            }

            object value = com.ExecuteScalar();
            return (value == null) ? null : value.ToString();
        }

        public int InsertId() {
            SQLiteCommand com = new SQLiteCommand("SELECT last_insert_rowid();", this.conn);
            return Convert.ToInt32(com.ExecuteScalar());
        }

        public static Database CreateDatabase(string path) {
            Database database = new Database(path);

            database.Execute(@"CREATE TABLE folders( id INTEGER PRIMARY KEY, parent_id INTEGER, name TEXT COLLATE NOCASE, path TEXT COLLATE NOCASE,
                FOREIGN KEY(parent_id) REFERENCES folders(id) );");
            database.Execute("CREATE INDEX folders_name_idx ON folders(name);");
            database.Execute("CREATE INDEX folders_path_idx ON folders(path);");
            database.Execute(@"CREATE TABLE files( id INTEGER PRIMARY KEY, folder_id INTEGER, name TEXT COLLATE NOCASE, date_modified INTEGER, md5 TEXT COLLATE NOCASE,
                FOREIGN KEY(folder_id) REFERENCES folders(id) );");
            database.Execute(@"CREATE INDEX files_folder_id_idx ON files(folder_id);");
            database.Execute(@"CREATE INDEX files_name_idx ON files(name);");
            // We have a foreign key constraint which enforces parent folders. However, root folders have a parentId of 0 (to make the code easier), which violates this.
            // Therefore, add a root entry with an ID of 0 and a parent of null
            database.Execute("INSERT INTO folders (id, parent_id, name, path) VALUES (0, null, 'root', '');");

            database.Execute("CREATE TABLE settings( id INTEGER PRIMARY KEY, name TEXT, value TEXT );");
            database.Execute("CREATE INDEX settings_name_idx ON settings(name)");

            return database;
        }
    }
}
