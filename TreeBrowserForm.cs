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
    public partial class TreeBrowserForm : Form
    {
        public TreeBrowserForm() {
            InitializeComponent();
            this.fileTreeBrowser1.Setup(@"D:\Users\Antony");
        }

        private void button1_Click(object sender, EventArgs e) {
            FileTreeBrowser.IgnoredFilesFolders ignored = this.fileTreeBrowser1.GetIgnoredFilesFolders();
        }
    }
}
