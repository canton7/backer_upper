namespace BackerUpper
{
    partial class TreeBrowserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeBrowserForm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.sourceBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.labelValidSource = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIgnorePattern = new System.Windows.Forms.TextBox();
            this.fileTreeBrowser = new BackerUpper.FileTreeBrowser();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Location = new System.Drawing.Point(12, 442);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxSource
            // 
            this.textBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSource.Location = new System.Drawing.Point(12, 43);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(259, 20);
            this.textBoxSource.TabIndex = 2;
            this.textBoxSource.TextChanged += new System.EventHandler(this.textBoxSource_TextChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Image = global::BackerUpper.Properties.Resources.IconFolderOpen;
            this.buttonBrowse.Location = new System.Drawing.Point(277, 40);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(26, 23);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // sourceBrowser
            // 
            this.sourceBrowser.Description = "Select folder to backup";
            // 
            // labelValidSource
            // 
            this.labelValidSource.AutoSize = true;
            this.labelValidSource.BackColor = System.Drawing.Color.White;
            this.labelValidSource.Location = new System.Drawing.Point(16, 75);
            this.labelValidSource.Name = "labelValidSource";
            this.labelValidSource.Size = new System.Drawing.Size(139, 13);
            this.labelValidSource.TabIndex = 4;
            this.labelValidSource.Text = "Please select a valid source";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.Location = new System.Drawing.Point(93, 440);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 417);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Filter";
            // 
            // textBoxIgnorePattern
            // 
            this.textBoxIgnorePattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIgnorePattern.Location = new System.Drawing.Point(47, 414);
            this.textBoxIgnorePattern.Name = "textBoxIgnorePattern";
            this.textBoxIgnorePattern.Size = new System.Drawing.Size(257, 20);
            this.textBoxIgnorePattern.TabIndex = 7;
            this.textBoxIgnorePattern.TextChanged += new System.EventHandler(this.textBoxIgnorePattern_TextChanged);
            // 
            // fileTreeBrowser
            // 
            this.fileTreeBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTreeBrowser.Location = new System.Drawing.Point(12, 69);
            this.fileTreeBrowser.Name = "fileTreeBrowser";
            this.fileTreeBrowser.Size = new System.Drawing.Size(292, 339);
            this.fileTreeBrowser.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(252, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Select exactly which files and folders get backed up";
            // 
            // TreeBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 477);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIgnorePattern);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelValidSource);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxSource);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.fileTreeBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TreeBrowserForm";
            this.Text = "Advanced Source Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FileTreeBrowser fileTreeBrowser;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.FolderBrowserDialog sourceBrowser;
        private System.Windows.Forms.Label labelValidSource;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIgnorePattern;
        private System.Windows.Forms.Label label2;
    }
}