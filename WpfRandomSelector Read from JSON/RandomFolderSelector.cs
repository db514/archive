using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using IWshRuntimeLibrary;
using Newtonsoft.Json;

namespace WpfRandomSelector
{
    class Configuration
    {
        public string StartingDirectory { get; set; }
        public List<string> StartingDirectoryList { get; set; }
        public List<string> ExcludeDirectoryList { get; set; }
        public bool UseMinimumFileCount { get; set; }
        public int MinimumFileCount { get; set; }
        public string TempImagePath { get; set; }
        public bool AutomaticallyAddItemToViewedList { get; set; }

    }


    class RandomFolderSelector : IItemRepository
    {

        #region Properties and variables
        List<string> folderList = new List<string>();
        public List<string> FolderList
        {
            get { return folderList; }
            set { folderList = value; }
        }

        private bool _loadListOnSelect;
        public bool LoadListOnSelect
        {
            get
            {
                return _loadListOnSelect;
            }
            set
            {
                _loadListOnSelect = value;
            }
        }
        private List<string> alreadyViewedList = new List<string>();
        public List<string> AlreadyViewedList
        {
            get { return alreadyViewedList; }
            set { alreadyViewedList = value; }
        }

        private Random randomSelect;

        private bool _automaticallyAddItemToViewedList;
        public bool AutomaticallyAddItemToViewedList
        {
            get { return _automaticallyAddItemToViewedList; }
            set { _automaticallyAddItemToViewedList = value; }
        }

        
        private int _minimumFileCount;
        public int MinimumFileCount
        {
            get { return _minimumFileCount; }
            set {_minimumFileCount = value; }
        }

        private string _currentPath = string.Empty;
        public string CurrentPath
        {
            get
            {
                return _currentPath;
            }
            set
            {
                _currentPath = value;
            }
        }

        private string _currentSelectedItem;
        public string CurrentSelectedItem
        {
            get { return _currentSelectedItem; }
            set { _currentSelectedItem = value;}
        }
        readonly string[] imageTypes = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };

        private bool _useTempImageDirectory = false;
        public bool UseTempImageDirectory
        {
            get { return _useTempImageDirectory; }
            set { _useTempImageDirectory = value; }
        }

        public Configuration configuration;

        #endregion

        public RandomFolderSelector()
        {
            string path = Directory.GetCurrentDirectory();
            string configNameAndPath = path + "\\config.json";
            if (System.IO.File.Exists(configNameAndPath))
            {
                string jsonConfigurationString = System.IO.File.ReadAllText(configNameAndPath);
                configuration = JsonConvert.DeserializeObject<Configuration>(jsonConfigurationString);
            }


        }    

        public void GetItemList(string path)
        {
            if (Utility.IsPathValid(path))
            {
                folderList.Clear();
                folderList = Directory.EnumerateDirectories(path).ToList();
                foreach (string current in configuration.ExcludeDirectoryList)
                {
                    if (folderList.Count > 0)
                    {
                        string str = folderList[0].Substring(0, folderList[0].LastIndexOf("\\") + 1);
                        if (folderList.Contains(str + current))
                        {
                            folderList.Remove(str + current);
                        }
                    }
                }
                _loadListOnSelect = true;
            }

        }

        public void GetItemsListFromXMLFile(string fileNameAndPath)
        {
            if (Utility.IsPathValid(fileNameAndPath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileNameAndPath);

                folderList.Clear();

                string path = string.Empty;
                XmlNode pathNode = doc.SelectSingleNode("root/path");
                if(pathNode != null)
                {
                    path = pathNode.InnerText;
                }
                XmlNodeList idNodes = doc.SelectNodes("root/items/name");
                foreach (XmlNode node in idNodes)
                {                    
                    folderList.Add(path + node.InnerText);
                }


                _currentPath = path;
                _loadListOnSelect = false;

            }

        }

        public string ReturnItem()
        {
            if (folderList.Count < 1)
            {
                return string.Empty;
            }

            if (randomSelect == null)
            {
                randomSelect = new Random(Guid.NewGuid().GetHashCode());
            }
            string itemPath = string.Empty;
            int fileCount;
            int minFileCount = configuration.UseMinimumFileCount ? _minimumFileCount : int.MinValue;
         
            foreach(string item in folderList)
            {
                int index = randomSelect.Next(0, folderList.Count);
                itemPath = folderList[index];
                bool test = alreadyViewedList.Contains(itemPath);
                fileCount = Directory.GetFiles(itemPath, "*", SearchOption.TopDirectoryOnly).Length;
                if(!alreadyViewedList.Contains(itemPath) && fileCount >= minFileCount && itemPath != _currentSelectedItem)
                {
                    break;
                }
            }
            if(AutomaticallyAddItemToViewedList)
            {
                if (alreadyViewedList.Contains(itemPath))
                {
                    itemPath = string.Empty;
                    return itemPath;
                }
                else
                {
                    alreadyViewedList.Add(itemPath);
                }
            }

            _currentSelectedItem = itemPath;
            _useTempImageDirectory = false;
            //check if there are image files in the itemPath directory
            string[] imageFiles = GetFilesFrom(itemPath + "\\", imageTypes, false);
            if(imageFiles.Length > 0)
            {
                _useTempImageDirectory = RandomlyCopyImageFiles(itemPath);                
            }

            if (_useTempImageDirectory == true)
            {
                return configuration.TempImagePath;
            }
            else
            {
                return itemPath;
            }

        }

