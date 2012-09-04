namespace BackerUpper
{
    partial class RestoreForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RestoreForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelBackupName = new System.Windows.Forms.Label();
            this.labelBackupSource = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRestoreTo = new System.Windows.Forms.TextBox();
            this.buttonDest = new System.Windows.Forms.Button();
            this.comboBoxBackends = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.folderBrowserRestoreTo = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            this.checkBoxOverwriteOnlyIfOlder = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Backup name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Source:";
            // 
            // labelBackupName
            // 
            this.labelBackupName.AutoSize = true;
            this.labelBackupName.Location = new System.Drawing.Point(130, 13);
            this.labelBackupName.Name = "labelBackupName";
            this.labelBackupName.Size = new System.Drawing.Size(75, 13);
            this.labelBackupName.TabIndex = 2;
            this.labelBackupName.Text = "Backup Name";
            // 
            // labelBackupSource
            // 
            this.labelBackupSource.AutoSize = true;
            this.labelBackupSource.Location = new System.Drawing.Point(131, 62);
            this.labelBackupSource.Name = "labelBackupSource";
            this.labelBackupSource.Size = new System.Drawing.Size(41, 13);
            this.labelBackupSource.TabIndex = 3;
            this.labelBackupSource.Text = "Source";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Restore To:";
            // 
            // textBoxRestoreTo
            // 
            this.textBoxRestoreTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRestoreTo.Location = new System.Drawing.Point(82, 88);
            this.textBoxRestoreTo.Name = "textBoxRestoreTo";
            this.textBoxRestoreTo.Size = new System.Drawing.Size(236, 20);
            this.textBoxRestoreTo.TabIndex = 5;
            // 
            // buttonDest
            // 
            this.buttonDest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDest.Image = global::BackerUpper.Properties.Resources.IconFolderOpen;
            this.buttonDest.Location = new System.Drawing.Point(324, 86);
            this.buttonDest.Name = "buttonDest";
            this.buttonDest.Size = new System.Drawing.Size(26, 23);
            this.buttonDest.TabIndex = 6;
            this.buttonDest.UseVisualStyleBackColor = true;
            this.buttonDest.Click += new System.EventHandler(this.buttonDest_Click);
            // 
            // comboBoxBackends
            // 
            this.comboBoxBackends.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBackends.FormattingEnabled = true;
            this.comboBoxBackends.Location = new System.Drawing.Point(134, 32);
            this.comboBoxBackends.Name = "comboBoxBackends";
            this.comboBoxBackends.Size = new System.Drawing.Size(71, 21);
            this.comboBoxBackends.TabIndex = 7;
            this.comboBoxBackends.SelectedIndexChanged += new System.EventHandler(this.comboBoxBackends_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Restore from Backend:";
            // 
            // buttonRestore
            // 
            this.buttonRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRestore.Location = new System.Drawing.Point(12, 170);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(75, 23);
            this.buttonRestore.TabIndex = 9;
            this.buttonRestore.Text = "Restore";
            this.buttonRestore.UseVisualStyleBackColor = true;
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.Location = new System.Drawing.Point(93, 170);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxOverwrite
            // 
            this.checkBoxOverwrite.AutoSize = true;
            this.checkBoxOverwrite.Location = new System.Drawing.Point(16, 123);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(216, 17);
            this.checkBoxOverwrite.TabIndex = 11;
            this.checkBoxOverwrite.Text = "Overwrite files which are already present";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            this.checkBoxOverwrite.CheckedChanged += new System.EventHandler(this.checkBoxOverwrite_CheckedChanged);
            // 
            // checkBoxOverwriteOnlyIfOlder
            // 
            this.checkBoxOverwriteOnlyIfOlder.AutoSize = true;
            this.checkBoxOverwriteOnlyIfOlder.Enabled = false;
            this.checkBoxOverwriteOnlyIfOlder.Location = new System.Drawing.Point(31, 145);
            this.checkBoxOverwriteOnlyIfOlder.Name = "checkBoxOverwriteOnlyIfOlder";
            this.checkBoxOverwriteOnlyIfOlder.Size = new System.Drawing.Size(313, 17);
            this.checkBoxOverwriteOnlyIfOlder.TabIndex = 12;
            this.checkBoxOverwriteOnlyIfOlder.Text = "But only if the file present is older than the one being restored";
            this.checkBoxOverwriteOnlyIfOlder.UseVisualStyleBackColor = true;
            // 
            // RestoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 205);
            this.Controls.Add(this.checkBoxOverwriteOnlyIfOlder);
            this.Controls.Add(this.checkBoxOverwrite);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonRestore);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxBackends);
            this.Controls.Add(this.buttonDest);
            this.Controls.Add(this.textBoxRestoreTo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelBackupSource);
            this.Controls.Add(this.labelBackupName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RestoreForm";
            this.Text = "Restore a Backup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelBackupName;
        private System.Windows.Forms.Label labelBackupSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRestoreTo;
        private System.Windows.Forms.Button buttonDest;
        private System.Windows.Forms.ComboBox comboBoxBackends;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserRestoreTo;
        private System.Windows.Forms.Button buttonRestore;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;
        private System.Windows.Forms.CheckBox checkBoxOverwriteOnlyIfOlder;
    }
}