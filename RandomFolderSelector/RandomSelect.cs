using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace RandomFolderSelector
{
    class RandomSelect
    {
        public List<string> folderList = new List<string>();
        List<string> exclusionList;
        public List<string> startingFolderList = new List<string>();
        public List<string> fileList = new List<string>();

        public string currentPath = string.Empty;
        public int minimumFileCount;
        
        public RandomSelect()
        {
            string path = Directory.GetCurrentDirectory();
            string fileNameAndPath = path + "\\config.xml";
            if(File.Exists(fileNameAndPath))
            {
                XDocument xdoc = XDocument.Load(fileNameAndPath);
                currentPath = xdoc.Descendants("startingDirectory").First().Value;

                string exclusionDirectoryList = xdoc.Descendants("excludeDirectoryList").First().Value;
                exclusionDirectoryList = exclusionDirectoryList.Replace("\n", string.Empty);
                exclusionList = exclusionDirectoryList.Split(',').ToList<string>();

                var folderSelectionElement = xdoc.Descendants("startingDirectoryList").FirstOrDefault();
                if (folderSelectionElement != null)
                {
                    string folderSelectionList = xdoc.Descendants("startingDirectoryList").First().Value;
                    folderSelectionList = folderSelectionList.Replace("\n", string.Empty);
                    startingFolderList = folderSelectionList.Split(',').ToList<string>();
                }

                string minimumFileCountString = xdoc.Descendants("minimumFileCount").First().Value;
    
                bool success = int.TryParse(minimumFileCountString, out minimumFileCount);
                
                SetFolderList(currentPath);
            }
        }

        public void SetFolderList(string path)
        {
            folderList.Clear();
            folderList = Directory.GetDirectories(path).ToList();
            foreach (string folder in exclusionList)
            {
                string folderAndPath = currentPath + "\\" + folder;
                if (folderList.Contains(folderAndPath))
                {
                    folderList.Remove(folderAndPath);
                }
            }
        }

        public void SetFileList(string path)
        {
            fileList = Directory.GetFiles(path).ToList();
            
        }

        public string GetRandomFolderName()
        {
            string result= string.Empty;
            Random randomSelect = new Random();
            int index = randomSelect.Next(0, folderList.Count);
            
            result = folderList[index];

            return result;
        }

        public string GetRandomFileName()
        {
            string result = string.Empty;
            if (fileList.Count > 0)
            {
                Random randomSelect = new Random();
                int index = randomSelect.Next(0, fileList.Count);
                result = fileList[index];
            }
            return result;
        }


    }
}
