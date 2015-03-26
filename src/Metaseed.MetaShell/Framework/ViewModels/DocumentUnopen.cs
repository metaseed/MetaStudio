using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using   System.Xml.Serialization;
using Metaseed.MetaShell.Properties;

namespace Metaseed.MetaShell.ViewModels
{
    [Serializable]
    public class DocumentUnopen:DocumentUnopenBase,IPackageContent
    {
        public string PackagePartID
        {
            get { throw new NotImplementedException(); }
        }
        public string PackagePartName
        {
            get { throw new NotImplementedException(); }
        }

        public string PackagePartType
        {
            get { throw new NotImplementedException(); }
        }

        public string PackgeContentType
        {
            get
            {
                return Resources.Unopen;
            }
        }

    }
}
