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
            this.textBoxDest = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSourceBrowser = new System.Windows.Forms.Button();
            this.buttonDestBrowser = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
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
            this.textBoxSource.Location = new System.Drawing.Point(48, 52);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(158, 20);
            this.textBoxSource.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source";
            // 
            // textBoxDest
            // 
            this.textBoxDest.Location = new System.Drawing.Point(48, 78);
            this.textBoxDest.Name = "textBoxDest";
            this.textBoxDest.Size = new System.Drawing.Size(158, 20);
            this.textBoxDest.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Dest";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(48, 24);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(158, 20);
            this.textBoxName.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Name";
            // 
            // buttonSourceBrowser
            // 
            this.buttonSourceBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSourceBrowser.Location = new System.Drawing.Point(212, 50);
            this.buttonSourceBrowser.Name = "buttonSourceBrowser";
            this.buttonSourceBrowser.Size = new System.Drawing.Size(75, 23);
            this.buttonSourceBrowser.TabIndex = 2;
            this.buttonSourceBrowser.Text = "Browse...";
            this.buttonSourceBrowser.UseVisualStyleBackColor = true;
            this.buttonSourceBrowser.Click += new System.EventHandler(this.buttonSourceBrowser_Click);
            // 
            // buttonDestBrowser
            // 
            this.buttonDestBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDestBrowser.Location = new System.Drawing.Point(212, 76);
            this.buttonDestBrowser.Name = "buttonDestBrowser";
            this.buttonDestBrowser.Size = new System.Drawing.Size(75, 23);
            this.buttonDestBrowser.TabIndex = 2;
            this.buttonDestBrowser.Text = "Browse...";
            this.buttonDestBrowser.UseVisualStyleBackColor = true;
            this.buttonDestBrowser.Click += new System.EventHandler(this.buttonDestBrowser_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(13, 125);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // Properties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 160);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonDestBrowser);
            this.Controls.Add(this.buttonSourceBrowser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.textBoxDest);
            this.Controls.Add(this.textBoxSource);
            this.Name = "Properties";
            this.Text = "Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog sourceBrowser;
        private System.Windows.Forms.FolderBrowserDialog destBrowser;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSourceBrowser;
        private System.Windows.Forms.Button buttonDestBrowser;
        private System.Windows.Forms.Button buttonSave;
    }
}