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
        // http://sharpertutorials.com/calculate-md5-checksum-file/
        public static string FileMD5(string path) {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retval = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retval.Length; i++) {
                sb.Append(retval[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
