namespace BackerUpper
{
    partial class Properties
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
            this.sourceBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.destBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMirrorDest = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSourceBrowser = new System.Windows.Forms.Button();
            this.buttonDestBrowser = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBoxMirror = new System.Windows.Forms.GroupBox();
            this.checkBoxMirror = new System.Windows.Forms.CheckBox();
            this.checkBoxS3 = new System.Windows.Forms.CheckBox();
            this.groupBoxS3 = new System.Windows.Forms.GroupBox();
            this.textBoxS3PrivateKey = new System.Windows.Forms.TextBox();
            this.textBoxS3PublicKey = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxS3Dest = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBoxMirror.SuspendLayout();
            this.groupBoxS3.SuspendLayout();
            this.SuspendLayout();
            // 
            // sourceBrowser
            // 
            this.sourceBrowser.Description = "Select folder to backup";
            // 
            // destBrowser
            // 
            this.destBrowser.Description = "Select where backup is stored";
            // 
            // textBoxSource
            // 
            this.textBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSource.Location = new System.Drawing.Point(47, 19);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(344, 20);
            this.textBoxSource.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Folder";
            // 
            // textBoxMirrorDest
            // 
            this.textBoxMirrorDest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMirrorDest.Location = new System.Drawing.Point(47, 19);
            this.textBoxMirrorDest.Name = "textBoxMirrorDest";
            this.textBoxMirrorDest.Size = new System.Drawing.Size(344, 20);
            this.textBoxMirrorDest.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Folder";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(93, 24);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(340, 20);
            this.textBoxName.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Backup Name";
            // 
            // buttonSourceBrowser
            // 
            this.buttonSourceBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSourceBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSourceBrowser.Image = global::backer_upper.Properties.Resources.IconFolderOpen;
            this.buttonSourceBrowser.Location = new System.Drawing.Point(397, 17);
            this.buttonSourceBrowser.Name = "buttonSourceBrowser";
            this.buttonSourceBrowser.Size = new System.Drawing.Size(26, 23);
            this.buttonSourceBrowser.TabIndex = 2;
            this.buttonSourceBrowser.UseVisualStyleBackColor = true;
            this.buttonSourceBrowser.Click += new System.EventHandler(this.buttonSourceBrowser_Click);
            // 
            // buttonDestBrowser
            // 
            this.buttonDestBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDestBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDestBrowser.Image = global::backer_upper.Properties.Resources.IconFolderOpen;
            this.buttonDestBrowser.Location = new System.Drawing.Point(397, 17);
            this.buttonDestBrowser.Name = "buttonDestBrowser";
            this.buttonDestBrowser.Size = new System.Drawing.Size(26, 23);
            this.buttonDestBrowser.TabIndex = 2;
            this.buttonDestBrowser.UseVisualStyleBackColor = true;
            this.buttonDestBrowser.Click += new System.EventHandler(this.buttonDestBrowser_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(4, 311);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(85, 311);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBoxSource);
            this.groupBox1.Controls.Add(this.buttonSourceBrowser);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 52);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // groupBoxMirror
            // 
            this.groupBoxMirror.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMirror.Controls.Add(this.label2);
            this.groupBoxMirror.Controls.Add(this.textBoxMirrorDest);
            this.groupBoxMirror.Controls.Add(this.buttonDestBrowser);
            this.groupBoxMirror.Location = new System.Drawing.Point(4, 131);
            this.groupBoxMirror.Name = "groupBoxMirror";
            this.groupBoxMirror.Size = new System.Drawing.Size(429, 49);
            this.groupBoxMirror.TabIndex = 7;
            this.groupBoxMirror.TabStop = false;
            this.groupBoxMirror.Text = "Mirror Destination";
            // 
            // checkBoxMirror
            // 
            this.checkBoxMirror.AutoSize = true;
            this.checkBoxMirror.Location = new System.Drawing.Point(12, 108);
            this.checkBoxMirror.Name = "checkBoxMirror";
            this.checkBoxMirror.Size = new System.Drawing.Size(120, 17);
            this.checkBoxMirror.TabIndex = 0;
            this.checkBoxMirror.Text = "Use Mirror Backend";
            this.checkBoxMirror.UseVisualStyleBackColor = true;
            this.checkBoxMirror.CheckedChanged += new System.EventHandler(this.checkBoxMirror_CheckedChanged);
            // 
            // checkBoxS3
            // 
            this.checkBoxS3.AutoSize = true;
            this.checkBoxS3.Location = new System.Drawing.Point(12, 187);
            this.checkBoxS3.Name = "checkBoxS3";
            this.checkBoxS3.Size = new System.Drawing.Size(107, 17);
            this.checkBoxS3.TabIndex = 8;
            this.checkBoxS3.Text = "Use S3 Backend";
            this.checkBoxS3.UseVisualStyleBackColor = true;
            this.checkBoxS3.CheckedChanged += new System.EventHandler(this.checkBoxS3_CheckedChanged);
            // 
            // groupBoxS3
            // 
            this.groupBoxS3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxS3.Controls.Add(this.textBoxS3PrivateKey);
            this.groupBoxS3.Controls.Add(this.textBoxS3PublicKey);
            this.groupBoxS3.Controls.Add(this.label6);
            this.groupBoxS3.Controls.Add(this.label5);
            this.groupBoxS3.Controls.Add(this.textBoxS3Dest);
            this.groupBoxS3.Controls.Add(this.label4);
            this.groupBoxS3.Location = new System.Drawing.Point(4, 211);
            this.groupBoxS3.Name = "groupBoxS3";
            this.groupBoxS3.Size = new System.Drawing.Size(423, 94);
            this.groupBoxS3.TabIndex = 9;
            this.groupBoxS3.TabStop = false;
            this.groupBoxS3.Text = "S3 Destination";
            // 
            // textBoxS3PrivateKey
            // 
            this.textBoxS3PrivateKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxS3PrivateKey.Location = new System.Drawing.Point(75, 69);
            this.textBoxS3PrivateKey.Name = "textBoxS3PrivateKey";
            this.textBoxS3PrivateKey.Size = new System.Drawing.Size(342, 20);
            this.textBoxS3PrivateKey.TabIndex = 4;
            // 
            // textBoxS3PublicKey
            // 
            this.textBoxS3PublicKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxS3PublicKey.Location = new System.Drawing.Point(75, 43);
            this.textBoxS3PublicKey.Name = "textBoxS3PublicKey";
            this.textBoxS3PublicKey.Size = new System.Drawing.Size(342, 20);
            this.textBoxS3PublicKey.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Private Key";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Public Key";
            // 
            // textBoxS3Dest
            // 
            this.textBoxS3Dest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxS3Dest.Location = new System.Drawing.Point(75, 17);
            this.textBoxS3Dest.Name = "textBoxS3Dest";
            this.textBoxS3Dest.Size = new System.Drawing.Size(342, 20);
            this.textBoxS3Dest.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Location";
            // 
            // Properties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 342);
            this.Controls.Add(this.groupBoxS3);
            this.Controls.Add(this.checkBoxS3);
            this.Controls.Add(this.checkBoxMirror);
            this.Controls.Add(this.groupBoxMirror);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxName);
            this.Name = "Properties";
            this.Text = "Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxMirror.ResumeLayout(false);
            this.groupBoxMirror.PerformLayout();
            this.groupBoxS3.ResumeLayout(false);
            this.groupBoxS3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog sourceBrowser;
        private System.Windows.Forms.FolderBrowserDialog destBrowser;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMirrorDest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSourceBrowser;
        private System.Windows.Forms.Button buttonDestBrowser;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxMirror;
        private System.Windows.Forms.CheckBox checkBoxMirror;
        private System.Windows.Forms.CheckBox checkBoxS3;
        private System.Windows.Forms.GroupBox groupBoxS3;
        private System.Windows.Forms.TextBox textBoxS3PrivateKey;
        private System.Windows.Forms.TextBox textBoxS3PublicKey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxS3Dest;
        private System.Windows.Forms.Label label4;
    }
}