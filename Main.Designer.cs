namespace BackerUpper
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.backupsList = new System.Windows.Forms.ListBox();
            this.buttonBackup = new System.Windows.Forms.Button();
            this.buttonProperties = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabelBackupAction = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.backgroundWorkerBackup = new System.ComponentModel.BackgroundWorker();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelLastRun = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSource = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelDest = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelStats = new System.Windows.Forms.Label();
            this.buttonImport = new System.Windows.Forms.Button();
            this.openFileDialogImport = new System.Windows.Forms.OpenFileDialog();
            this.buttonViewLogs = new System.Windows.Forms.Button();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.labelVersion = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // backupsList
            // 
            this.backupsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.backupsList.FormattingEnabled = true;
            this.backupsList.Location = new System.Drawing.Point(12, 12);
            this.backupsList.Name = "backupsList";
            this.backupsList.Size = new System.Drawing.Size(319, 82);
            this.backupsList.TabIndex = 0;
            this.backupsList.SelectedIndexChanged += new System.EventHandler(this.backupsList_SelectedIndexChanged);
            // 
            // buttonBackup
            // 
            this.buttonBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBackup.Location = new System.Drawing.Point(12, 100);
            this.buttonBackup.Name = "buttonBackup";
            this.buttonBackup.Size = new System.Drawing.Size(75, 23);
            this.buttonBackup.TabIndex = 1;
            this.buttonBackup.Text = "Backup";
            this.buttonBackup.UseVisualStyleBackColor = true;
            this.buttonBackup.Click += new System.EventHandler(this.buttonBackup_Click);
            // 
            // buttonProperties
            // 
            this.buttonProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonProperties.Location = new System.Drawing.Point(255, 100);
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.Size = new System.Drawing.Size(75, 23);
            this.buttonProperties.TabIndex = 2;
            this.buttonProperties.Text = "Properties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreate.Location = new System.Drawing.Point(337, 13);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 3;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.CanOverflow = true;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelBackupAction,
            this.statusLabelTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 258);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(424, 24);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelBackupAction
            // 
            this.statusLabelBackupAction.Name = "statusLabelBackupAction";
            this.statusLabelBackupAction.Size = new System.Drawing.Size(371, 19);
            this.statusLabelBackupAction.Spring = true;
            this.statusLabelBackupAction.Text = "Idle";
            this.statusLabelBackupAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabelTime
            // 
            this.statusLabelTime.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.statusLabelTime.Name = "statusLabelTime";
            this.statusLabelTime.Size = new System.Drawing.Size(38, 19);
            this.statusLabelTime.Text = "00:00";
            // 
            // backgroundWorkerBackup
            // 
            this.backgroundWorkerBackup.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerBackup_DoWork);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(174, 100);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.Location = new System.Drawing.Point(337, 229);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Location = new System.Drawing.Point(337, 42);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 6;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Last Run:";
            // 
            // labelLastRun
            // 
            this.labelLastRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLastRun.AutoSize = true;
            this.labelLastRun.Location = new System.Drawing.Point(68, 139);
            this.labelLastRun.Name = "labelLastRun";
            this.labelLastRun.Size = new System.Drawing.Size(36, 13);
            this.labelLastRun.TabIndex = 8;
            this.labelLastRun.Text = "Never";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Source:";
            // 
            // labelSource
            // 
            this.labelSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSource.AutoSize = true;
            this.labelSource.Location = new System.Drawing.Point(68, 156);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(33, 13);
            this.labelSource.TabIndex = 10;
            this.labelSource.Text = "None";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Dest:";
            // 
            // labelDest
            // 
            this.labelDest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDest.AutoSize = true;
            this.labelDest.Location = new System.Drawing.Point(68, 190);
            this.labelDest.Name = "labelDest";
            this.labelDest.Size = new System.Drawing.Size(33, 13);
            this.labelDest.TabIndex = 12;
            this.labelDest.Text = "None";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 173);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Stats:";
            // 
            // labelStats
            // 
            this.labelStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStats.AutoSize = true;
            this.labelStats.Location = new System.Drawing.Point(68, 173);
            this.labelStats.Name = "labelStats";
            this.labelStats.Size = new System.Drawing.Size(33, 13);
            this.labelStats.TabIndex = 14;
            this.labelStats.Text = "None";
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImport.Location = new System.Drawing.Point(337, 71);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 15;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonViewLogs
            // 
            this.buttonViewLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonViewLogs.Location = new System.Drawing.Point(337, 100);
            this.buttonViewLogs.Name = "buttonViewLogs";
            this.buttonViewLogs.Size = new System.Drawing.Size(75, 23);
            this.buttonViewLogs.TabIndex = 17;
            this.buttonViewLogs.Text = "View Logs";
            this.buttonViewLogs.UseVisualStyleBackColor = true;
            this.buttonViewLogs.Click += new System.EventHandler(this.buttonViewLogs_Click);
            // 
            // buttonRestore
            // 
            this.buttonRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRestore.Location = new System.Drawing.Point(93, 100);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(75, 23);
            this.buttonRestore.TabIndex = 18;
            this.buttonRestore.Text = "Restore";
            this.buttonRestore.UseVisualStyleBackColor = true;
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonHelp.Location = new System.Drawing.Point(337, 129);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(75, 23);
            this.buttonHelp.TabIndex = 19;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(9, 234);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(75, 13);
            this.labelVersion.TabIndex = 20;
            this.labelVersion.Text = "Version v0.0.0";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 282);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonRestore);
            this.Controls.Add(this.buttonViewLogs);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.labelStats);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelDest);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelLastRun);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonProperties);
            this.Controls.Add(this.buttonBackup);
            this.Controls.Add(this.backupsList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(440, 320);
            this.Name = "Main";
            this.Text = "Backer Upper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox backupsList;
        private System.Windows.Forms.Button buttonBackup;
        private System.Windows.Forms.Button buttonProperties;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelBackupAction;
        private System.ComponentModel.BackgroundWorker backgroundWorkerBackup;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelTime;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLastRun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelDest;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelStats;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.OpenFileDialog openFileDialogImport;
        private System.Windows.Forms.Button buttonViewLogs;
        private System.Windows.Forms.Button buttonRestore;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Label labelVersion;

    }
}

