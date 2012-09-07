using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            string backup = args.FirstOrDefault(x => x.StartsWith("--backup="));
            if (backup != null) backup = backup.Substring(9).Trim(new char[] { '\'', '"' });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main(backup));
        }
    }
}
