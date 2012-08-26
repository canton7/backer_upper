using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace BackerUpper
{
    class S3Backend : BackendBase, IBackend
    {
        private AmazonS3Client client;
        private string bucket;
        private string prefix;
        List<string> files;
        List<string> folders;

        public override string Name {
            get { return "S3"; }
        }

        public override bool StripFilesFoldersOnDBBackup {
            get { return true; }
        }

        public S3Backend(string dest, string publicKey, string privateKey)
            : base(dest) {
            string[] parts = dest.Split(new char[] {'/'}, 2);
            this.bucket = parts[0];
            this.prefix = parts.Length > 1 ? parts[1].Replace('\\', '/').TrimEnd(new char[] { '/' }) + '/' : "/";

            this.client = new AmazonS3Client(publicKey, privateKey);
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
        }

        public override void CreateFolder(string folder) {
            folder = folder.Replace('\\', '/');
            PutObjectRequest request = new PutObjectRequest() {
                BucketName = this.bucket,
                Key = this.prefix + folder + '/',
                ContentBody = ""
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

        public override void CreateFile(string file, string source, string fileMD5) {
            file = file.Replace('\\', '/');
            string key = this.prefix + file;

            if (this.files.Contains(file)) {
                try {
                    GetObjectMetadataRequest metaRequest = new GetObjectMetadataRequest() {
                        BucketName = this.bucket,
                        Key = key
                    };
                    GetObjectMetadataResponse metaResponse = client.GetObjectMetadata(metaRequest);
                    string destMD5 = metaResponse.Headers["x-amz-meta-md5"];
                    if (destMD5 == fileMD5)
                        return;
                }
                catch (AmazonS3Exception e) { throw new BackupOperationException(file, e.Message); }
            }

            PutObjectRequest putRequest = new PutObjectRequest() {
                BucketName = this.bucket,
                FilePath = source,
                Key = key,
                Timeout = -1,
                CannedACL = S3CannedACL.Private
            };
            putRequest.PutObjectProgressEvent += new EventHandler<PutObjectProgressArgs>(putRequest_PutObjectProgressEvent);
            putRequest.AddHeader("x-amz-meta-md5", fileMD5);
            this.withHandling(() => this.client.PutObject(putRequest), file);
            this.files.Add(file);
        }

        public override bool TestFile(string file, DateTime lastModified, string fileMD5) {
            // lastModified isn't much of an indication, since we can't update it at will (updateLastModified does nothing)
            // But we can use the md5 for a good test
            file = file.Replace('\\', '/');
            string key = this.prefix + file;

            if (!this.files.Contains(file))
                throw new BackupOperationException(file, "Could not find file, in order to test");

            try {
                GetObjectMetadataRequest metaRequest = new GetObjectMetadataRequest() {
                    BucketName = this.bucket,
                    Key = key
                };
                GetObjectMetadataResponse metaResponse = client.GetObjectMetadata(metaRequest);
                string destMD5 = metaResponse.Headers["x-amz-meta-md5"];

                return destMD5 == fileMD5;
            }
            catch (AmazonS3Exception e) { throw new BackupOperationException(file, e.Message); }
        }

        public override void BackupDatabase(string file, string source) {
            this.CreateFile(file, source, null);
        }

        private void putRequest_PutObjectProgressEvent(object sender, PutObjectProgressArgs e) {
            this.ReportProcess(e.PercentDone);
        }

        public override void UpdateFile(string file, string source, string fileMD5) {
            this.CreateFile(file, source, fileMD5);
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

        public override void PurgeFiles(IEnumerable<string> filesIn, IEnumerable<string> foldersIn) {
            HashSet<string> limitFiles = new HashSet<string>(filesIn.Select(x => x.Replace('\\', '/')));
            HashSet<string> limitFolders = new HashSet<string>(foldersIn.Select(x => x.Replace('\\', '/')));

            // We can't modify the list we're iterating over, and DeleteFile does modify it
            string[] iterateFiles = this.files.ToArray();
            foreach (string file in iterateFiles) {
                if (!limitFiles.Contains(file)) {
                    this.DeleteFile(file);
                }
            }

            string[] iterateFolders = this.folders.ToArray();
            foreach (string folder in iterateFolders) {
                if (!limitFolders.Contains(folder)) {
                    this.DeleteFolder(folder);
                }
            }
        }

        private void withHandling(Action action, string errorFile) {
            try {
                action();
            }
            catch (AmazonS3Exception e) { throw new BackupOperationException(errorFile, e.Message); }
        }
    }
}
