using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;
using SHDocVw;
using System;
//using Microsoft.Win32;
//using System.Runtime.InteropServices;

namespace WpfRandomSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RandomFolderSelector folderSelector = new RandomFolderSelector();
        RandomFileSelector fileSelector = new RandomFileSelector();

        List<string> itemsToBeDeleted = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            if (folderSelector.startingFolderList != null && folderSelector.startingFolderList.Count > 0)
            {
                foreach (string currentItem in folderSelector.startingFolderList)
                {
                    CmbFolderList.Items.Add(currentItem);
                }

                if(!string.IsNullOrWhiteSpace(folderSelector.CurrentPath))
                {
                    CmbFolderList.SelectedIndex = CmbFolderList.Items.IndexOf(folderSelector.CurrentPath);
                }
                else
                {
                    CmbFolderList.SelectedIndex = 0;
                }

                fileSelector.CurrentPath = folderSelector.CurrentPath;
            }

            ChkUseMinimumCount.IsChecked = folderSelector.UseMinimumFileCount;
            TxtMinimumFileCount.Text = folderSelector.MinimumFileCount.ToString();
            ChkAutomaticallyAddItemToList.IsChecked = folderSelector.AutomaticallyAddItemToViewedList;
        }

        //This is called on double click only
        private void BtnSetParentFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            //If there is a current parent folder then set the selected path to the parent folder
            if (LblParentFolder.Content.ToString().Contains(":"))
            {
                folderBrowserDialog.SelectedPath = LblParentFolder.Content.ToString();
            }
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LblParentFolder.Content = folderBrowserDialog.SelectedPath;
                folderSelector.GetItemList(folderBrowserDialog.SelectedPath);
                MainWindow mainWindow = this;
                mainWindow.fileSelector.GetItemList(folderBrowserDialog.SelectedPath);
                mainWindow.CmbFolderList.Items.Add(folderBrowserDialog.SelectedPath);
                mainWindow.folderSelector.ClearAlreadyViewedItems();
                CmbFolderList.SelectedIndex = CmbFolderList.Items.Count - 1;
            }
            
        }

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Wait;
 
            #region Folder Item Select
            //Folder
            if (RbFolder.IsChecked == true)
            {
                FrameworkElement sendingButton = sender as System.Windows.Controls.Button;

                if (sendingButton.Name == "BtnSelectItem")
                {                    
                    try
                    {
                        ClosePreviouslyOpenedExplorerWindows(folderSelector.TempImagePath, LblSelectedItem.Content.ToString());

                        string text = folderSelector.ReturnItem();
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            Cursor = System.Windows.Input.Cursors.Arrow;
                            System.Windows.MessageBox.Show("There are no folders to view. Either the folder list is empty or all folders have been viewed", "No Unviewed Items Exist", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            LblSelectedItem.Content = folderSelector.CurrentSelectedItem;
                            Cursor = System.Windows.Input.Cursors.Arrow;
                            Process.Start("explorer.exe", text);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Display error to user
                        System.Windows.MessageBox.Show(ex.Message, "Error Selecting Item", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (LblSelectedItem.Content.ToString() == "No Item Selected")
                {
                    Cursor = System.Windows.Input.Cursors.Arrow;
                    System.Windows.MessageBox.Show("Please select an item ", "No Item Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            #endregion

            #region File Item Select
            //File
            else
            {
                string text = fileSelector.ReturnItem();
                if (string.IsNullOrWhiteSpace(text))
                {
                    Cursor = System.Windows.Input.Cursors.Arrow;
                    System.Windows.MessageBox.Show("There are no files to view. Either the file list is empty or all files have been viewed", "No Unviewed Files Exist", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    LblSelectedItem.Content = text;
                    Cursor = System.Windows.Input.Cursors.Arrow;
                    Process.Start(text);
                }
            }
            #endregion 
      

            Cursor = System.Windows.Input.Cursors.Arrow;
            return;

        }

        private void cmbFolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LblParentFolder.Content = CmbFolderList.SelectedItem.ToString();

            //Set current paths in file and folder selectors
            folderSelector.CurrentPath = CmbFolderList.SelectedItem.ToString();
            fileSelector.CurrentPath = CmbFolderList.SelectedItem.ToString();

            //Set Get Item Lists in file and folder selectors
            folderSelector.GetItemList(CmbFolderList.SelectedItem.ToString());
            fileSelector.GetItemList(CmbFolderList.SelectedItem.ToString());           

            //CLear already vied lists in file and folder selectors
            folderSelector.ClearAlreadyViewedItems();
            fileSelector.ClearAlreadyViewedItems();

        }

        private void RefreshList_OnClick(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = LblParentFolder.Content.ToString();
            folderSelector.GetItemList(selectedDirectory);
            fileSelector.GetItemList(selectedDirectory);
            folderSelector.ClearAlreadyViewedItems();
            fileSelector.ClearAlreadyViewedItems();
        }

        private void ReLoadCurrentItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (folderSelector.UseTempImageDirectory)
            {
                Process.Start("explorer.exe", folderSelector.TempImagePath);
            }
            else
            {
                Process.Start("explorer.exe", LblSelectedItem.Content.ToString());
            }
        }

        private void ChkAutomaticallyAddItemToList_OnClick(object sender, RoutedEventArgs e)
        {
            folderSelector.AutomaticallyAddItemToViewedList = ChkAutomaticallyAddItemToList.IsChecked??false;
            fileSelector.AutomaticallyAddItemToViewedList = ChkAutomaticallyAddItemToList.IsChecked ?? false;

        }

        private void ChkUseMinimumCount_OnClick(object sender, RoutedEventArgs e)
        {
            folderSelector.UseMinimumFileCount = ChkUseMinimumCount.IsChecked ?? false;
        }

        private void ViewAlreadyViewedList_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (RbFile.IsChecked ?? false == true)
            {
                //open the AlreadyViewListDisplay passing in a reference to this form
                AlreadyViewListDisplay disp = new AlreadyViewListDisplay(fileSelector.AlreadyViewedList, this);
                disp.Show();

            }
            else
            {
                //open the AlreadyViewListDisplay passing in a reference to this form
                AlreadyViewListDisplay disp = new AlreadyViewListDisplay(folderSelector.AlreadyViewedList, this);
                disp.Show();
            }
            
        }

        public void ItemsToBeDeletedChanged(List<string> items)
        {
            itemsToBeDeleted = items;

            //Folder
            if (RbFolder.IsChecked == true)
            {
                folderSelector.RemoveItemFromAlreadyViewedList(itemsToBeDeleted);
            }
            //File
            else
            {
                fileSelector.RemoveItemFromAlreadyViewedList(itemsToBeDeleted);
            }

        }

        private void TxtMinimumFileCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int minimumFileCount;
            bool success = int.TryParse(TxtMinimumFileCount.Text, out minimumFileCount);
            if(success)
            {
                folderSelector.MinimumFileCount = minimumFileCount;
            }
            else
            {
                System.Windows.MessageBox.Show("Please Enter a Valid Number", "Invalid Number", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // delete files in the temp directory            
            if (Directory.Exists(folderSelector.TempImagePath))
            {
                DirectoryInfo di = new DirectoryInfo(folderSelector.TempImagePath);
                foreach (FileInfo file in di.GetFiles())
                {
                    FileAttributes attributes = File.GetAttributes(file.FullName);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        file.IsReadOnly = false;
                    }
                    file.Delete();
                } 
            }
        }

        private void RbFolderFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton clickedRadioButton = (sender as System.Windows.Controls.RadioButton);
            string name = clickedRadioButton.Name;
            switch (clickedRadioButton.Name)
            {
                case "RbFile":
                    if (CmbFolderList.SelectedItem.ToString().ToLower() != LblSelectedItem.Content.ToString().ToLower())
                    {
                        fileSelector.GetItemList(CmbFolderList.SelectedItem.ToString());
                        LblSelectedItem.Content = fileSelector.CurrentPath;
                    }
                break;

                case "RbFolder":
                    if (CmbFolderList.SelectedItem.ToString().ToLower() != LblSelectedItem.Content.ToString().ToLower())
                    {
                        folderSelector.GetItemList(CmbFolderList.SelectedItem.ToString());
                        if(folderSelector.FolderList.Count == 0)
                        {
                            string selectedDirectory = CmbFolderList.SelectedItem.ToString();
                            folderSelector.GetItemList(selectedDirectory);
                            fileSelector.GetItemList(selectedDirectory);
                        }
                        LblSelectedItem.Content = folderSelector.CurrentSelectedItem?? CmbFolderList.SelectedItem.ToString();
                    }
                    break;

                default:
                    break;
            }
        }

        private void RerandomizeCurrentImageItem_Click(object sender, RoutedEventArgs e)
        {

            Cursor = System.Windows.Input.Cursors.Arrow;

            ClosePreviouslyOpenedExplorerWindows(folderSelector.TempImagePath, string.Empty);

            folderSelector.RandomlyCopyImageFiles(folderSelector.CurrentSelectedItem);
            
            Process.Start("explorer.exe", folderSelector.TempImagePath);

            Cursor = System.Windows.Input.Cursors.Arrow;
            return;
        }

        private void ManuallyLoadDirectory_Click(object sender, RoutedEventArgs e)
        {
            BtnSelectItem.IsEnabled = false;

            Cursor = System.Windows.Input.Cursors.Wait;

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            //If there is a current parent folder then set the selected path to the parent folder
            if (LblParentFolder.Content.ToString().Contains(":"))
            {
                folderBrowserDialog.SelectedPath = LblParentFolder.Content.ToString();

                
            }

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ClosePreviouslyOpenedExplorerWindows(folderSelector.TempImagePath, LblSelectedItem.Content.ToString());

                LblSelectedItem.Content = folderBrowserDialog.SelectedPath;
                folderSelector.CurrentSelectedItem = folderBrowserDialog.SelectedPath;

                Cursor = System.Windows.Input.Cursors.Wait;

                LblParentFolder.Content = Path.GetDirectoryName(folderBrowserDialog.SelectedPath);

                folderSelector.RandomlyCopyImageFiles(folderBrowserDialog.SelectedPath);

                Process.Start("explorer.exe", folderSelector.TempImagePath);

                Cursor = System.Windows.Input.Cursors.Arrow;
            }

            BtnSelectItem.IsEnabled = true;
            Cursor = System.Windows.Input.Cursors.Arrow;
            return;
        }

        private void LoadItemListFromXML_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (RbFolder.IsChecked == true)
                {
                    folderSelector.GetItemsListFromXMLFile(openFileDialog.FileName);
                    LblParentFolder.Content = folderSelector.CurrentPath;
                }
                else
                {
                    fileSelector.GetItemsListFromXMLFile(openFileDialog.FileName);
                    LblParentFolder.Content = fileSelector.CurrentPath;
                }
             
            }
            
        }

        private void ClosePreviouslyOpenedExplorerWindows(string path, string alternatePath = "")
        {
            //for project add a reference to Microsoft Internet Controls from the COM tab
            ShellWindows _shellWindows = new SHDocVw.ShellWindows();
            string processType;
            //format path to all lower for use in comparison.  This is necessary since the path or alternate path might be read in from a config
            //file and the case may vary from that of selected path
            string alternatePathFormatted = alternatePath.ToLower();
            string pathFormatted = path.ToLower();

            //Cycle through all open windows
            foreach (InternetExplorer ie in _shellWindows)
            {
                //this parses the name of the process
                processType = Path.GetFileNameWithoutExtension(ie.FullName).ToLower();


                //this could also be used for IE windows with processType of "iexplore"
                if (processType.Equals("explorer"))
                {
                    // Remove the file:///  from the URL location and replace escape character with actual character
                    string UnescapedLocationURL = Uri.UnescapeDataString(ie.LocationURL.Replace(@"file:///", ""));

                    //Replace / with \\ to match with path or alternate path strings
                    string explorerPath = UnescapedLocationURL.Replace('/', '\\');

                    explorerPath = explorerPath.ToLower();


                    if (explorerPath == pathFormatted
                        || explorerPath == alternatePathFormatted
                        || explorerPath.Contains(alternatePathFormatted))

                    {
                        //if there is an explorer process open at the path or the alternate path then close it
                        ie.Quit();
                    }
                }
            }
        }

        private void AddCurrentItemToAlreadyViewedList_Click(object sender, RoutedEventArgs e)
        {            
            //Folder
            if (RbFolder.IsChecked == true)
            {
                folderSelector.AddItemToAlreadyViewedList(folderSelector.CurrentSelectedItem);

            }
            //File
            else
            {
                fileSelector.AddItemToAlreadyViewedList(fileSelector.CurrentPath);
            }
            
        }

        private void OpenSelectedDirectory_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(LblSelectedItem.Content.ToString()))
            {
                Process.Start("explorer.exe", LblSelectedItem.Content.ToString());
            }
        }
    }
}
