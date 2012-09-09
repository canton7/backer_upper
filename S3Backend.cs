using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace BackerUpper
{
    class S3Backend : BackendBase
    {
        private AmazonS3Client client;
        private string bucket;
        private string prefix;
        private List<string> files;
        private List<string> folders;
        private S3StorageClass storageClass;
        private bool performTests;

        public override string Name {
            get { return "S3"; }
        }

        public override bool StripFilesFoldersOnDBBackup {
            get { return true; }
        }

        public S3Backend(string dest, bool performTests, string publicKey, string privateKey, bool useRRS)
            : base(dest) {
            string[] parts = dest.Split(new char[] {'/'}, 2);
            this.bucket = parts[0];
            this.prefix = parts.Length > 1 ? parts[1].Replace('\\', '/').TrimEnd(new char[] { '/' }) + '/' : "/";

            this.client = new AmazonS3Client(publicKey, privateKey);
            this.storageClass = useRRS ? S3StorageClass.ReducedRedundancy : S3StorageClass.Standard;
            this.performTests = performTests;
        }

        public override void SetupInitial() {
            // This is a chance to do heavy stuff, but in a thread
            
            // Obtain a full list of the bucket. Helps a lot later
            ListObjectsRequest listRequest = new ListObjectsRequest() {
                BucketName = this.bucket,
                Prefix = this.prefix
            };
            ListObjectsResponse listResponse;
            this.files = new List<string>();
            this.folders = new List<string>();
            // This needs fixing, but somewhere else
            this.folders.Add("");

            try {
                do {
                    listResponse = client.ListObjects(listRequest);
                    foreach (S3Object obj in listResponse.S3Objects) {
                        if (obj.Key.EndsWith("/") && obj.Size == 0)
                            this.folders.Add(obj.Key.TrimEnd(new char[] { '/' }).Substring(this.prefix.Length));
                        else
                            this.files.Add(obj.Key.Substring(this.prefix.Length));
                    }
                    listRequest.Marker = listResponse.NextMarker;
                } while (listResponse.NextMarker != null);
            }
            catch (AmazonS3Exception ex) {
                string message = ex.Message;
                // Clarify this error
                if (ex.ErrorCode == "SignatureDoesNotMatch")
                    message += "\n\nThis can be caused by an invalid S3 secret key credential.";
                throw new IOException(message);
            }
            catch (System.Net.WebException e) { throw new IOException(e.Message); }
        }

        public override void CreateFolder(string folder) {
            folder = folder.Replace('\\', '/');
            PutObjectRequest request = new PutObjectRequest() {
                BucketName = this.bucket,
                Key = this.prefix + folder + '/',
                ContentBody = "",
                StorageClass = this.storageClass,
            };
            this.withHandling(() => this.client.PutObject(request), folder);
            this.folders.Add(folder);
        }

        public override void DeleteFolder(string folder) {
            folder = folder.Replace('\\', '/');
            DeleteObjectRequest request = new DeleteObjectRequest() {
                BucketName = this.bucket,
                Key = (this.prefix + folder + '/')
            };
            this.withHandling(() => this.client.DeleteObject(request), folder);
            this.folders.Remove(folder);
        }

        public override bool FolderExists(string folder) {
            folder = folder.Replace('\\', '/');
            return this.folders.Contains(folder);
        }

        public override bool CreateFile(string file, string source, DateTime lastModified, string fileMD5, bool reportProgress=true) {
            file = file.Replace('\\', '/');
            string key = this.prefix + file;

            if (this.files.Contains(file) && this.FileMD5(file) == fileMD5) {
                return false;
            }

            PutObjectRequest putRequest = new PutObjectRequest() {
                BucketName = this.bucket,
                FilePath = source,
                Key = key,
                Timeout = -1,
                CannedACL = S3CannedACL.Private,
                StorageClass = this.storageClass,
            };
            if (reportProgress)
                putRequest.PutObjectProgressEvent += new EventHandler<PutObjectProgressArgs>(putRequest_PutObjectProgressEvent);
            this.withHandling(() => putRequest.AddHeader("x-amz-meta-md5", fileMD5), file);
            this.withHandling(() => this.client.PutObject(putRequest), file);
            this.files.Add(file);
            return true;
        }

        public override bool TestFile(string file, DateTime lastModified, string fileMD5) {
            // lastModified isn't much of an indication, since we can't update it at will (updateLastModified does nothing)
            // But we can use the md5 for a good test
            file = file.Replace('\\', '/');
            string key = this.prefix + file;

            if (!this.files.Contains(file))
                return false;

            // If we're not performing tests, don't
            if (!this.performTests)
                return true;

            return fileMD5 == this.FileMD5(file);
        }

        public override void RestoreFile(string file, string dest, DateTime lastModified) {
            file = file.Replace('\\', '/');
            string key = this.prefix + file;

            if (!this.files.Contains(file))
                throw new BackupOperationException(file, "File could not be found on backend, so can't restore");

            GetObjectRequest getRequest = new GetObjectRequest() {
                BucketName = this.bucket,
                Key = key,
                Timeout = -1,
            };
            try {
                GetObjectResponse getResponse = this.client.GetObject(getRequest);
                getResponse.WriteObjectProgressEvent += new EventHandler<WriteObjectProgressArgs>(getResponse_WriteObjectProgressEvent);
                getResponse.WriteResponseStreamToFile(dest);
            }
            catch (AmazonS3Exception e) { throw new BackupOperationException(file, e.Message); }
            catch (System.Net.WebException e) { throw new BackupOperationException(file, e.Message); }
            File.SetLastWriteTimeUtc(dest, lastModified);
        }

        public override void TouchFile(string file, DateTime lastModified) {
            // Don't do anything. We don't care about mtimes for our TestFile function
        }

        public override void BackupDatabase(string file, string source) {
            this.CreateFile(file, source, DateTime.UtcNow, null, false);
        }

        private void putRequest_PutObjectProgressEvent(object sender, PutObjectProgressArgs e) {
            this.ReportProcess(e.PercentDone);
        }

        void getResponse_WriteObjectProgressEvent(object sender, WriteObjectProgressArgs e) {
            this.ReportProcess(e.PercentDone);
        }


        public override void DeleteFile(string file) {
            file = file.Replace('\\', '/');
            DeleteObjectRequest request = new DeleteObjectRequest() {
                BucketName = this.bucket,
                Key = this.prefix + file
            };
            this.withHandling(() => this.client.DeleteObject(request), file);
            this.files.Remove(file);
        }

        public override bool FileExists(string file) {
            file = file.Replace('\\', '/');
            return this.files.Contains(file);
        }

        public override string FileMD5(string file) {
            file = file.Replace('\\', '/');
            string key = this.prefix + file;

            if (!this.files.Contains(file))
                return null;

            try {
                GetObjectMetadataRequest metaRequest = new GetObjectMetadataRequest() {
                    BucketName = this.bucket,
                    Key = key
                };
                GetObjectMetadataResponse metaResponse = client.GetObjectMetadata(metaRequest);
                return metaResponse.Headers["x-amz-meta-md5"];
            }
            catch (AmazonS3Exception e) { throw new BackupOperationException(file, e.Message); }
            catch (System.Net.WebException e) { throw new BackupOperationException(file, e.Message); }
        }

        public override DateTime FileLastModified(string file) {
            // We can't provide this information, so just return now
            return DateTime.UtcNow;
        }

        public override bool CreateFromAlternateCopy(string file, string source) {
            file = file.Replace('\\', '/');
            source = source.Replace('\\', '/');
            CopyObjectRequest request = new CopyObjectRequest() {
                SourceBucket = this.bucket,
                DestinationBucket = this.bucket,
                SourceKey = this.prefix + source,
                DestinationKey = this.prefix + file
            };
            this.withHandling(() => this.client.CopyObject(request), file);
            this.files.Add(file);
            return true;
        }

        public override void CreateFromAlternateMove(string file, string source) {
            this.CreateFromAlternateCopy(file, source);
            this.withHandling(() => this.DeleteFile(source), file);
        }

        public override IEnumerable<EntityRecord> ListFilesFolders() {
            // Iterate folders, and for each folder, iterate files which begin with that folder
            // probably isn't the most efficient way due to searching, but hey
            // Sort folders first, just to be sure (I think S3 sorts anyway, but hey). Nothing else minds it being sorted
            this.folders.Sort();
            foreach (string folder in this.folders) {
                string folderReversedSlashes = folder.Replace('/', '\\');
                yield return new EntityRecord(folderReversedSlashes, Entity.Folder);
                foreach (string file in this.files.Where(x => Path.GetDirectoryName(x) == folderReversedSlashes)) {
                    yield return new EntityRecord(file, Entity.File);
                }
            }
        }

        public override void PurgeFiles(IEnumerable<string> filesIn, IEnumerable<string> foldersIn, PurgeProgressHandler handler=null) {
            HashSet<string> limitFiles = new HashSet<string>(filesIn.Select(x => x.Replace('\\', '/')));
            HashSet<string> limitFolders = new HashSet<string>(foldersIn.Select(x => x.Replace('\\', '/')));

            // We can't modify the list we're iterating over, and DeleteFile does modify it
            string[] iterateFiles = this.files.ToArray();
            foreach (string file in iterateFiles) {
                if (!limitFiles.Contains(file)) {
                    this.DeleteFile(file);
                    if (handler != null && !handler(Entity.File, file, true))
                        return;
                }
                else {
                    if (handler != null && !handler(Entity.File, file, false))
                        return;
                }
            }

            string[] iterateFolders = this.folders.ToArray();
            foreach (string folder in iterateFolders) {
                if (!limitFolders.Contains(folder)) {
                    this.DeleteFolder(folder);
                    if (handler != null && !handler(Entity.Folder, folder, true))
                        return;
                }
                else {
                    if (handler != null && !handler(Entity.Folder, folder, false))
                        return;
                }
            }
        }

        private void withHandling(Action action, string errorFile) {
            try {
                action();
            }
            catch (AmazonS3Exception e) { throw new BackupOperationException(errorFile, e.Message); }
            catch (System.Net.WebException e) { throw new BackupOperationException(errorFile, e.Message); }
        }
    }
}
