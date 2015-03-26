using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using System.IO;
using Catel.Modules;

namespace Metaseed.Modules
{
    //https://compositewpf.codeplex.com/discussions/350389 
    /// <summary>
    /// this kind of module catalog,could not correctly load the contract dll(the dll always in modue dll that use it as the port out side contrct) of the modules.
    /// DirectoryModuleCatalog when find the 
    /// </summary>
    class AllSubDirectoriesModuleCatalog : SafeDirectoryModuleCatalog//SafeDirectoryModuleCatalog too slow
    {
        IEnumerable<string> languageFolders;
        IEnumerable<string> LanguageFolders
        {
            get
            {
                if (languageFolders == null)
                {
                    languageFolders = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures).Select(info => info.Name);
                }
                return languageFolders;
            }

        }
        protected override void InnerLoad()
        {
            base.InnerLoad();
            var folders = new DirectoryInfo(ModulePath).GetDirectories("*", SearchOption.AllDirectories);
            //string[] languageFolders = new string[] { "zh-Hans", "zh", "zh-Chs", "zh-Hant" };//http://www.csharp-examples.net/culture-names/
            var folders_ExceptLanguageFolders = from folder in folders where !LanguageFolders.Contains(folder.Name, StringComparer.InvariantCultureIgnoreCase) select folder.FullName;
            // var bb = folders_ExceptLanguageFolders.ToArray();
            foreach (string folder in folders_ExceptLanguageFolders)
            {
                ModulePath = folder;
                base.InnerLoad();
            }
        }
    }
}
