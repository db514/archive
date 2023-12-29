using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace WpfRandomSelector
{
    class RandomFileSelector :IItemRepository
    {
        private string currentPath = string.Empty;        
        public string CurrentPath
        {
            get { return currentPath; }
            set { currentPath = value; }
        }

        private List<string> fileList = new List<string>();
        
        public List<string> FileList
        {
            get { return fileList; }
            set { fileList = value; }
        }

        private List<string> alreadyViewedList = new List<string>();    
        
        public List<string> AlreadyViewedList
        {
            get { return alreadyViewedList; }
            set { alreadyViewedList = value; }
        }

        private bool _automaticallyAddItemToViewedList;
        public bool AutomaticallyAddItemToViewedList
        {
            get { return _automaticallyAddItemToViewedList; }
            set { _automaticallyAddItemToViewedList = value; }
        }

        public void GetItemList(string path)
        {

            if (Utility.IsPathValid(path))
            {
                fileList = Directory.EnumerateFiles(path).ToList();
                currentPath = path; 
            }
        }

        public void GetItemsListFromXMLFile(string fileNameAndPath)
        {
            if (Utility.IsPathValid(fileNameAndPath))
            {
                //string filename = Path.GetFileName(fileNameAndPath);
                string path = Path.GetDirectoryName(fileNameAndPath);
                path += "\\";

                XmlDocument doc = new XmlDocument();
                doc.Load(fileNameAndPath);

                fileList.Clear();

                //List<string> list = new List<string>();
                //XmlNode idNodes = doc.SelectSingleNode("items/name");


                XmlNodeList idNodes = doc.SelectNodes("items/name");
                foreach (XmlNode node in idNodes)
                {
                    fileList.Add(path + node.InnerText);
                    //list.Add(path+node.InnerText);
                }

                currentPath = path;
            }

        }


        public string ReturnItem()
        {
            string text = string.Empty;
            Random randomSelect = new Random(Guid.NewGuid().GetHashCode());
            if (randomSelect == null)
            {
                randomSelect = new Random(Guid.NewGuid().GetHashCode());
            }

            foreach (string item in fileList)
            {
                int index = randomSelect.Next(0, fileList.Count);
                text = fileList[index];
                bool test = alreadyViewedList.Contains(text);
                if (!alreadyViewedList.Contains(text))
                {
                    break;
                }
            }
            if (AutomaticallyAddItemToViewedList)
            {
                if (alreadyViewedList.Contains(text))
                {
                    text = string.Empty;
                    return text;
                }
                else
                {
                    alreadyViewedList.Add(text);
                }
            }

            return text;
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
    }
}
