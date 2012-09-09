using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BackerUpper
{
    // Turns out it's a bad idea to subclass TreeView. Things get very confused
    public partial class FileTreeBrowser : UserControl
    {
        public FileTreeBrowser() {
            InitializeComponent();

            this.tree.StateImageList = new ImageList();

            // Indexes line up to CheckedState indexes
            this.tree.StateImageList.Images.Add(this.drawCheckBox(CheckedState.Unchecked));
            this.tree.StateImageList.Images.Add(this.drawCheckBox(CheckedState.Checked));
            this.tree.StateImageList.Images.Add(this.drawCheckBox(CheckedState.Mixed));

            this.tree.ImageList = new ImageList();
            this.tree.ImageList.Images.Add(new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("BackerUpper.Resources.IconFolderOpen.gif")));
        }

        public void Clear() {
            this.tree.Nodes.Clear();
        }

        public void Populate(string startDir, string ignoreRules) {
            // Get rid of anything there before
            this.tree.Nodes.Clear();
            // aaand start again!
            TreeTraverser treeTraverser = new TreeTraverser(startDir, ignoreRules);
            TreeNodeTri node;
            TreeTraverser.FolderEntry root = treeTraverser.ListFolders(0).First();
            foreach (TreeTraverser.FolderEntry folder in root.GetFolders()) {
                node = new TreeNodeTri(folder, this.tree.ImageList);
                node.Populate();
                this.tree.Nodes.Add(node);
            }
            foreach (TreeTraverser.FileEntry file in root.GetFiles()) {
                this.tree.Nodes.Add(new TreeNodeTri(file, this.tree.ImageList));
            }
        }

        public IgnoredFilesFolders GetIgnoredFilesFolders() {
            List<string> ignoredFiles = new List<string>();
            List<string> ignoredFolders = new List<string>();

            foreach (TreeNodeTri node in this.tree.Nodes) {
                node.IgnoresPaths(ignoredFolders, ignoredFiles);
            }

            return new IgnoredFilesFolders(ignoredFolders, ignoredFiles);
        }

        private Bitmap drawCheckBox(CheckedState state) {
            Bitmap bmp = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bmp);

            System.Windows.Forms.VisualStyles.CheckBoxState boxState = state == CheckedState.Unchecked ? 
                System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal : (state == CheckedState.Checked ? 
                System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal : System.Windows.Forms.VisualStyles.CheckBoxState.MixedNormal);

            CheckBoxRenderer.DrawCheckBox(Graphics.FromImage(bmp), new Point(0, 1), boxState);
            return bmp;
        }

        private void tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            // is the click on the checkbox?  If not, discard it
            System.Windows.Forms.TreeViewHitTestInfo info = this.tree.HitTest(e.X, e.Y);
            if (info == null || info.Location != System.Windows.Forms.TreeViewHitTestLocations.StateImage) {
                return;
            }
            ((TreeNodeTri)e.Node).Click();
        }

        private void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            ((TreeNodeTri)e.Node).PopulateChildren();
        }

        public struct IgnoredFilesFolders
        {
            public IEnumerable<string> Files;
            public IEnumerable<string> Folders;

            public IgnoredFilesFolders(IEnumerable<string> folders, IEnumerable<string> files) {
                this.Folders = folders;
                this.Files = files;
            }
        }


        private enum CheckedState { Unchecked = 0, Checked = 1, Mixed = 2 };

        private class TreeNodeTri : TreeNode
        {
            public CheckedState State { get; private set; }
            private TreeTraverser.FolderEntry folderEntry;
            private TreeTraverser.FileEntry fileEntry;
            private ImageList imageList;

            public TreeNodeTri(TreeTraverser.FolderEntry folderEntry, ImageList imageList)
                    : base(folderEntry.Name) {

                this.imageList = imageList;
                this.State = CheckedState.Checked;
                this.StateImageIndex = (int)CheckedState.Checked;

                this.folderEntry = folderEntry;
                this.ImageIndex = 0;
                this.SelectedImageIndex = 0;
            }

            public TreeNodeTri(TreeTraverser.FileEntry fileEntry, ImageList imageList)
                    : base(fileEntry.Filename) {

                this.imageList = imageList;
                this.State = CheckedState.Checked;
                this.StateImageIndex = (int)CheckedState.Checked;

                this.fileEntry = fileEntry;
                
                if (!imageList.Images.ContainsKey(fileEntry.Extension)) {
                    Icon icon = Icon.ExtractAssociatedIcon(fileEntry.FullPath);
                    imageList.Images.Add(fileEntry.Extension, icon);
                }
                this.ImageKey = fileEntry.Extension;
                this.SelectedImageKey = fileEntry.Extension;
            }

            public void Populate() {
                if (this.folderEntry == null || this.Nodes.Count > 0)
                    return;

                foreach (TreeTraverser.FolderEntry folder in this.folderEntry.GetFolders()) {
                    this.Nodes.Add(new TreeNodeTri(folder, this.imageList));
                }
                foreach (TreeTraverser.FileEntry file in this.folderEntry.GetFiles()) {
                    this.Nodes.Add(new TreeNodeTri(file, this.imageList));
                }
            }

            public void PopulateChildren() {
                foreach (TreeNodeTri node in this.Nodes) {
                    node.Populate();
                }
            }

            public void Click() {
                CheckedState state = this.State == CheckedState.Checked ? CheckedState.Unchecked : CheckedState.Checked;
                this.ChangeState(state);
                this.UpdateParent();
                this.UpdateChildren();
            }

            public void ChangeState(CheckedState state) {
                this.State = state;
                this.StateImageIndex = (int)state;
                this.UpdateChildren();
            }

            public void UpdateChildren() {
                if (this.State != CheckedState.Mixed) {
                    // Now, children time! All children have the same state as us!
                    foreach (TreeNodeTri node in this.Nodes) {
                        node.ChangeState(this.State);
                    }
                }
            }

            public void UpdateParent() {
                if (this.Parent != null)
                    ((TreeNodeTri)this.Parent).ChildChangedState(this.State);
            }

            public void ChildChangedState(CheckedState childState) {
                // If all children checked, then we're checked. Ditto unchecked. If bit of both or neither, then we're mixed
                CheckedState state = childState; // Good starting point, at least one node has this state
                foreach (TreeNodeTri node in this.Nodes) {
                    if (state != node.State) {
                        state = CheckedState.Mixed;
                        break;
                    }
                }

                this.ChangeState(state);
                this.UpdateParent();
            }

            public void IgnoresPaths(List<string> ignoredFolders, List<string> ignoredFiles) {
                if (this.folderEntry == null) {
                    // We're a file
                    if (this.State == CheckedState.Unchecked)
                        ignoredFiles.Add(this.fileEntry.RelPath);
                }
                else {
                    // We're a folder
                    // If we're ignored, add ourselves and quit
                    // Otherwise, recurse
                    if (this.State == CheckedState.Unchecked)
                        ignoredFolders.Add(this.folderEntry.RelPath);
                    else {
                        foreach (TreeNodeTri node in this.Nodes) {
                            node.IgnoresPaths(ignoredFolders, ignoredFiles);
                        }
                    }
                }
            }
        }

    }
}
