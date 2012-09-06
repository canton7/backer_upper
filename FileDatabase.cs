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
                return new FolderStatus(-1, FolderModStatus.New);
            }

            return new FolderStatus(int.Parse(result), FolderModStatus.Unmodified);
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

        public IEnumerable<FolderRecord> RecordedFolders() {
            // Make sure we list children before parents. Slightly hacky way of doing it, but should work
            DataTable result = this.db.ExecuteReader("SELECT id, path FROM folders ORDER BY path DESC");

            DataRow row;
            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                yield return new FolderRecord(Convert.ToInt32(row["id"]), (string)row["path"]);
            }
        }

        public FileStatus InspectFile(int folderId, string name, DateTime lastModified) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            string[] result = this.db.ExecuteRow("SELECT files.date_modified, files.md5 FROM files WHERE name = @name and folder_id = @folder_id LIMIT 1", "@folder_id", folderId, "@name", Path.GetFileName(name));

            if (result.Length == 0) {
                return new FileStatus(FileModStatus.New, null);
            }

            int fileEpoch = Convert.ToInt32(result[0]);
            string md5 = result[1];

            if (lastModifiedEpoch > fileEpoch) {
                return new FileStatus(FileModStatus.Newer, md5);
            }
            else if (lastModifiedEpoch < fileEpoch) {
                return new FileStatus(FileModStatus.Older, md5);
            }
            else {
                return new FileStatus(FileModStatus.Unmodified, md5);
            }
        }

        // BAD DUPLICATION. But, InspectFile is very heavily on the critical path during a backup, and this is only used during a restore
        public FileStatusWithLastModified InspectFileWithLastModified(int folderId, string name, DateTime lastModified) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            string[] result = this.db.ExecuteRow("SELECT files.date_modified, files.md5 FROM files WHERE name = @name and folder_id = @folder_id LIMIT 1", "@folder_id", folderId, "@name", Path.GetFileName(name));

            if (result.Length == 0) {
                return new FileStatusWithLastModified(FileModStatus.New, null, DateTime.UtcNow);
            }

            int fileEpoch = Convert.ToInt32(result[0]);
            string md5 = result[1];
            DateTime fileLastModified = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(fileEpoch);

            if (lastModifiedEpoch > fileEpoch) {
                return new FileStatusWithLastModified(FileModStatus.Newer, md5, fileLastModified);
            }
            else if (lastModifiedEpoch < fileEpoch) {
                return new FileStatusWithLastModified(FileModStatus.Older, md5, fileLastModified);
            }
            else {
                return new FileStatusWithLastModified(FileModStatus.Unmodified, md5, fileLastModified);
            }
        }

        public FileRecord[] SearchForAlternates(string fileMD5) {
            DataTable result = this.db.ExecuteReader("SELECT files.id, folders.path, files.name FROM files LEFT JOIN folders ON files.folder_id = folders.id WHERE files.md5 = @md5", "@md5", fileMD5);

            FileRecord[] alternates = new FileRecord[result.Rows.Count];
            DataRow row;

            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                alternates[i] = new FileRecord(Convert.ToInt32(row["id"]), Path.Combine(row["path"].ToString(), row["name"].ToString()));
            }

            return alternates;
        }

        public void AddFile(int folderId, string name, DateTime lastModified, string md5) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            this.db.Execute("INSERT OR REPLACE INTO files( folder_id, name, date_modified, md5 ) VALUES (@folder_id, @name, @date_modified, @md5);", "@folder_id", folderId, "@name", Path.GetFileName(name), "@date_modified", lastModifiedEpoch, "@md5", md5);
        }

        public void UpdateFile(int folderId, string name, DateTime lastModified, string md5) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            this.db.Execute("UPDATE files SET date_modified = @date_modified, md5 = @md5 WHERE folder_id = @folder_id AND name = @name;", "@date_modified", lastModifiedEpoch, "@md5", md5, "@folder_id", folderId, "@name", Path.GetFileName(name));
        }

        public void UpdateFile(int id, DateTime lastModified, string md5=null) {
            int lastModifiedEpoch = (int)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;

            if (md5 == null)
                this.db.Execute("UPDATE files SET date_modified = @date_modified WHERE id = @id", "@date_modified", lastModifiedEpoch, "@id", id);
            else
                this.db.Execute("UPDATE files SET date_modified = @date_modified, md5 = @md5 WHERE id = @id", "@date_modified", lastModifiedEpoch, "@md5", md5, "@id", id);
        }

        public void DeleteFile(int fileId) {
            this.db.Execute("DELETE FROM files WHERE id = @id", "@id", fileId);
        }

        public void DeleteFile(int folderId, string name) {
            this.db.Execute("DELETE FROM files WHERE folder_id = @folder_id AND name = @name", "@folder_id", folderId, "@name", Path.GetFileName(name));
        }

        public IEnumerable<FileRecord> RecordedFiles() {
            DataTable result = this.db.ExecuteReader(@"SELECT files.id, folders.path, files.name FROM files LEFT JOIN folders ON files.folder_id = folders.id");

            DataRow row;
            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                yield return new FileRecord(Convert.ToInt32(row["id"]), Path.Combine(row["path"].ToString(), row["name"].ToString()));
            }
        }

        public IEnumerable<FileRecordExtended> RecordedFilesExtended() {
            DataTable result = this.db.ExecuteReader(@"SELECT files.id, folders.path, files.name, files.date_modified, files.md5 FROM files LEFT JOIN folders ON files.folder_id = folders.id");

            DataRow row;
            for (int i = 0; i < result.Rows.Count; i++) {
                row = result.Rows[i];
                DateTime lastModified = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt32(row["date_modified"]));
                yield return new FileRecordExtended(Convert.ToInt32(row["id"]), Path.Combine(row["path"].ToString(), row["name"].ToString()), lastModified, row["md5"].ToString());
            }
        }

        public void PurgeDatabase(IEnumerable<string> files, IEnumerable<string> folders) {
            // Delete entries from the DB which aren't in files or folders
            List<int> idsToDelete = new List<int>();
            DataTable dbFiles = this.db.ExecuteReader("SELECT files.id, files.name, folders.path FROM files LEFT JOIN folders ON files.folder_id = folders.id");
            foreach (DataRow file in dbFiles.Rows) {
                if (!files.Contains(Path.Combine(file["path"].ToString(), file["name"].ToString())))
                    idsToDelete.Add(Convert.ToInt32(file["id"]));
            }
            this.db.Execute("DELETE FROM files WHERE id IN ("+String.Join(",", idsToDelete)+")");
            idsToDelete.Clear();
            DataTable dbFolders = this.db.ExecuteReader("SELECT id, path FROM folders");
            foreach (DataRow folder in dbFolders.Rows) {
                if (!folders.Contains(folder["path"]))
                    idsToDelete.Add(Convert.ToInt32(folder["id"]));
            }
            this.db.Execute("DELETE FROM folders WHERE id IN ("+String.Join(",", idsToDelete)+")");
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

        

        public enum FolderModStatus { New, Unmodified };

        public struct FolderStatus
        {
            public int Id;
            public FolderModStatus FolderModStatus;

            public FolderStatus(int id, FolderModStatus folderModStatus) {
                this.Id = id;
                this.FolderModStatus = folderModStatus;
            }
        }

        public enum FileModStatus { New, Newer, Older, Unmodified };

        public struct FileStatus
        {
            public FileModStatus FileModStatus;
            public string MD5;

            public FileStatus(FileModStatus fileModStatus, string md5) {
                this.FileModStatus = fileModStatus;
                this.MD5 = md5;
            }
        }
        
        public struct FileStatusWithLastModified
        {
            public FileModStatus FileModStatus;
            public string MD5;
            public DateTime LastModified;

            public FileStatusWithLastModified(FileModStatus fileModStatus, string md5, DateTime lastModified) {
                this.FileModStatus = fileModStatus;
                this.MD5 = md5;
                this.LastModified = lastModified;
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

        public struct FileRecordExtended
        {
            public int Id;
            public string Path;
            public DateTime LastModified;
            public string MD5;

            public FileRecordExtended(int id, string path, DateTime lastModified, string md5) {
                this.Id = id;
                this.Path = path;
                this.LastModified = lastModified;
                this.MD5 = md5;
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
