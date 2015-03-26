using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Metaseed.IO
{
    //http://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory

    static public class DirectoryHelper
    {
      static public string GetRelativePath(this string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
