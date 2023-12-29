using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WpfRandomSelector
{
    class RandomFolderSelector : IItemRepository
    {
        
        public List<string> FolderList { get; set; } = new List<string>();

        List<string> exclusionList;

        public List<string> startingFolderList;

        public bool UseMinimumFileCount { get; set; }

        public List<string> AlreadyViewedList { get; set; } = new List<string>();

        private Random randomSelect;

        public bool AutomaticallyAddItemToViewedList { get; set; }

        public int MinimumFileCount { get; set; }

        public string CurrentPath { get; set; } = string.Empty;

        public string CurrentSelectedItem { get; set; }

        public string TempImagePath { get; set; }

        string[] imageTypes = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };

        public bool UseTempImageDirectory { get; set; } = false;

        public RandomFolderSelector()
        {
            string path = Directory.GetCurrentDirectory();
            string fileNameAndPath = path + "\\config.xml";
            if (File.Exists(fileNameAndPath))
            {
                XDocument xdoc = XDocument.Load(fileNameAndPath);
                CurrentPath = xdoc.Descendants("startingDirectory").First().Value;
                string exclusionDirectoryList = xdoc.Descendants("excludeDirectoryList").First().Value;
                exclusionDirectoryList = exclusionDirectoryList.Replace("\n", string.Empty);
                exclusionList = exclusionDirectoryList.Split(',').ToList();

                XElement folderSelectionElement = xdoc.Descendants("startingDirectoryList").FirstOrDefault();
                if (folderSelectionElement != null)
                {
                    string folderSelectionList = xdoc.Descendants("startingDirectoryList").First().Value;
                    folderSelectionList = folderSelectionList.Replace("\n", string.Empty);
                    startingFolderList = folderSelectionList.Split(',').ToList();
                }

                XElement useMinimumFileCountElement = xdoc.Descendants("useMinimumFileCount").FirstOrDefault();
                if(useMinimumFileCountElement != null)
                {
                    string useMinimumFileCountString = xdoc.Descendants("useMinimumFileCount").First().Value;
                    if(useMinimumFileCountString.ToLower() == "true" || useMinimumFileCountString.ToLower() == "t" || useMinimumFileCountString.ToLower() == "1" )
                    {
                        AutomaticallyAddItemToViewedList = true;
                    }
                    else
                    {
                        AutomaticallyAddItemToViewedList = false;
                    }
                                       
                }

                XElement minimumFileCountElement = xdoc.Descendants("minimumFileCount").FirstOrDefault();
                if(minimumFileCountElement != null && UseMinimumFileCount == true)
                {
                    string minimumFileCountString = xdoc.Descendants("minimumFileCount").First().Value;
                    int tempMinimumFileCount;
                    bool success = int.TryParse(minimumFileCountString, out tempMinimumFileCount);
                    if(success)
                    {
                        MinimumFileCount = tempMinimumFileCount;
                    }
                }

                XElement automaticallyAddItemToViewedListElement = xdoc.Descendants("automaticallyAddItemToViewedList").FirstOrDefault();
                if(automaticallyAddItemToViewedListElement != null)
                {
                    string automaticallyAddItemToViewedListString = xdoc.Descendants("automaticallyAddItemToViewedList").First().Value;
                    if(automaticallyAddItemToViewedListString.ToLower() == "true" || automaticallyAddItemToViewedListString.ToLower() == "t" || automaticallyAddItemToViewedListString.ToLower() == "1")
                    {

                    }
                }

                XElement tempImagePath = xdoc.Descendants("tempImagePath").FirstOrDefault();
                if (tempImagePath != null)
                {
                    TempImagePath = xdoc.Descendants("tempImagePath").First().Value;
                    
                }

                GetItemList(CurrentPath);
            }

        }    

        public void GetItemList(string path)
        {
            FolderList.Clear();
            FolderList = Directory.EnumerateDirectories(path).ToList();
            foreach (string current in exclusionList)
            {
                if (FolderList.Count > 0)
                {
                    string str = FolderList[0].Substring(0, FolderList[0].LastIndexOf("\\") + 1);
                    if (FolderList.Contains(str + current))
                    {
                        FolderList.Remove(str + current);
                    }
                }
            }

        }        

        public string ReturnItem()
        {
            if (randomSelect == null)
            {
                randomSelect = new Random(Guid.NewGuid().GetHashCode());
            }
            string itemPath = string.Empty;
            int fileCount = -1;
           
            // If the alreadyViewedList count is zero then either we are not ignoring already view objects, or there are none which have been viewed
            if (AlreadyViewedList.Count == 0)
            {
                while (fileCount < MinimumFileCount)
                {
                    int index = randomSelect.Next(0, FolderList.Count);
                    itemPath = FolderList[index];
                    fileCount = Directory.GetFiles(itemPath, "*", SearchOption.TopDirectoryOnly).Length;
                }

                if (AutomaticallyAddItemToViewedList && string.IsNullOrWhiteSpace(itemPath) == false)
                {
                    AlreadyViewedList.Add(itemPath);
                }
            }
            else
            {
                //Loop a maximium of the number of files in the file list
                while (AlreadyViewedList.Count < FolderList.Count())
                {
                    int folderListIndex = randomSelect.Next(0, FolderList.Count);
                    itemPath = FolderList[folderListIndex];
                    //If the selected directory is not in the already viewed list then add it and break out of the loop otherwise try again
                    if (!AlreadyViewedList.Contains(itemPath))
                    {
                        int fCount = Directory.GetFiles(itemPath, "*", SearchOption.TopDirectoryOnly).Length;
                        if (UseMinimumFileCount && fCount > MinimumFileCount)
                        {
                            AlreadyViewedList.Add(itemPath);
                            break;
                        }
                        else
                        {
                            AlreadyViewedList.Add(itemPath);
                        itemPath = string.Empty;

                        }

                    }
                }

            }

            CurrentSelectedItem = itemPath;
            //check if there are image files in the itemPath directory
            string[] imageFiles = GetFilesFrom(itemPath + "\\", imageTypes, false);
            if(imageFiles.Length > 0)
            {
                UseTempImageDirectory = RandomlyCopyImageFiles(itemPath);                
            }

            if (UseTempImageDirectory == true)
            {
                return TempImagePath;
            }
            else
            {
                return itemPath;
            }

        }

        public void AddItemToAlreadyViewedList(string item)
        {
            AlreadyViewedList.Add(item);
        }

        public void ClearAlreadyViewedItems()
        {
            AlreadyViewedList.Clear();
        }

        public bool RandomlyCopyImageFiles(string sourcePath)
        {
            //This will create the directory specified in _tempImagePath unless it already exists
            if (!string.IsNullOrWhiteSpace(TempImagePath))
            {
                Directory.CreateDirectory(TempImagePath);
                DirectoryInfo di = new DirectoryInfo(TempImagePath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                //return false so caller can use original path
                return false;
            }

            //now that the directory is created randomize the images
            string[] sourceFileList = Directory.GetFiles(sourcePath, "*", SearchOption.TopDirectoryOnly);
            string sourceFolderName = sourcePath.Substring(sourcePath.LastIndexOf('\\') + 1, sourcePath.Length - sourcePath.LastIndexOf('\\') -1);
            if (randomSelect == null)
            {
                randomSelect = new Random(Guid.NewGuid().GetHashCode());
            }
            string[] randomFileList = sourceFileList.OrderBy(x => randomSelect.Next()).ToArray();
            for(int index = 0; index < sourceFileList.Length; index++)
            {                
                string fileExtension = sourceFileList[index].Substring(sourceFileList[index].LastIndexOf('.'), sourceFileList[index].Length - sourceFileList[index].LastIndexOf('.'));
                string destinationFileName = TempImagePath + "\\" + sourceFolderName + index.ToString() + fileExtension;
                File.Copy(randomFileList[index], destinationFileName);
            }

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

    }
}
