namespace RandomFolderSelector
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnParentFolder = new System.Windows.Forms.Button();
            this.lblParentFolder = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.lblSelectedFolder = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.cmbFolderList = new System.Windows.Forms.ComboBox();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.cbSelectFiles = new System.Windows.Forms.CheckBox();
            this.btnReloadFileList = new System.Windows.Forms.Button();
            this.gbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnParentFolder
            // 
            this.btnParentFolder.Location = new System.Drawing.Point(13, 52);
            this.btnParentFolder.Name = "btnParentFolder";
            this.btnParentFolder.Size = new System.Drawing.Size(90, 23);
            this.btnParentFolder.TabIndex = 0;
            this.btnParentFolder.Text = "Parent Folder";
            this.btnParentFolder.UseVisualStyleBackColor = true;
            this.btnParentFolder.Click += new System.EventHandler(this.btnParentFolder_Click);
            // 
            // lblParentFolder
            // 
            this.lblParentFolder.AutoSize = true;
            this.lblParentFolder.Location = new System.Drawing.Point(123, 20);
            this.lblParentFolder.Name = "lblParentFolder";
            this.lblParentFolder.Size = new System.Drawing.Size(109, 13);
            this.lblParentFolder.TabIndex = 1;
            this.lblParentFolder.Text = "Parent Folder Not Set";
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(13, 100);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(89, 23);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "&Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // lblSelectedFolder
            // 
            this.lblSelectedFolder.AutoSize = true;
            this.lblSelectedFolder.Location = new System.Drawing.Point(123, 105);
            this.lblSelectedFolder.Name = "lblSelectedFolder";
            this.lblSelectedFolder.Size = new System.Drawing.Size(120, 13);
            this.lblSelectedFolder.TabIndex = 3;
            this.lblSelectedFolder.Text = "Selected Folder Not Set";
            // 
            // cmbFolderList
            // 
            this.cmbFolderList.FormattingEnabled = true;
            this.cmbFolderList.Location = new System.Drawing.Point(126, 54);
            this.cmbFolderList.Name = "cmbFolderList";
            this.cmbFolderList.Size = new System.Drawing.Size(275, 21);
            this.cmbFolderList.TabIndex = 1;
            this.cmbFolderList.Visible = false;
            this.cmbFolderList.SelectedIndexChanged += new System.EventHandler(this.cmbFolderList_SelectedIndexChanged);
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.btnReloadFileList);
            this.gbOptions.Controls.Add(this.cbSelectFiles);
            this.gbOptions.Location = new System.Drawing.Point(593, 23);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(155, 138);
            this.gbOptions.TabIndex = 4;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // cbSelectFiles
            // 
            this.cbSelectFiles.AutoSize = true;
            this.cbSelectFiles.Location = new System.Drawing.Point(18, 31);
            this.cbSelectFiles.Name = "cbSelectFiles";
            this.cbSelectFiles.Size = new System.Drawing.Size(80, 17);
            this.cbSelectFiles.TabIndex = 0;
            this.cbSelectFiles.Text = "Select Files";
            this.cbSelectFiles.UseVisualStyleBackColor = true;
            // 
            // btnReloadFileList
            // 
            this.btnReloadFileList.Location = new System.Drawing.Point(33, 91);
            this.btnReloadFileList.Name = "btnReloadFileList";
            this.btnReloadFileList.Size = new System.Drawing.Size(90, 23);
            this.btnReloadFileList.TabIndex = 5;
            this.btnReloadFileList.Text = "Reload File List";
            this.btnReloadFileList.UseVisualStyleBackColor = true;
            this.btnReloadFileList.Click += new System.EventHandler(this.btnReloadFileList_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 279);
            this.Controls.Add(this.gbOptions);
            this.Controls.Add(this.cmbFolderList);
            this.Controls.Add(this.lblSelectedFolder);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lblParentFolder);
            this.Controls.Add(this.btnParentFolder);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Random Folder Selector";
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnParentFolder;
        private System.Windows.Forms.Label lblParentFolder;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label lblSelectedFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ComboBox cmbFolderList;
        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.CheckBox cbSelectFiles;
        private System.Windows.Forms.Button btnReloadFileList;
    }
}

