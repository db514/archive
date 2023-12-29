using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

namespace OpenWindowsExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            List<string> folderList = new List<string>();
            string folderListString = string.Empty;
            string path = Directory.GetCurrentDirectory();
            string fileNameAndPath = path + "\\config.xml";
            if (File.Exists(fileNameAndPath))
            {
                XDocument xdoc = XDocument.Load(fileNameAndPath);
                folderListString = xdoc.Descendants("StartingDirectoryList").First().Value;
                folderList = folderListString.Split(',').ToList<string>();

                foreach (string folder in folderList)
                {
                    var process = Process.Start("explorer.exe", folder);
                    System.Threading.Thread.Sleep(200);

                }

            }
        }
    }
}
