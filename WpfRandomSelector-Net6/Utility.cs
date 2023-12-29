using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfRandomSelector
{
    public class Utility
    {
        public static bool IsPathValid(string path)
        {
            try
            {
                _ = Path.GetFullPath(path);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
