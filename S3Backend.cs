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

        public S3Backend(string dest, string publicKey, string privateKey)
            : base(dest) {
            string[] parts = dest.Split(new char[] {'/'}, 2);
            this.bucket = parts[0];
            this.prefix = parts.Length > 1 ? parts[1].TrimEnd(new char[]{ '/' }) + '/' : "/";

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

        public override void CreateFolder(string folder) {
            folder = folder.Replace('\\', '/');
            PutObjectRequest request = new PutObjectRequest() {
                BucketName = this.bucket,
                Key = this.prefix + folder + '/',
                ContentBody = ""
            };
            this.client.PutObject(request);
            this.folders.Add(folder);
        }

        public override void DeleteFolder(string folder) {
            folder = folder.Replace('\\', '/');
            DeleteObjectRequest request = new DeleteObjectRequest() {
                BucketName = this.bucket,
                Key = (this.prefix + folder + '/')
            };
            this.client.DeleteObject(request);
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
                GetObjectMetadataRequest metaRequest = new GetObjectMetadataRequest() {
                    BucketName = this.bucket,
                    Key = key
                };
                GetObjectMetadataResponse metaResponse = client.GetObjectMetadata(metaRequest);
                string destMD5 = metaResponse.Headers["x-amz-meta-md5"];
                if (destMD5 == fileMD5)
                    return;
            }

            PutObjectRequest putRequest = new PutObjectRequest() {
                BucketName = this.bucket,
                FilePath = source,
                Key = key,
                Timeout = -1,
                CannedACL = S3CannedACL.Private
            };
            putRequest.AddHeader("x-amz-meta-md5", fileMD5);
            this.client.PutObject(putRequest);
            this.files.Add(file);
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
            this.client.DeleteObject(request);
            this.files.Remove(file);
        }

        public override bool FileExists(string file) {
            file = file.Replace('\\', '/');
            return this.files.Contains(file);
        }

        public override void CreateFromAlternateCopy(string file, string source) {
            file = file.Replace('\\', '/');
            source = source.Replace('\\', '/');
            CopyObjectRequest request = new CopyObjectRequest() {
                SourceBucket = this.bucket,
                DestinationBucket = this.bucket,
                SourceKey = this.prefix + source,
                DestinationKey = this.prefix + file
            };
            this.client.CopyObject(request);
            this.files.Add(file);
        }

        public override void CreateFromAlternateMove(string file, string source) {
            this.CreateFromAlternateCopy(file, source);
            this.DeleteFile(source);
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
    }
}
