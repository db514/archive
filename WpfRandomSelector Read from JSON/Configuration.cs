using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
