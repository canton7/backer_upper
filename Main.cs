using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BackerUpper
{
    public partial class Main : Form
    {
        public Main() {
            InitializeComponent();

            MirrorBackend backend = new MirrorBackend(@"D:\Users\Antony\Documents\projects\backer_upper\test_dest");
            FileScanner fileScanner = new FileScanner(@"D:\Users\Antony\Documents\projects\backer_upper\test_src", "test.sqlite", backend);
            fileScanner.PruneDatabase();
            fileScanner.Backup();
            fileScanner.PurgeDest();
        }
    }
}