        public void AddItemToAlreadyViewedList(string item)
        {
            alreadyViewedList.Add(item);
        }

        public void ClearAlreadyViewedItems()
        {
            alreadyViewedList.Clear();
        }

        public void RemoveItemFromAlreadyViewedList(List<string> itemsToBeRemoved)
        {
            foreach (string item in itemsToBeRemoved)
            {
                alreadyViewedList.Remove(item);
            }
        }

        public bool RandomlyCopyImageFiles(string sourcePath)
        {
            //This will create the directory specified in _tempImagePath unless it already exists
            if (!string.IsNullOrWhiteSpace(configuration.TempImagePath))
            {
                Directory.CreateDirectory(configuration.TempImagePath);
                DirectoryInfo di = new DirectoryInfo(configuration.TempImagePath);
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in di.GetFiles())
                {
                    FileAttributes attributes = System.IO.File.GetAttributes(file.FullName);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        file.IsReadOnly = false;
                    }
                    file.Delete();
                }

                //get the source image directory name to use are a shortcut name
                //string sourceFileDirectoryName = sourcePath.Substring(sourcePath.LastIndexOf('\\') + 1, sourcePath.Length - sourcePath.LastIndexOf('\\') - 1);
               // CreateShortCut(sourceFileDirectoryName, sourcePath, _tempImagePath);
            }
            else
            {
                //return false so caller can use original path
                return false;
            }

            //now that the directory is created randomize the images
            string[] originalFileList = Directory.GetFiles(sourcePath, "*", SearchOption.TopDirectoryOnly);
            string originalFolderName = sourcePath.Substring(sourcePath.LastIndexOf('\\') + 1, sourcePath.Length - sourcePath.LastIndexOf('\\') -1);
            if (randomSelect == null)
            {
                randomSelect = new Random(Guid.NewGuid().GetHashCode());
            }
            string[] randomFileList = originalFileList.OrderBy(x => randomSelect.Next()).ToArray();

            StreamWriter SaveFile = new StreamWriter(configuration.TempImagePath + @"\Log.txt");

            for (int index = 0; index < randomFileList.Length; index++)
            {                
                string sourceFileExtension = randomFileList[index].Substring(randomFileList[index].LastIndexOf('.'), randomFileList[index].Length - randomFileList[index].LastIndexOf('.'));
                string sourceFileName = randomFileList[index].Substring(randomFileList[index].LastIndexOf('\\') + 1, randomFileList[index].Length - randomFileList[index].LastIndexOf('\\') - 1);
                string destinationFileName = configuration.TempImagePath + "\\" + originalFolderName + " " + index.ToString() + sourceFileExtension;

                System.IO.File.Copy(randomFileList[index], destinationFileName);
                //Add source file name and destination file name to text file for logging purposes
                SaveFile.WriteLine(originalFolderName + " " + index.ToString() + sourceFileExtension + @" -> " + sourceFileName);
            }

            SaveFile.Close();
            return true;
        }

        //This function will get the files from searchFolder that match the files in extensions in the filters array with the option to search 
        //subdirectories if isRecursive equals true
        public static string[] GetFilesFrom(string searchFolder, string[] filters, bool isRecursive)
        {
            List<string> filesFound = new List<string>();
            SearchOption searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (string filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, string.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        //this function will create a shorcut in the source directory to the destination directory with the name shortcutName
        //It uses COM object Windows Script Host Object Model which will need to be added as a reference
        private static void CreateShortCut(string shortcutName, string sourceDirectory, string destinationDirectory)
        {

            shortcutName = "Open " + shortcutName + " Directory";

            string shortcutLocation = System.IO.Path.Combine(destinationDirectory, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Shortcut description";   // The description of the shortcut
            //shortcut.IconLocation = @"c:\myicon.ico";           // The icon of the shortcut
            shortcut.TargetPath = sourceDirectory;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }

    }
}
