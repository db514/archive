using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DirectoryListing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, int> directories = new Dictionary<string, int>();
        List<string> folderList = new List<string>();
        string listParentName;

        private readonly List<string> predefinedPaths = new List<string>()
        {
            @"C:\img\bimbo",
            @"C:\img\bimbo archive",
            @"C:\img\cel",
            @"C:\img\cel archive",
            @"C:\img\cel bbw",
            @"C:\img\fit",
            @"C:\img\fit archive",
            @"C:\img\hairy",
            @"C:\img\hairy archive"
        };

        private readonly List<string> predefinedFileNames = new List<string>()
        {
            @"C:\listing\b.txt",
            @"C:\listing\ba.txt",
            @"C:\listing\c.txt",
            @"C:\listing\ca.txt",
            @"C:\listing\cb.txt",
            @"C:\listing\f.txt",
            @"C:\listing\fa.txt",
            @"C:\listing\h.txt",
            @"C:\listing\ha.txt"
        };

        private readonly IEnumerable<string> systemDirectories = new List<string>
        {
            @"c:\$Recycle.Bin",
            @"c:\Documents and Settings",
            @"c:\System Volume Information",
        };

        private string previousSourcePath = @"C:\img\";
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSetParentFolder_Click(object sender, RoutedEventArgs e)
        {

            Cursor = System.Windows.Input.Cursors.Wait;

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = @"C:\img\bimbo";
            
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                previousSourcePath = folderBrowserDialog.SelectedPath;
                GetDirectories(folderBrowserDialog.SelectedPath);
            }

            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnSaveListing_Click(object sender, RoutedEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Wait;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "txt";
            
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if(!saveFileDialog.FileName.EndsWith(".txt"))
                {
                    saveFileDialog.FileName += ".txt";
                }
                WriteFile(saveFileDialog.FileName);
            }

            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void BtnCreateListings_Click(object sender, RoutedEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Wait;
            string root = @"C:\listing";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            if (predefinedPaths.Count() == predefinedFileNames.Count())
            {
                for (int index = 0; index < predefinedPaths.Count(); index ++)
                {
                    if (File.Exists(predefinedFileNames[index]))
                    {
                        File.Delete(predefinedFileNames[index]);
                    }

                    GetDirectories(predefinedPaths[index]);
                    WriteFile(predefinedFileNames[index]);
                }
            }

            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void WriteFile(string filePath)
        {
            File.WriteAllLines(filePath, directories.Select(x => x.Key + " [" + x.Value + "]").ToArray());
        }

        private void GetDirectories(string path)
        {
            folderList.Clear();
            directories.Clear();
            folderList = Directory.EnumerateDirectories(path).ToList<string>();
            foreach (string systemDirectory in systemDirectories)
            {
                if (folderList.Contains(systemDirectory))
                {
                    folderList.Remove(systemDirectory);
                }
            }

            string selectedFolder = path;
            int startOfParentName = selectedFolder.LastIndexOf('\\');
            listParentName = path.Substring(startOfParentName + 1, selectedFolder.Length - (startOfParentName + 1));

            foreach (string currentFolder in folderList)
            {
                string name = currentFolder.Substring(currentFolder.LastIndexOf('\\') + 1, (currentFolder.Length - 1 - currentFolder.LastIndexOf('\\')));
                List<string> fileList = Directory.EnumerateFiles(currentFolder).ToList();
                directories.Add(name, fileList.Count);
            }
        }

    }
}
