using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace DirectoryListing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, int> directories = new Dictionary<string, int>();

        //These are the directories of interest with the drive letter set to C
        private readonly List<string> predefinedPaths = new List<string>()
        {
            @"C:\img\bimbo archive",
            @"C:\img\cel archive",
            @"C:\img\fit archive",
            @"C:\img\hairy archive",
            @"C:\img\bimbo",
            @"C:\img\cel",
            @"C:\img\cel bbw",
            @"C:\img\fit",
            @"C:\img\hairy"
        };

        //These are the diurectires to use.  This list will be the predefiend if they exist on C, otherwise the 
        //Drive letter will be changed to the drive letter where the predefined directory resides.
        private List<string> pathsToUse = new List<string>();

        //These will be the directory names with a .txt extension
        private List<string> filesToWrite = new List<string>();

        private readonly IEnumerable<string> systemDirectories = new List<string>
        {
            @"c:\$Recycle.Bin",
            @"c:\Documents and Settings",
            @"c:\System Volume Information",
        };

        private string previousSourcePath = @"C:\img\";

        private string driveLetterToUse = "C";
       
        public MainWindow()
        {
            InitializeComponent();
            PopulatePathsToUseAndFilesToWrite();
        }

        private void PopulatePathsToUseAndFilesToWrite()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                if(drive.IsReady)
                {
                    string path = drive.RootDirectory.ToString() + "img";

                    string driveLetter = drive.Name.Replace(":\\", "").Trim();

                    foreach (string predefinedPath in predefinedPaths)
                    {
                        string pathToTest = predefinedPath.Replace("C", driveLetter);
                        if (Directory.Exists(pathToTest))
                        {
                            pathsToUse.Add(pathToTest);
                            filesToWrite.Add(pathToTest.Substring(pathToTest.LastIndexOf('\\') + 1) + ".txt");
                        }
                    }

                    //If the pathsToUse has been populated then set the driveLetterToUse to this drive letter 
                    if (pathsToUse.Count > 0)
                    {
                        driveLetterToUse = driveLetter;
                    }
                }
            }
        }

        private void BtnSetParentFolder_Click(object sender, RoutedEventArgs e)
        {

            Cursor = System.Windows.Input.Cursors.Wait;

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            //folderBrowserDialog.SelectedPath = @"C:\img\bimbo";
            
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
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.DefaultExt = "txt";
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = "txt"
            };

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
            string outputDirectory = driveLetterToUse +  @":\listing";
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            if (pathsToUse.Count() == filesToWrite.Count())
            {
                for (int index = 0; index < pathsToUse.Count(); index++)
                {
                    if (File.Exists(filesToWrite[index]))
                    {
                        File.Delete(filesToWrite[index]);
                    }

                    GetDirectories(pathsToUse[index]);
                    WriteFile(outputDirectory + "\\" + filesToWrite[index]);
                }

                System.Windows.MessageBox.Show("Directory List completed", "Listing Complete", MessageBoxButton.OK, MessageBoxImage.None);
            }

            else
            {
                System.Windows.MessageBox.Show("File names to list count does not match file paths count", "configuration error",MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void WriteFile(string filePath)
        {
            File.WriteAllLines(filePath, directories.Select(x => x.Key + " [" + x.Value + "]").ToArray());
        }

        private void GetDirectories(string path)
        {
            List<string> folderList = new List<string>();

            directories.Clear();
            folderList = Directory.EnumerateDirectories(path).ToList<string>();
            foreach (string systemDirectory in systemDirectories)
            {
                if (folderList.Contains(systemDirectory))
                {
                    folderList.Remove(systemDirectory);
                }
            }

            foreach (string currentFolder in folderList)
            {
                if (!currentFolder.Contains("@"))
                {
                    string name = currentFolder.Substring(currentFolder.LastIndexOf('\\') + 1, (currentFolder.Length - 1 - currentFolder.LastIndexOf('\\')));
                    List<string> fileList = Directory.EnumerateFiles(currentFolder).ToList();
                    directories.Add(name, fileList.Count); 
                }
            }
        }

    }
}
