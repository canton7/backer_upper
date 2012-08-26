using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace BackerUpper
{
    class FileUtils
    {
        public delegate bool HashProgress(int percent);

        // Thanks to http://www.infinitec.de/post/2007/06/09/Displaying-progress-updates-when-hashing-large-files.aspx
        public static string FileMD5(string path, HashProgress hander = null) {
            byte[] buffer;
            byte[] oldBuffer;
            int bytesRead;
            int oldBytesRead;
            long size;
            long totalBytesRead = 0;
            StringBuilder result;
            int prevPrecentage = -1;
            int percentage;
            bool cancel = false;
            using (Stream stream = File.OpenRead(path)) 
            using (HashAlgorithm hashAlgorithm = MD5.Create()) {
                size = stream.Length;
                buffer = new byte[8192];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;
                do {
                    oldBytesRead = bytesRead;
                    oldBuffer = buffer;
                    buffer = new byte[8192];
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    totalBytesRead += bytesRead;
                    if (bytesRead == 0) {
                        hashAlgorithm.TransformFinalBlock(oldBuffer, 0, oldBytesRead);
                    }
                    else {
                        hashAlgorithm.TransformBlock(oldBuffer, 0, oldBytesRead, oldBuffer, 0);
                    }
                    if (hander != null) {
                        // Stop ourselves firing handlers all the bloody time
                        percentage = (int)((double)totalBytesRead * 100 / size);
                        if (percentage > prevPrecentage) {
                            cancel = !hander(percentage);
                            prevPrecentage = percentage;
                        }
                    }
                    //BackgroundWorker.ReportProgress((int)​((double)totalBytesRead * 100 / size));
                } while (!cancel && bytesRead != 0);
                if (cancel)
                    return null;
                byte[] finalBytes = hashAlgorithm.Hash;
                result = new StringBuilder(finalBytes.Length * 2);
                foreach (byte b in finalBytes)
                    result.AppendFormat("{0:x2}", b);
            }
            return result.ToString();
        }
    }
}
