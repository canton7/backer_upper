using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace BackerUpper
{
    class FileDatabase
    {
        const int CACHED_INSERT_SIZE = 100;

        private SQLiteConnection conn;
        private SQLiteConnection diskConn;
        private List<CachedInsert> cachedInserts;

        //http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/

        public FileDatabase(string path) {
            this.cachedInserts = new List<CachedInsert>();

            bool setupDatabase = !File.Exists(path);

            this.diskConn = new SQLiteConnection("Data Source="+path);
            this.diskConn.Open();

            this.conn = new SQLiteConnection("Data Source=:memory:");
            this.conn.Open();

            this.execute("PRAGMA foreign_keys = ON;");

            if (setupDatabase)
                this.setupDatabase();
            else
                this.diskConn.BackupDatabase(this.conn, "main", "main", -1, null, 0);
        }

        public FolderStatus InspectFolder(string path) {
            string result = this.executeScalar("SELECT id FROM folders WHERE path = @path LIMIT 1", "@path", path);

            if (result == null) {
                return new FolderStatus(0, FolderModStatus.New);
            }

            return new FolderStatus(int.Parse(result), FolderModStatus.StillThere);
        }

        public int AddFolder(string path) {
            string name = Path.GetFileName(path);
            string parentPath = (path == "") ? "" : Path.GetDirectoryName(path);
            
            // Can't cache this one, as we need its inserted ID. Unless we move to GUIDs?
            this.execute("INSERT INTO folders( parent_id, name, path ) VALUES ((SELECT id FROM folders WHERE path = @parent_path LIMIT 1), @name, @path)", "@name", name, "@parent_path", parentPath, "@path", path);
            return this.insertId();
        }

        public void DeleteFolder(int folderId) {
            this.execute("DELETE FROM folders WHERE id = @id", "@id", folderId);
        }

        public FolderRecord[] RecordedFolders() {
            // Make sure we list children before parents. Slightly hacky way of doing it, but should work
            DataTable result = this.executeReader("SELECT id, path FROM folders ORDER BY path DESC");
            FolderRecord[] folderRecords = new FolderRecord[result.Rows.Count];

            DataRow row;
            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                folderRecords[i] = new FolderRecord(Convert.ToInt32(row["id"]), (string)row["path"]);
            }
            return folderRecords;
        }

        public FileStatus InspectFile(int folderId, string name, DateTime lastModified, string fileMD5) {
            // md5 is used to search for "alternates" -- files in a different location, but with the same contents.
            // This can be used to detect moves, and saves unnecessary copying
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            // So search for alternates as well, but favour an exact match
            // 0: exact, 1: id, 2: path, 3: name, 4: date_modified, , 5: md5
            string[] result = this.executeRow(@"SELECT (files.name = @name AND files.folder_id = @folder_id) AS exact, files.id, folders.path, files.name, files.date_modified, files.md5
                FROM files LEFT JOIN folders ON files.folder_id = folders.id
                WHERE exact OR files.md5 = @md5
                ORDER BY exact DESC
                LIMIT 1
                ", "@folder_id", folderId, "@name", Path.GetFileName(name), "@md5", fileMD5);

            if (result.Length == 0) {
                return new FileStatus(FileModStatus.New, null);
            }

            if (result[0] == "1") {
                // Is an exact match
                int fileEpoch = int.Parse(result[4]);
                string md5 = result[5];

                if (lastModifiedEpoch > fileEpoch) {
                    return new FileStatus(FileModStatus.Modified, md5);
                }
                else {
                    return new FileStatus(FileModStatus.Unmodified, md5);
                }
            }
            else {
                // Didn't find an exact match, but found an alternate
                return new FileStatus(FileModStatus.Alternate, result[5], Path.Combine(result[2], result[3]), Convert.ToInt32(result[1]));
            }
        }

        public void AddFile(int folderId, string name, DateTime lastModified, string md5) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            this.execute("INSERT INTO files( folder_id, name, date_modified, md5 ) VALUES (@folder_id, @name, @date_modified, @md5);", "@folder_id", folderId, "@name", Path.GetFileName(name), "@date_modified", lastModifiedEpoch, "@md5", md5);
        }

        public void UpdateFile(int folderId, string name, DateTime lastModified, string md5) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            this.execute("UPDATE files SET date_modified = @date_modified, md5 = @md5 WHERE folder_id = @folder_id AND name = @name;", "@date_modified", lastModifiedEpoch, "@md5", md5, "@folder_id", folderId, "@name", name);
        }

        public void DeleteFile(int fileId) {
            this.execute("DELETE FROM files WHERE id = @id", "@id", fileId);
        }

        public void DeleteFile(int folderId, string name) {
            this.execute("DELETE FROM files WHERE folder_id = @folder_id AND name = @name", "@folder_id", folderId, "@name", Path.GetFileName(name));
        }

        public FileRecord[] RecordedFiles() {
            DataTable result = this.executeReader(@"SELECT files.id, folders.path, files.name FROM files LEFT JOIN folders ON files.folder_id = folders.id");
            FileRecord[] fileRecords = new FileRecord[result.Rows.Count];

            DataRow row;
            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                fileRecords[i] = new FileRecord(Convert.ToInt32(row["id"]), Path.Combine((string)row["path"], (string)row["name"]));
            }
            return fileRecords;
        }

        public void FinishAndSync() {
            //this.executeCachedInserts(true);
            this.conn.BackupDatabase(this.diskConn, "main", "main", -1, null, 0);
        }

        // Not currently used, as the in-memory databases are so fast
        /*
        private void executeCached(string sql, params object[] parameters) {
            SQLiteParameter[] sqliteParamters = new SQLiteParameter[parameters.Length/2];
            for (int i = 0; i < parameters.Length; i += 2) {
                sqliteParamters[i/2] = new SQLiteParameter(parameters[i].ToString(), parameters[i+1]);
            }
            this.cachedInserts.Add(new CachedInsert(sql, sqliteParamters));

            // returns straight away if it doesn't want to insert
            this.executeCachedInserts();
        }

        private void executeCachedInserts(bool force = false) {
            if (!force && this.cachedInserts.Count < CACHED_INSERT_SIZE)
                return;

            SQLiteCommand cmd = new SQLiteCommand("BEGIN", this.conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            // Partition into different sorts of commands
            Dictionary<string, List<SQLiteParameter[]>> commands = new Dictionary<string, List<SQLiteParameter[]>>();
            foreach (CachedInsert insert in this.cachedInserts) {
                if (!commands.ContainsKey(insert.Query)) {
                    commands[insert.Query] = new List<SQLiteParameter[]>();
                }
                commands[insert.Query].Add(insert.Paramters);
            }
            this.cachedInserts.Clear();

            foreach (KeyValuePair<string, List<SQLiteParameter[]>> kvp in commands) {
                cmd = new SQLiteCommand(kvp.Key, this.conn);
                foreach (SQLiteParameter[] parameters in kvp.Value) {
                    cmd.Parameters.Clear();
                    foreach (SQLiteParameter parameter in parameters) {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.ExecuteNonQuery();
                }
            }

            cmd = new SQLiteCommand("END", this.conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
         * */

        private DataTable executeReader(string sql, params object[] parameters) {
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

        private string[] executeRow(string sql, params object[] parameters) {
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
        private int execute(string sql, params object[] parameters) {
            SQLiteCommand com = new SQLiteCommand(sql, this.conn);

            for (int i=0; i<parameters.Length; i+=2) {
                com.Parameters.AddWithValue(parameters[i].ToString(), parameters[i+1]);
            }

            return com.ExecuteNonQuery();
        }

        private string executeScalar(string sql, params object[] parameters) {
            SQLiteCommand com = new SQLiteCommand(sql, this.conn);

            for (int i=0; i<parameters.Length; i+=2) {
                com.Parameters.AddWithValue(parameters[i].ToString(), parameters[i+1]);
            }

            object value = com.ExecuteScalar();
            return (value == null) ? null : value.ToString();
        }

        private int insertId() {
            SQLiteCommand com = new SQLiteCommand("SELECT last_insert_rowid();", this.conn);
            return Convert.ToInt32(com.ExecuteScalar());
        }

        private void setupDatabase() {
            this.execute(@"CREATE TABLE folders( id INTEGER PRIMARY KEY, parent_id INTEGER, name TEXT COLLATE NOCASE, path TEXT COLLATE NOCASE,
                FOREIGN KEY(parent_id) REFERENCES folders(id) );");
            this.execute("CREATE INDEX folder_name_idx ON folders(name);");
            this.execute("CREATE INDEX folder_path_idx ON folders(path);");
            this.execute(@"CREATE TABLE files( id INTEGER PRIMARY KEY, folder_id INTEGER, name TEXT COLLATE NOCASE, date_modified INTEGER, md5 TEXT COLLATE NOCASE,
                FOREIGN KEY(folder_id) REFERENCES folders(id) );");
            // We have a foreign key constraint which enforces parent folders. However, root folders have a parentId of 0 (to make the code easier), which violates this.
            // Therefore, add a root entry with an ID of 0 and a parent of null
            this.execute("INSERT INTO folders (id, parent_id, name, path) VALUES (0, null, 'root', '');");
        }

        public enum FolderModStatus { New, StillThere, Deleted };

        public struct FolderStatus
        {
            public int Id;
            public FolderModStatus FolderModStatus;

            public FolderStatus(int id, FolderModStatus folderModStatus) {
                this.Id = id;
                this.FolderModStatus = folderModStatus;
            }
        }

        public enum FileModStatus { New, Modified, Unmodified, Deleted, Alternate };

        public struct FileStatus
        {
            public FileModStatus FileModStatus;
            public string MD5;
            public string AlternatePath;
            public int AlternateId;

            public FileStatus(FileModStatus fileModStatus, string md5, string alternatePath = null, int alternateId = 0) {
                this.FileModStatus = fileModStatus;
                this.MD5 = md5;
                this.AlternatePath = alternatePath;
                this.AlternateId = alternateId;
            }
        }

        public struct FileRecord
        {
            public int Id;
            public string Path;

            public FileRecord(int id, string path) {
                this.Id = id;
                this.Path = path;
            }
        }

        public struct FolderRecord {
            public int Id;
            public string Path;

            public FolderRecord(int id, string path) {
                this.Id = id;
                this.Path = path;
            }
        }

        private struct CachedInsert
        {
            public string Query;
            public SQLiteParameter[] Paramters;

            public CachedInsert(string query, SQLiteParameter[] paramters) {
                this.Query = query;
                this.Paramters = paramters;
            }
        }
    }
}
