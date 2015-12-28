using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.Win32
{

    public class AppShortCut
    {
        //creatShortCut(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        static public void CreatShortCut(string path)
        {
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
            dynamic shell = Activator.CreateInstance(t);
            try
            {
                var shortCutPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/" +
                                   string.Format("{0}.lnk", "Maxwell X");
                if (File.Exists(shortCutPath)) return;
                var lnk = shell.CreateShortcut(shortCutPath);
                try
                {
                    lnk.TargetPath = path;
                    lnk.IconLocation = path + ", 0";
                    lnk.Save();
                }
                finally
                {
                    Marshal.FinalReleaseComObject(lnk);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }
    }
}
