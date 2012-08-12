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
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // backupsList
            // 
            this.backupsList.FormattingEnabled = true;
            this.backupsList.Location = new System.Drawing.Point(12, 12);
            this.backupsList.Name = "backupsList";
            this.backupsList.Size = new System.Drawing.Size(193, 82);
            this.backupsList.TabIndex = 0;
            // 
            // buttonBackup
            // 
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
            this.buttonProperties.Location = new System.Drawing.Point(93, 100);
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.Size = new System.Drawing.Size(75, 23);
            this.buttonProperties.TabIndex = 2;
            this.buttonProperties.Text = "Properties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(212, 12);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 238);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(297, 24);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelBackupAction
            // 
            this.statusLabelBackupAction.Name = "statusLabelBackupAction";
            this.statusLabelBackupAction.Size = new System.Drawing.Size(213, 19);
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
            this.buttonExit.Location = new System.Drawing.Point(12, 212);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 262);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonProperties);
            this.Controls.Add(this.buttonBackup);
            this.Controls.Add(this.backupsList);
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

    }
}

