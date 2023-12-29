using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
//installed nuget package Windows7APICodePack-Shell
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WpfRandomSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RandomFolderSelector folderSelector = new RandomFolderSelector();

        RandomFileSelector fileSelector = new RandomFileSelector();
        
        List<string> alreadyViewed = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            if (folderSelector.startingFolderList != null && folderSelector.startingFolderList.Count > 0)
            {
                foreach (string currentItem in folderSelector.startingFolderList)
                {
                    this.CmbFolderList.Items.Add(currentItem);
                }

                CmbFolderList.SelectedIndex = 0;
            }

            ChkUseMinimumCount.IsChecked = folderSelector.UseMinimumFileCount;
            TxtMinimumFileCount.Text = folderSelector.MinimumFileCount.ToString();
            ChkAutomaticallyAddItemToList.IsChecked = folderSelector.AutomaticallyAddItemToViewedList;
        }

        private void BtnSetParentFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            if (LblParentFolder.Content.ToString().Contains(":"))
            {
                folderDialog.InitialDirectory = LblParentFolder.Content.ToString();
            }
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                LblParentFolder.Content = folderDialog.FileName;
                folderSelector.GetItemList(folderDialog.FileName);
                MainWindow mainWindow = this;
                mainWindow.fileSelector.GetItemList(folderDialog.FileName);
                mainWindow.CmbFolderList.Items.Add(folderDialog.FileName);
                mainWindow.folderSelector.ClearAlreadyViewedItems();
            }
            CmbFolderList.SelectedIndex = CmbFolderList.Items.Count - 1;
        }

        private void BtnSelectItem_Click(object sender, RoutedEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Wait;

            #region Folder Item Select
            //Folder
            if (RbFolder.IsChecked == true)
            {
                FrameworkElement sendingButton = sender as Button;

                if (sendingButton.Name == "BtnSelectItem")
                {
                    string text = folderSelector.ReturnItem();
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Cursor = System.Windows.Input.Cursors.Arrow;
                        MessageBox.Show("There are on items to view. Either the list is empty or all items have been viewed", "No Unviewed Items Exist", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        LblSelectedItem.Content = folderSelector.CurrentSelectedItem;
                        Cursor = System.Windows.Input.Cursors.Arrow;
                        Process.Start("explorer.exe", text);
                    }
                }
                else if (LblSelectedItem.Content.ToString() == "No Item Selected")
                {
                    Cursor = System.Windows.Input.Cursors.Arrow;
                    MessageBox.Show("Please select an item ", "No Item Selected", MessageBoxButton.OK, MessageBoxImage.Information);                    
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
                    MessageBox.Show("There are on items to view. Either the list is empty or all items have been viewed", "No Unviewed Items Exist", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (RbFolder.IsChecked == true)
            {
                folderSelector.GetItemList(CmbFolderList.SelectedItem.ToString());
            }
            else
            {
                fileSelector.GetItemList(CmbFolderList.SelectedItem.ToString());
            }
            LblParentFolder.Content = CmbFolderList.SelectedItem.ToString();
            folderSelector.ClearAlreadyViewedItems();
        }

        private void RefreshList_OnClick(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = LblParentFolder.Content.ToString();
            folderSelector.GetItemList(selectedDirectory);
            fileSelector.GetItemList(selectedDirectory);
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
                AlreadyViewListDisplay disp = new AlreadyViewListDisplay(fileSelector.AlreadyViewedList);
                disp.Show();

            }
            else
            {
                AlreadyViewListDisplay disp = new AlreadyViewListDisplay(folderSelector.AlreadyViewedList);
                disp.Show();
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
                MessageBox.Show("Please Enter a Valid Number", "Invalid Number", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    file.Delete();
                } 
            }
        }

        private void RbFolderFile_Click(object sender, RoutedEventArgs e)
        {
            RadioButton clickedRadioButton = (sender as RadioButton);
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
                        LblSelectedItem.Content = folderSelector.CurrentSelectedItem;
                    }
                    break;

                default:
                    break;
            }
        }

        private void RerandomizeCurrentImageItem_Click(object sender, RoutedEventArgs e)
        {

            Cursor = System.Windows.Input.Cursors.Arrow;

            folderSelector.RandomlyCopyImageFiles(folderSelector.CurrentSelectedItem);
            
            Process.Start("explorer.exe", folderSelector.TempImagePath);

            Cursor = System.Windows.Input.Cursors.Arrow;
            return;
        }

        private void ManuallyLoadDirectory_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            if (LblParentFolder.Content.ToString().Contains(":"))
            {
                folderDialog.InitialDirectory = LblParentFolder.Content.ToString();
            }
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                LblParentFolder.Content = folderDialog.FileName;
                Cursor = System.Windows.Input.Cursors.Wait;
                folderSelector.RandomlyCopyImageFiles(folderDialog.FileName);
                Process.Start("explorer.exe", folderSelector.TempImagePath);
                Cursor = System.Windows.Input.Cursors.Arrow;


            }

            return;
        }
    }
}
