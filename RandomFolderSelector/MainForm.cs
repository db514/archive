using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RandomFolderSelector
{
    public partial class MainForm : Form
    {
        RandomSelect randomSelect;
        string CurrentFolder;
        string parentFolderPath;
        string currentFile;
        List<string> selectedList = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            randomSelect = new RandomSelect();
            folderBrowserDialog1.Description = "Select the parent directory";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.SelectedPath = @"C:\";
            folderBrowserDialog1.ShowNewFolderButton = false;
            if(!string.IsNullOrEmpty(randomSelect.currentPath))
            {
                folderBrowserDialog1.SelectedPath = randomSelect.currentPath;
                lblParentFolder.Text = randomSelect.currentPath;
            }
            if (randomSelect.startingFolderList.Count > 0)
            {
                foreach (string currentItem in randomSelect.startingFolderList)
                {
                    cmbFolderList.Items.Add(currentItem);
                }
                
                cmbFolderList.Visible = true;
                cmbFolderList.SelectedIndex = 0;
            }
            else
            {
                cmbFolderList.Visible = false;
            }
            
            parentFolderPath = cmbFolderList.SelectedItem.ToString();
            lblParentFolder.Text = parentFolderPath;

        }

        private void btnParentFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                parentFolderPath = folderBrowserDialog1.SelectedPath;
                randomSelect.SetFolderList(parentFolderPath);
                lblParentFolder.Text = parentFolderPath;
                cmbFolderList.Items.Add(parentFolderPath);
                CurrentFolder = parentFolderPath;
                randomSelect.SetFileList(parentFolderPath);
                
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            int fileCount = 0;

            if (cbSelectFiles.Checked == false)
            {
                while (fileCount < randomSelect.minimumFileCount)
                {
                  
                    CurrentFolder = randomSelect.GetRandomFolderName();                                     
                    DirectoryInfo dir = new System.IO.DirectoryInfo(CurrentFolder);
                    fileCount = dir.GetFiles().Length;
                }
                lblSelectedFolder.Text = CurrentFolder;
                Process.Start("explorer.exe", CurrentFolder).WaitForExit();
                
            }
            else
            {
                currentFile = randomSelect.GetRandomFileName();
                lblSelectedFolder.Text = currentFile;
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = currentFile; 
                myProcess.Start();
                

            }
            
        }

        private void cmbFolderList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDirectory = cmbFolderList.SelectedItem.ToString();
            randomSelect.SetFolderList(selectedDirectory);
            randomSelect.SetFileList(selectedDirectory);
            

        }
        
        private void btnReloadFileList_Click(object sender, EventArgs e)
        {            
            randomSelect.SetFileList(parentFolderPath);
        }
        
    }
}
