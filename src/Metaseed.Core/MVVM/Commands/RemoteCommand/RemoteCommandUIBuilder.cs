using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandUIBuilder : IRemoteCommandUIBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UriString">
        /// format is like: pack://application:,,,/assemblyName;component/Resources/Images/NewDoc.png 
        /// or icon file relative path from MetaStudio: i.e. ../no.png
        /// </param>
        /// <returns></returns>
        protected BitmapImage GetBitmap(string uriString)
        {
            string looseFilePath = null;
            if (string.IsNullOrEmpty(uriString))
            {
                return null;
            }
            if (File.Exists(uriString))
            {
                looseFilePath = uriString;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + uriString))
            {
                looseFilePath = AppDomain.CurrentDomain.BaseDirectory + uriString;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/"+uriString))
            {
                looseFilePath = AppDomain.CurrentDomain.BaseDirectory +"/"+ uriString;
            }
            try
            {
                Uri uri;
                if (looseFilePath != null)
                {
                    if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
                    {
                        return new BitmapImage(uri);
                    }
                }
                if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
                {
                    return new BitmapImage(uri);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
        virtual public void GenerateUI(CompositeRemoteCommand command)
        {

        }

        virtual public void RemoveUI(string commandID)
        {

        }
    }
}
