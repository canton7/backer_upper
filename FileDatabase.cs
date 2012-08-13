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
        //const int CACHED_INSERT_SIZE = 100;
        
        //private List<CachedInsert> cachedInserts;

        private Database db;

        //http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/

        public FileDatabase(Database db) {
            //this.cachedInserts = new List<CachedInsert>();
            this.db = db;
        }

        public void SyncToDisk() {
            this.db.SyncToDisk();
        }

        public FolderStatus InspectFolder(string path) {
            string result = this.db.ExecuteScalar("SELECT id FROM folders WHERE path = @path LIMIT 1", "@path", path);

            if (result == null) {
                return new FolderStatus(0, FolderModStatus.New);
            }

            return new FolderStatus(int.Parse(result), FolderModStatus.StillThere);
        }

        public int AddFolder(string path) {
            string name = Path.GetFileName(path);
            string parentPath = (path == "") ? "" : Path.GetDirectoryName(path);
            
            // Can't cache this one, as we need its inserted ID. Unless we move to GUIDs?
            this.db.Execute("INSERT INTO folders( parent_id, name, path ) VALUES ((SELECT id FROM folders WHERE path = @parent_path LIMIT 1), @name, @path)", "@name", name, "@parent_path", parentPath, "@path", path);
            return this.db.InsertId();
        }

        public void DeleteFolder(int folderId) {
            this.db.Execute("DELETE FROM folders WHERE id = @id", "@id", folderId);
        }

        public FolderRecord[] RecordedFolders() {
            // Make sure we list children before parents. Slightly hacky way of doing it, but should work
            DataTable result = this.db.ExecuteReader("SELECT id, path FROM folders ORDER BY path DESC");
            FolderRecord[] folderRecords = new FolderRecord[result.Rows.Count];

            DataRow row;
            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                folderRecords[i] = new FolderRecord(Convert.ToInt32(row["id"]), (string)row["path"]);
            }
            return folderRecords;
        }

        public FileStatus InspectFile(int folderId, string name, DateTime lastModified) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            string[] result = this.db.ExecuteRow("SELECT files.date_modified, files.md5 FROM files WHERE name = @name and folder_id = @folder_id LIMIT 1", "@folder_id", folderId, "@name", Path.GetFileName(name));

            if (result.Length == 0) {
                return new FileStatus(FileModStatus.New, null);
            }

            // Is an exact match
            int fileEpoch = int.Parse(result[0]);
            string md5 = result[1];

            if (lastModifiedEpoch > fileEpoch) {
                return new FileStatus(FileModStatus.Modified, md5);
            }
            else {
                return new FileStatus(FileModStatus.Unmodified, md5);
            }
        }

        public FileRecord SearchForAlternates(string fileMD5) {
            string[] result = this.db.ExecuteRow("SELECT files.id, folders.path, files.name FROM files LEFT JOIN folders ON files.folder_id = folders.id WHERE files.md5 = @md5 LIMIT 1", "@md5", fileMD5);

            if (result.Length == 0) {
                return new FileRecord(0, null);
            }

            return new FileRecord(Convert.ToInt32(result[0]), Path.Combine((string)result[1], (string)result[2]));
        }

        public void AddFile(int folderId, string name, DateTime lastModified, string md5) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            this.db.Execute("INSERT INTO files( folder_id, name, date_modified, md5 ) VALUES (@folder_id, @name, @date_modified, @md5);", "@folder_id", folderId, "@name", Path.GetFileName(name), "@date_modified", lastModifiedEpoch, "@md5", md5);
        }

        public void UpdateFile(int folderId, string name, DateTime lastModified, string md5) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            this.db.Execute("UPDATE files SET date_modified = @date_modified, md5 = @md5 WHERE folder_id = @folder_id AND name = @name;", "@date_modified", lastModifiedEpoch, "@md5", md5, "@folder_id", folderId, "@name", name);
        }

        public void DeleteFile(int fileId) {
            this.db.Execute("DELETE FROM files WHERE id = @id", "@id", fileId);
        }

        public void DeleteFile(int folderId, string name) {
            this.db.Execute("DELETE FROM files WHERE folder_id = @folder_id AND name = @name", "@folder_id", folderId, "@name", Path.GetFileName(name));
        }

        public FileRecord[] RecordedFiles() {
            DataTable result = this.db.ExecuteReader(@"SELECT files.id, folders.path, files.name FROM files LEFT JOIN folders ON files.folder_id = folders.id");
            FileRecord[] fileRecords = new FileRecord[result.Rows.Count];

            DataRow row;
            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                fileRecords[i] = new FileRecord(Convert.ToInt32(row["id"]), Path.Combine(row["path"].ToString(), row["name"].ToString()));
            }
            return fileRecords;
        }

        /*
        public void FinishAndSync() {
            //this.executeCachedInserts(true);
            this.conn.BackupDatabase(this.diskConn, "main", "main", -1, null, 0);
        }
         * */

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

        public enum FileModStatus { New, Modified, Unmodified, Deleted };

        public struct FileStatus
        {
            public FileModStatus FileModStatus;
            public string MD5;

            public FileStatus(FileModStatus fileModStatus, string md5) {
                this.FileModStatus = fileModStatus;
                this.MD5 = md5;
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

        public struct FolderRecord
        {
            public int Id;
            public string Path;

            public FolderRecord(int id, string path) {
                this.Id = id;
                this.Path = path;
            }
        }

        //private struct CachedInsert
        //{
        //    public string Query;
        //    public SQLiteParameter[] Paramters;

        //    public CachedInsert(string query, SQLiteParameter[] paramters) {
        //        this.Query = query;
        //        this.Paramters = paramters;
        //    }
        //}
    }
}
