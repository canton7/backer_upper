using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using RestSharp;
using System.Threading;

namespace BackerUpper
{
    class VersionChecker
    {
        // bool handler(Version newVersion), returns true to download
        public static void Check(Func<Version, bool> handler) {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(Constants.GITHUB_DOWNLOADS_URL, Method.GET);
            IRestResponse<List<DownloadItem>> response = client.Execute<List<DownloadItem>>(request);
            if (response.Data == null)
                return;
            Version currentVersion = CurrentOrLastPromptedVersion();
            DownloadItem newestOnline = response.Data.OrderByDescending(x => x.version).First();
            // Up to date?
            if (newestOnline.version <= currentVersion)
                return;

            string versionFile = Path.Combine(Constants.APPDATA_FOLDER, Constants.VERSION_FILE);

            if (handler(newestOnline.version)) {
                Process.Start(newestOnline.html_url);
                // Try to delete version file
                try {
                    File.Delete(versionFile);
                }
                catch (IOException) { }
            }
            else {
                // User isn't interested. Write to ignore file
                try {
                    File.WriteAllText(versionFile, newestOnline.version.ToString());
                }
                catch (IOException) { }
            }
        }

        public static void CheckAsync(Func<Version, bool> handler) {
            Thread thread = new Thread(() => {
                Check(handler);
            });
            thread.Start();
        }

        public static Version CurrentOrLastPromptedVersion() {
            // Returns the last version we prompted the user to upgrade to, or the current version if none
            string versionFile = Path.Combine(Constants.APPDATA_FOLDER, Constants.VERSION_FILE);
            Version fileVersion = null;
            if (File.Exists(versionFile)) {
                string fileVersionStr = File.ReadLines(versionFile).First();
                try {
                    fileVersion = new Version(fileVersionStr);
                }
                catch (FormatException) {
                    // Bad file? ignore and (try to) delete it
                    try {
                        File.Delete(versionFile);
                    }
                    catch (IOException) { }
                }
            }
            Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;

            // If fileVersion exists and is newer than currentVersion, return this
            return (fileVersion != null && fileVersion > currentVersion) ? fileVersion : currentVersion;
        }

        private struct DownloadItem
        {
            public string html_url { get; set; }
            public string content_type { get; set; }
            public string description { get; set; }
            public int size { get; set; }
            public string created_at { get; set; }
            public string name { get; set; }
            public int download_count { get; set; }
            public int id { get; set; }
            public string url { get; set; }

            private Version versionCache;
            public Version version {
                get {
                    if (versionCache != null)
                        return versionCache;
                    Regex r = new Regex(@"v(\d+\.\d+\.\d+)");
                    string versionStr = r.Match(this.name).Groups[1].ToString();
                    try {
                        versionCache = new Version(versionStr);
                    }
                    catch (ArgumentException) {
                        versionCache = new Version(0, 0, 0);
                    }
                    return versionCache;
                }
            }
        }
    }
}
