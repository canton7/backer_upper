using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Timers;

namespace BackerUpper
{
    class Database
    {
        // If we're working from on-disk, diskConn is null and conn contains the disk database
        // If we're working from memoroy, conn contains the memory db and diskConn the disk db
        private SQLiteConnection conn;
        private SQLiteConnection diskConn;
        private Timer syncTimer;
        private bool nonScalarExecuted = false;
        private string lockName;
        private FileStream dbLock;
        public string FilePath { get; private set; }

        public bool AutoSyncToDisk {
            get { return this.syncTimer.Enabled; }
            set {
                this.syncTimer.Enabled = value;
                if (value)
                    this.syncTimer.Start();
                else
                    this.syncTimer.Stop();
            }
        }

        public Database(string path) {
            this.FilePath = path;

            if (!File.Exists(this.FilePath))
                throw new FileNotFoundException("Could not find database "+this.FilePath);

            this.lockName = Path.Combine(Path.GetDirectoryName(this.FilePath), Path.GetFileNameWithoutExtension(this.FilePath) + ".lock");

            this.conn = new SQLiteConnection("Data Source="+this.FilePath);
            this.conn.Open();

            this.diskConn = null;

            this.executeWithoutLock("PRAGMA foreign_keys = ON;");

            // 5 minutes
            this.syncTimer = new Timer(300000) {
                AutoReset = true,
                Enabled = false,
            };
            this.syncTimer.Elapsed += new ElapsedEventHandler(syncTimer_Elapsed);
        }

        void syncTimer_Elapsed(object sender, ElapsedEventArgs e) {
            // Periodically sync the DB back to disk if a nonScalar execution has occurred -- most likely to be UPDATE/INSERT etc
            if (!this.nonScalarExecuted)
                return;
            this.SyncToDisk();
            this.nonScalarExecuted = false;
        }

        public void Open() {
            // Just ensure we're open
            if (this.conn.State != ConnectionState.Open)
                this.conn.Open();
        }

        public void Lock() {
            if (this.dbLock != null)
                return;

            try {
                this.dbLock = new FileStream(this.lockName, FileMode.Create);
            }
            catch (IOException) {
                throw new DatabaseInUseException(this.lockName);
            }
        }
        
        public void Unlock() {
            if (this.dbLock == null)
                return;

            this.dbLock.Close();
            this.dbLock = null;
        }

        public void LoadToMemory() {
            if (this.diskConn != null)
                return;

            this.Lock();

            SQLiteConnection memoryConn = new SQLiteConnection("Data Source=:memory:");
            memoryConn.Open();
            this.conn.BackupDatabase(memoryConn, "main", "main", -1, null, 0);
            this.diskConn = this.conn;
            this.conn = memoryConn;
        }

        public void SyncToDisk() {
            // Only if we're using a memory db
            if (this.diskConn != null)
                this.conn.BackupDatabase(this.diskConn, "main", "main", -1, null, 0);
        }

        public void UnloadFromMemory() {
            if (this.diskConn == null)
                return;

            this.SyncToDisk();
            this.conn.Close();
            this.conn = this.diskConn;
            this.diskConn = null;

            this.Unlock();
            File.Delete(this.lockName);
        }

        public void Close() {
            this.UnloadFromMemory();
            // UnloadFromMemory may not unlock
            this.Unlock();
            this.conn.Close();
        }

        public int NumFiles() {
            return Convert.ToInt32(this.ExecuteScalar("SELECT COUNT(id) FROM 'files'"));
        }

        public int NumFolders() {
            return Convert.ToInt32(this.ExecuteScalar("SELECT COUNT(id) FROM 'folders'"));
        }

        public static string GetExportableFile(string filePath, bool stripFilesFolders=false) {
            // If we're not stripping files and folders, just return the file path
            if (!stripFilesFolders)
                return filePath;
            // Otherwise copy, open, truncate files and folders, close
            string destPath = Path.GetTempFileName();
            File.Copy(filePath, destPath, true);
            Database db = new Database(destPath);
            db.Execute("DELETE FROM 'files'");
            db.Execute("DELETE FROM 'folders'");
            db.Close();
            return destPath;
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
            return this.execute(true, sql, parameters);
        }

        private int executeWithoutLock(string sql, params object[] parameters) {
            return this.execute(false, sql, parameters);
        }

        private int execute(bool requireLock, string sql, params object[] parameters) {
            if (requireLock)
                this.Lock();
            this.nonScalarExecuted = true;

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

        public class DatabaseInUseException : IOException
        {
            public string LockFile { get; private set; }
            public DatabaseInUseException(string lockFile)
                    : base() {
                this.LockFile = lockFile;
            }
        }
    }
}
