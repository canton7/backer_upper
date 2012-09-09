using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BackerUpper
{
    public partial class TreeBrowserForm : Form
    {
        public bool Saved { get; private set; }
        public string Source { get; private set; }
        public string IgnorePattern { get; private set; }
        public IEnumerable<string> IgnoredFiles { get; private set; }
        public IEnumerable<string> IgnoredFolders { get; private set; }

        public TreeBrowserForm(string source, string ignorePattern, IEnumerable<string> ignoredFiles, IEnumerable<string> ignoredFolders) {
            InitializeComponent();

            this.IgnoredFiles = ignoredFiles;
            this.IgnoredFolders = ignoredFolders;
            this.fileTreeBrowser.IgnoredFiles = new HashSet<string>(this.IgnoredFiles);
            this.fileTreeBrowser.IgnoredFolders = new HashSet<string>(this.IgnoredFolders);

            this.textBoxSource.Text = source;
            this.textBoxIgnorePattern.Text = ignorePattern;
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            this.sourceBrowser.SelectedPath = this.textBoxSource.Text;
            DialogResult result = this.sourceBrowser.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
                return;
            this.textBoxSource.Text = this.sourceBrowser.SelectedPath;
        }

        private void textBoxSource_TextChanged(object sender, EventArgs e) {
            if (!Directory.Exists(this.textBoxSource.Text)) {
                this.fileTreeBrowser.Clear();
                this.labelValidSource.Visible = true;
                return;
            }

            this.labelValidSource.Visible = false;
            this.fileTreeBrowser.Populate(this.textBoxSource.Text.Trim(), this.textBoxIgnorePattern.Text);
        }

        private void textBoxIgnorePattern_TextChanged(object sender, EventArgs e) {
            this.fileTreeBrowser.Populate(this.textBoxSource.Text.Trim(), this.textBoxIgnorePattern.Text);
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            FileTreeBrowser.IgnoredFilesFolders ignored = this.fileTreeBrowser.GetIgnoredFilesFolders();
            this.Source = this.textBoxSource.Text;
            this.IgnorePattern = this.textBoxIgnorePattern.Text;
            this.IgnoredFiles = ignored.Files;
            this.IgnoredFolders = ignored.Folders;
            this.Saved = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
