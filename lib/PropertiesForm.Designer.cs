﻿namespace BackerUpper
{
    partial class PropertiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesForm));
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
            this.textBoxIgnorePattern = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBoxMirror = new System.Windows.Forms.GroupBox();
            this.checkBoxMirror = new System.Windows.Forms.CheckBox();
            this.checkBoxS3 = new System.Windows.Forms.CheckBox();
            this.groupBoxS3 = new System.Windows.Forms.GroupBox();
            this.checkBoxS3Test = new System.Windows.Forms.CheckBox();
            this.linkLabelAWSCredientials = new System.Windows.Forms.LinkLabel();
            this.checkBoxUseRRS = new System.Windows.Forms.CheckBox();
            this.textBoxS3PrivateKey = new System.Windows.Forms.TextBox();
            this.textBoxS3PublicKey = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxS3Dest = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxScheduler = new System.Windows.Forms.GroupBox();
            this.checkBoxSchedulerOnBatteries = new System.Windows.Forms.CheckBox();
            this.checkBoxSchedulerWhenAvailable = new System.Windows.Forms.CheckBox();
            this.checkBoxIgnoreWarnings = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoclose = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePickerScheduleTime = new System.Windows.Forms.DateTimePicker();
            this.checkBoxScheduleSun = new System.Windows.Forms.CheckBox();
            this.checkBoxScheduleSat = new System.Windows.Forms.CheckBox();
            this.checkBoxScheduleFri = new System.Windows.Forms.CheckBox();
            this.checkBoxScheduleThurs = new System.Windows.Forms.CheckBox();
            this.checkBoxScheduleWeds = new System.Windows.Forms.CheckBox();
            this.checkBoxScheduleTues = new System.Windows.Forms.CheckBox();
            this.checkBoxScheduleMon = new System.Windows.Forms.CheckBox();
            this.checkBoxUseScheduler = new System.Windows.Forms.CheckBox();
            this.buttonSourceAdvanced = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBoxMirror.SuspendLayout();
            this.groupBoxS3.SuspendLayout();
            this.groupBoxScheduler.SuspendLayout();
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
            this.textBoxSource.Size = new System.Drawing.Size(293, 20);
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
            this.textBoxMirrorDest.Size = new System.Drawing.Size(293, 20);
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
            this.textBoxName.Size = new System.Drawing.Size(289, 20);
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
            this.buttonSourceBrowser.Image = ((System.Drawing.Image)(resources.GetObject("buttonSourceBrowser.Image")));
            this.buttonSourceBrowser.Location = new System.Drawing.Point(346, 17);
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
            this.buttonDestBrowser.Image = ((System.Drawing.Image)(resources.GetObject("buttonDestBrowser.Image")));
            this.buttonDestBrowser.Location = new System.Drawing.Point(346, 17);
            this.buttonDestBrowser.Name = "buttonDestBrowser";
            this.buttonDestBrowser.Size = new System.Drawing.Size(26, 23);
            this.buttonDestBrowser.TabIndex = 2;
            this.buttonDestBrowser.UseVisualStyleBackColor = true;
            this.buttonDestBrowser.Click += new System.EventHandler(this.buttonDestBrowser_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(12, 518);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(93, 518);
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
            this.groupBox1.Controls.Add(this.buttonSourceAdvanced);
            this.groupBox1.Controls.Add(this.textBoxIgnorePattern);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBoxSource);
            this.groupBox1.Controls.Add(this.buttonSourceBrowser);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 77);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // textBoxIgnorePattern
            // 
            this.textBoxIgnorePattern.Location = new System.Drawing.Point(47, 45);
            this.textBoxIgnorePattern.Name = "textBoxIgnorePattern";
            this.textBoxIgnorePattern.Size = new System.Drawing.Size(212, 20);
            this.textBoxIgnorePattern.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Filter";
            // 
            // groupBoxMirror
            // 
            this.groupBoxMirror.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMirror.Controls.Add(this.label2);
            this.groupBoxMirror.Controls.Add(this.textBoxMirrorDest);
            this.groupBoxMirror.Controls.Add(this.buttonDestBrowser);
            this.groupBoxMirror.Location = new System.Drawing.Point(4, 156);
            this.groupBoxMirror.Name = "groupBoxMirror";
            this.groupBoxMirror.Size = new System.Drawing.Size(378, 49);
            this.groupBoxMirror.TabIndex = 7;
            this.groupBoxMirror.TabStop = false;
            this.groupBoxMirror.Text = "Mirror Destination";
            // 
            // checkBoxMirror
            // 
            this.checkBoxMirror.AutoSize = true;
            this.checkBoxMirror.Location = new System.Drawing.Point(12, 133);
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
            this.checkBoxS3.Location = new System.Drawing.Point(12, 212);
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
            this.groupBoxS3.Controls.Add(this.checkBoxS3Test);
            this.groupBoxS3.Controls.Add(this.linkLabelAWSCredientials);
            this.groupBoxS3.Controls.Add(this.checkBoxUseRRS);
            this.groupBoxS3.Controls.Add(this.textBoxS3PrivateKey);
            this.groupBoxS3.Controls.Add(this.textBoxS3PublicKey);
            this.groupBoxS3.Controls.Add(this.label6);
            this.groupBoxS3.Controls.Add(this.label5);
            this.groupBoxS3.Controls.Add(this.textBoxS3Dest);
            this.groupBoxS3.Controls.Add(this.label4);
            this.groupBoxS3.Location = new System.Drawing.Point(4, 236);
            this.groupBoxS3.Name = "groupBoxS3";
            this.groupBoxS3.Size = new System.Drawing.Size(378, 137);
            this.groupBoxS3.TabIndex = 9;
            this.groupBoxS3.TabStop = false;
            this.groupBoxS3.Text = "S3 Destination";
            // 
            // checkBoxS3Test
            // 
            this.checkBoxS3Test.AutoSize = true;
            this.checkBoxS3Test.Location = new System.Drawing.Point(192, 114);
            this.checkBoxS3Test.Name = "checkBoxS3Test";
            this.checkBoxS3Test.Size = new System.Drawing.Size(127, 17);
            this.checkBoxS3Test.TabIndex = 7;
            this.checkBoxS3Test.Text = "Test for Modifications";
            this.checkBoxS3Test.UseVisualStyleBackColor = true;
            // 
            // linkLabelAWSCredientials
            // 
            this.linkLabelAWSCredientials.AutoSize = true;
            this.linkLabelAWSCredientials.Location = new System.Drawing.Point(72, 92);
            this.linkLabelAWSCredientials.Name = "linkLabelAWSCredientials";
            this.linkLabelAWSCredientials.Size = new System.Drawing.Size(107, 13);
            this.linkLabelAWSCredientials.TabIndex = 6;
            this.linkLabelAWSCredientials.TabStop = true;
            this.linkLabelAWSCredientials.Text = "View your credentials";
            this.linkLabelAWSCredientials.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAWSCredientials_LinkClicked);
            // 
            // checkBoxUseRRS
            // 
            this.checkBoxUseRRS.AutoSize = true;
            this.checkBoxUseRRS.Location = new System.Drawing.Point(11, 114);
            this.checkBoxUseRRS.Name = "checkBoxUseRRS";
            this.checkBoxUseRRS.Size = new System.Drawing.Size(174, 17);
            this.checkBoxUseRRS.TabIndex = 5;
            this.checkBoxUseRRS.Text = "Reduced Redundancy Storage";
            this.checkBoxUseRRS.UseVisualStyleBackColor = true;
            // 
            // textBoxS3PrivateKey
            // 
            this.textBoxS3PrivateKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxS3PrivateKey.Location = new System.Drawing.Point(75, 69);
            this.textBoxS3PrivateKey.Name = "textBoxS3PrivateKey";
            this.textBoxS3PrivateKey.Size = new System.Drawing.Size(297, 20);
            this.textBoxS3PrivateKey.TabIndex = 4;
            // 
            // textBoxS3PublicKey
            // 
            this.textBoxS3PublicKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxS3PublicKey.Location = new System.Drawing.Point(75, 43);
            this.textBoxS3PublicKey.Name = "textBoxS3PublicKey";
            this.textBoxS3PublicKey.Size = new System.Drawing.Size(297, 20);
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
            this.textBoxS3Dest.Size = new System.Drawing.Size(297, 20);
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
            // groupBoxScheduler
            // 
            this.groupBoxScheduler.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxScheduler.Controls.Add(this.checkBoxSchedulerOnBatteries);
            this.groupBoxScheduler.Controls.Add(this.checkBoxSchedulerWhenAvailable);
            this.groupBoxScheduler.Controls.Add(this.checkBoxIgnoreWarnings);
            this.groupBoxScheduler.Controls.Add(this.checkBoxAutoclose);
            this.groupBoxScheduler.Controls.Add(this.label7);
            this.groupBoxScheduler.Controls.Add(this.dateTimePickerScheduleTime);
            this.groupBoxScheduler.Controls.Add(this.checkBoxScheduleSun);
            this.groupBoxScheduler.Controls.Add(this.checkBoxScheduleSat);
            this.groupBoxScheduler.Controls.Add(this.checkBoxScheduleFri);
            this.groupBoxScheduler.Controls.Add(this.checkBoxScheduleThurs);
            this.groupBoxScheduler.Controls.Add(this.checkBoxScheduleWeds);
            this.groupBoxScheduler.Controls.Add(this.checkBoxScheduleTues);
            this.groupBoxScheduler.Controls.Add(this.checkBoxScheduleMon);
            this.groupBoxScheduler.Enabled = false;
            this.groupBoxScheduler.Location = new System.Drawing.Point(4, 404);
            this.groupBoxScheduler.Name = "groupBoxScheduler";
            this.groupBoxScheduler.Size = new System.Drawing.Size(378, 101);
            this.groupBoxScheduler.TabIndex = 10;
            this.groupBoxScheduler.TabStop = false;
            this.groupBoxScheduler.Text = "Schedule";
            // 
            // checkBoxSchedulerOnBatteries
            // 
            this.checkBoxSchedulerOnBatteries.AutoSize = true;
            this.checkBoxSchedulerOnBatteries.Location = new System.Drawing.Point(257, 25);
            this.checkBoxSchedulerOnBatteries.Name = "checkBoxSchedulerOnBatteries";
            this.checkBoxSchedulerOnBatteries.Size = new System.Drawing.Size(106, 17);
            this.checkBoxSchedulerOnBatteries.TabIndex = 6;
            this.checkBoxSchedulerOnBatteries.Text = "Start on batteries";
            this.checkBoxSchedulerOnBatteries.UseVisualStyleBackColor = true;
            // 
            // checkBoxSchedulerWhenAvailable
            // 
            this.checkBoxSchedulerWhenAvailable.AutoSize = true;
            this.checkBoxSchedulerWhenAvailable.Location = new System.Drawing.Point(129, 24);
            this.checkBoxSchedulerWhenAvailable.Name = "checkBoxSchedulerWhenAvailable";
            this.checkBoxSchedulerWhenAvailable.Size = new System.Drawing.Size(122, 17);
            this.checkBoxSchedulerWhenAvailable.TabIndex = 5;
            this.checkBoxSchedulerWhenAvailable.Text = "Start when available";
            this.checkBoxSchedulerWhenAvailable.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreWarnings
            // 
            this.checkBoxIgnoreWarnings.AutoSize = true;
            this.checkBoxIgnoreWarnings.Location = new System.Drawing.Point(134, 74);
            this.checkBoxIgnoreWarnings.Name = "checkBoxIgnoreWarnings";
            this.checkBoxIgnoreWarnings.Size = new System.Drawing.Size(104, 17);
            this.checkBoxIgnoreWarnings.TabIndex = 4;
            this.checkBoxIgnoreWarnings.Text = "Ignore Warnings";
            this.checkBoxIgnoreWarnings.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoclose
            // 
            this.checkBoxAutoclose.AutoSize = true;
            this.checkBoxAutoclose.Checked = true;
            this.checkBoxAutoclose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoclose.Location = new System.Drawing.Point(8, 74);
            this.checkBoxAutoclose.Name = "checkBoxAutoclose";
            this.checkBoxAutoclose.Size = new System.Drawing.Size(120, 17);
            this.checkBoxAutoclose.TabIndex = 3;
            this.checkBoxAutoclose.Text = "Close when finished";
            this.checkBoxAutoclose.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Start at";
            // 
            // dateTimePickerScheduleTime
            // 
            this.dateTimePickerScheduleTime.CustomFormat = "h:mm tt";
            this.dateTimePickerScheduleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerScheduleTime.Location = new System.Drawing.Point(55, 24);
            this.dateTimePickerScheduleTime.Name = "dateTimePickerScheduleTime";
            this.dateTimePickerScheduleTime.ShowUpDown = true;
            this.dateTimePickerScheduleTime.Size = new System.Drawing.Size(68, 20);
            this.dateTimePickerScheduleTime.TabIndex = 1;
            this.dateTimePickerScheduleTime.Value = new System.DateTime(1990, 1, 1, 9, 0, 0, 0);
            // 
            // checkBoxScheduleSun
            // 
            this.checkBoxScheduleSun.AutoSize = true;
            this.checkBoxScheduleSun.Checked = true;
            this.checkBoxScheduleSun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScheduleSun.Location = new System.Drawing.Point(327, 51);
            this.checkBoxScheduleSun.Name = "checkBoxScheduleSun";
            this.checkBoxScheduleSun.Size = new System.Drawing.Size(45, 17);
            this.checkBoxScheduleSun.TabIndex = 0;
            this.checkBoxScheduleSun.Text = "Sun";
            this.checkBoxScheduleSun.UseVisualStyleBackColor = true;
            // 
            // checkBoxScheduleSat
            // 
            this.checkBoxScheduleSat.AutoSize = true;
            this.checkBoxScheduleSat.Checked = true;
            this.checkBoxScheduleSat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScheduleSat.Location = new System.Drawing.Point(279, 51);
            this.checkBoxScheduleSat.Name = "checkBoxScheduleSat";
            this.checkBoxScheduleSat.Size = new System.Drawing.Size(42, 17);
            this.checkBoxScheduleSat.TabIndex = 0;
            this.checkBoxScheduleSat.Text = "Sat";
            this.checkBoxScheduleSat.UseVisualStyleBackColor = true;
            // 
            // checkBoxScheduleFri
            // 
            this.checkBoxScheduleFri.AutoSize = true;
            this.checkBoxScheduleFri.Checked = true;
            this.checkBoxScheduleFri.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScheduleFri.Location = new System.Drawing.Point(236, 51);
            this.checkBoxScheduleFri.Name = "checkBoxScheduleFri";
            this.checkBoxScheduleFri.Size = new System.Drawing.Size(37, 17);
            this.checkBoxScheduleFri.TabIndex = 0;
            this.checkBoxScheduleFri.Text = "Fri";
            this.checkBoxScheduleFri.UseVisualStyleBackColor = true;
            // 
            // checkBoxScheduleThurs
            // 
            this.checkBoxScheduleThurs.AutoSize = true;
            this.checkBoxScheduleThurs.Checked = true;
            this.checkBoxScheduleThurs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScheduleThurs.Location = new System.Drawing.Point(177, 51);
            this.checkBoxScheduleThurs.Name = "checkBoxScheduleThurs";
            this.checkBoxScheduleThurs.Size = new System.Drawing.Size(53, 17);
            this.checkBoxScheduleThurs.TabIndex = 0;
            this.checkBoxScheduleThurs.Text = "Thurs";
            this.checkBoxScheduleThurs.UseVisualStyleBackColor = true;
            // 
            // checkBoxScheduleWeds
            // 
            this.checkBoxScheduleWeds.AutoSize = true;
            this.checkBoxScheduleWeds.Checked = true;
            this.checkBoxScheduleWeds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScheduleWeds.Location = new System.Drawing.Point(117, 51);
            this.checkBoxScheduleWeds.Name = "checkBoxScheduleWeds";
            this.checkBoxScheduleWeds.Size = new System.Drawing.Size(54, 17);
            this.checkBoxScheduleWeds.TabIndex = 0;
            this.checkBoxScheduleWeds.Text = "Weds";
            this.checkBoxScheduleWeds.UseVisualStyleBackColor = true;
            // 
            // checkBoxScheduleTues
            // 
            this.checkBoxScheduleTues.AutoSize = true;
            this.checkBoxScheduleTues.Checked = true;
            this.checkBoxScheduleTues.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScheduleTues.Location = new System.Drawing.Point(61, 51);
            this.checkBoxScheduleTues.Name = "checkBoxScheduleTues";
            this.checkBoxScheduleTues.Size = new System.Drawing.Size(50, 17);
            this.checkBoxScheduleTues.TabIndex = 0;
            this.checkBoxScheduleTues.Text = "Tues";
            this.checkBoxScheduleTues.UseVisualStyleBackColor = true;
            // 
            // checkBoxScheduleMon
            // 
            this.checkBoxScheduleMon.AutoSize = true;
            this.checkBoxScheduleMon.Checked = true;
            this.checkBoxScheduleMon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScheduleMon.Location = new System.Drawing.Point(8, 51);
            this.checkBoxScheduleMon.Name = "checkBoxScheduleMon";
            this.checkBoxScheduleMon.Size = new System.Drawing.Size(47, 17);
            this.checkBoxScheduleMon.TabIndex = 0;
            this.checkBoxScheduleMon.Text = "Mon";
            this.checkBoxScheduleMon.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseScheduler
            // 
            this.checkBoxUseScheduler.AutoSize = true;
            this.checkBoxUseScheduler.Location = new System.Drawing.Point(12, 381);
            this.checkBoxUseScheduler.Name = "checkBoxUseScheduler";
            this.checkBoxUseScheduler.Size = new System.Drawing.Size(96, 17);
            this.checkBoxUseScheduler.TabIndex = 11;
            this.checkBoxUseScheduler.Text = "Use Scheduler";
            this.checkBoxUseScheduler.UseVisualStyleBackColor = true;
            this.checkBoxUseScheduler.CheckedChanged += new System.EventHandler(this.checkBoxUseScheduler_CheckedChanged);
            // 
            // buttonSourceAdvanced
            // 
            this.buttonSourceAdvanced.Location = new System.Drawing.Point(265, 43);
            this.buttonSourceAdvanced.Name = "buttonSourceAdvanced";
            this.buttonSourceAdvanced.Size = new System.Drawing.Size(75, 23);
            this.buttonSourceAdvanced.TabIndex = 5;
            this.buttonSourceAdvanced.Text = "Advanced";
            this.buttonSourceAdvanced.UseVisualStyleBackColor = true;
            this.buttonSourceAdvanced.Click += new System.EventHandler(this.buttonSourceAdvanced_Click);
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 552);
            this.Controls.Add(this.checkBoxUseScheduler);
            this.Controls.Add(this.groupBoxScheduler);
            this.Controls.Add(this.groupBoxS3);
            this.Controls.Add(this.checkBoxS3);
            this.Controls.Add(this.checkBoxMirror);
            this.Controls.Add(this.groupBoxMirror);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(410, 590);
            this.Name = "PropertiesForm";
            this.Text = "Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxMirror.ResumeLayout(false);
            this.groupBoxMirror.PerformLayout();
            this.groupBoxS3.ResumeLayout(false);
            this.groupBoxS3.PerformLayout();
            this.groupBoxScheduler.ResumeLayout(false);
            this.groupBoxScheduler.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBoxScheduler;
        private System.Windows.Forms.DateTimePicker dateTimePickerScheduleTime;
        private System.Windows.Forms.CheckBox checkBoxScheduleSun;
        private System.Windows.Forms.CheckBox checkBoxScheduleSat;
        private System.Windows.Forms.CheckBox checkBoxScheduleFri;
        private System.Windows.Forms.CheckBox checkBoxScheduleThurs;
        private System.Windows.Forms.CheckBox checkBoxScheduleWeds;
        private System.Windows.Forms.CheckBox checkBoxScheduleTues;
        private System.Windows.Forms.CheckBox checkBoxScheduleMon;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxAutoclose;
        private System.Windows.Forms.CheckBox checkBoxIgnoreWarnings;
        private System.Windows.Forms.CheckBox checkBoxUseScheduler;
        private System.Windows.Forms.CheckBox checkBoxUseRRS;
        private System.Windows.Forms.LinkLabel linkLabelAWSCredientials;
        private System.Windows.Forms.CheckBox checkBoxS3Test;
        private System.Windows.Forms.TextBox textBoxIgnorePattern;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxSchedulerWhenAvailable;
        private System.Windows.Forms.CheckBox checkBoxSchedulerOnBatteries;
        private System.Windows.Forms.Button buttonSourceAdvanced;
    }
}