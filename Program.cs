using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace BackerUpper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            // Hacky option parser
            string backupToOpen = args.FirstOrDefault(x => Path.GetExtension(x) == Constants.BACKUP_EXTENSION);
            string backupToRun = args.FirstOrDefault(x => x.StartsWith("--backup="));
            if (backupToRun != null) backupToRun = backupToRun.Substring(9).Trim(new char[] { '\'', '"' });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main(backupToOpen, backupToRun));
        }
    }
}
