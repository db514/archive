using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfRandomSelector
{
    class RandomFileSelector :IItemRepository
    {
        /*Using auto property This is old way
            private List<string> _currentPath = new List<string>();
            public List<string> CurrentPath
            {
                get { return _currentPath; }
                set { _currentPath = value; }
            }
        */
        public string CurrentPath { get; set; } = string.Empty;

        public List<string> FileList { get; set; } = new List<string>();

        public List<string> AlreadyViewedList { get; set; } = new List<string>();

        public bool AutomaticallyAddItemToViewedList;

        public void GetItemList(string path)
        {
            FileList = Directory.EnumerateFiles(path).ToList();
            CurrentPath = path;
        }
        
        public string ReturnItem()
        {
            string text = string.Empty;
            Random randomSelect = new Random(Guid.NewGuid().GetHashCode());
            if (randomSelect == null)
            {
                randomSelect = new Random(Guid.NewGuid().GetHashCode());
            }
            if (FileList.Count > 0)
            {
                if (AlreadyViewedList.Count == 0)
                {
                    int index = randomSelect.Next(0, FileList.Count);
                    text = FileList[index];

                    if (AutomaticallyAddItemToViewedList && string.IsNullOrWhiteSpace(text) == false)
                    {
                        AlreadyViewedList.Add(text);
                    }
                }
                else
                {
                    //Loop a maximium of the number of files in the file list
                    while (AlreadyViewedList.Count < FileList.Count())
                    {
                        int folderListIndex = randomSelect.Next(0, FileList.Count);
                        text = FileList[folderListIndex];
                        //If the selected directory is no in the already viewed list then add it and break out of the loop otherwsies try again
                        if (!AlreadyViewedList.Contains(text))
                        {
                            AlreadyViewedList.Add(text);
                            break;
                        }
                    }
                }
            }

            return text;
        }

        public void AddItemToAlreadyViewedList(string item)
        {
            AlreadyViewedList.Add(item);
        }
    }
}
