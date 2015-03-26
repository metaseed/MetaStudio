//-----------------------------------------------------------------------------
// Description:
//   PackageWrite shows how to write a Package zip file
//   containing content, resource, and relationship parts.
//-----------------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Packaging;
using System.Collections.Generic;
using System.Linq;

namespace Metaseed.IO
{
    //http://207.46.16.248/fr-fr/library/ms748388(VS.85).aspx
    //http://msdn.microsoft.com/en-us/library/ms748388.aspx
    //http://msdn.microsoft.com/en-us/library/bb608597.aspx
    //http://msdn.microsoft.com/en-us/library/system.io.packaging.package.aspx
    //http://www.williamwishart.co.uk/post/2010/03/19/Zip-File-and-Folders-with-Net.aspx
    //http://madprops.org/blog/Zip-Your-Streams-with-System-IO-Packaging/
    //http://blogs.msdn.com/b/ericwhite/archive/2007/12/11/packages-and-parts.aspx
    public static class PackageManager
    {

        public static void Open(string packageName, string contentName, ref Stream toStream)
        {
            using (Package package = Package.Open(packageName, FileMode.Open, FileAccess.Read,FileShare.Read))
            {
                PackagePart dataPart = package.GetPart(PackUriHelper.CreatePartUri(new Uri(contentName, UriKind.Relative)));
                CopyStream(dataPart.GetStream(), toStream);
            }
        }

        public static void Open(string packagePath, ref Stream dataPartStream, ref Stream ramDefPartStream)
        {
            using (Package package = Package.Open(packagePath, FileMode.Open, FileAccess.Read))
            {
                PackagePart dataPart = package.GetPart(PackUriHelper.CreatePartUri( new Uri(@"Content\data.cf", UriKind.Relative)));
                PackagePart ramDefPart = package.GetPart(PackUriHelper.CreatePartUri(new Uri(@"Content\rd.xx", UriKind.Relative)));
                CopyStream(dataPart.GetStream(), dataPartStream);
                CopyStream(ramDefPart.GetStream(), ramDefPartStream);
            }
        }


        public static void Save(string packageName, string data, string xmlRamData)
        {
            Uri partUriData = PackUriHelper.CreatePartUri(new Uri(@"Content\data.cf", UriKind.Relative));
            Uri partUriRamDef = PackUriHelper.CreatePartUri(new Uri(@"Content\rd.xx", UriKind.Relative));
            using (Package package = Package.Open(packageName, FileMode.Create))
            {
                // Add the Document part to the Package
                PackagePart packagePartDocument = package.CreatePart(partUriRamDef,System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Maximum);
                using (System.IO.MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlRamData)))
                {
                    CopyStream(stream, packagePartDocument.GetStream());
                }
                PackagePart packagePartData = package.CreatePart(partUriData,System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);
                using (System.IO.MemoryStream stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(data)))
                {
                    CopyStream(stream, packagePartData.GetStream());
                }
            }
        }
        public static void Save(string packageName, Stream rawData, Stream xmlRamData)
        {
            Uri partUriData = PackUriHelper.CreatePartUri(
                                      new Uri(@"Content\data.cf", UriKind.Relative));
            Uri partUriRamDef = PackUriHelper.CreatePartUri(
                                      new Uri(@"Content\rd.xx", UriKind.Relative));
            using (Package package = Package.Open(packageName, FileMode.Create))
            {
                // Add the Document part to the Package
                PackagePart packagePartDocument = package.CreatePart(partUriRamDef,
                                   System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Maximum);
                //using (System.IO.MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlRamData)))
                //{
                CopyStream(xmlRamData, packagePartDocument.GetStream());
                //}
                PackagePart packagePartData = package.CreatePart(partUriData,
                                   System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);
                CopyStream(rawData, packagePartData.GetStream());
            }
        }
        /// <summary>
        /// usage
        /*           
         * Stream stream; Stream ramStream;
            using (System.IO.Packaging.Package pk = PackageManager.CreatPacage(CurrentPackagePath, out stream, out ramStream))
            {
                if (stream!=null)
                {
                    _ODBuilderService.Seriallize(stream);
                    stream.Dispose();
                }
                if (ramStream!=null)
                {
                    _ramDataBuilderService.Seriallize(ramStream);
                    ramStream.Dispose();
                }
            }
         */
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="streamRawData"></param>
        /// <param name="streamXML_RamData"></param>
        /// <returns></returns>
        public static Package CreatPacage(string packageName, out Stream streamRawData, out Stream streamXML_RamData)
        {
            Uri partUriData = PackUriHelper.CreatePartUri(
                                         new Uri(@"Content\data.cf", UriKind.Relative));
            Uri partUriRamDef = PackUriHelper.CreatePartUri(
                                      new Uri(@"Content\rd.xx", UriKind.Relative));
            //using (Package package = Package.Open(packageName, FileMode.Create))
            //{
            Package package = Package.Open(packageName, FileMode.Create);
            // Add the Document part to the Package
            PackagePart packagePartRamData = package.CreatePart(partUriRamDef,
                               System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Maximum);

            streamXML_RamData = packagePartRamData.GetStream();
            PackagePart packagePartData = package.CreatePart(partUriData,
                               System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);
            streamRawData = packagePartData.GetStream();
            return package;
            //}
        }

        //  --------------------------- CopyStream ---------------------------
        /// <summary>
        ///   Copies data from a source stream to a target stream.</summary>
        /// <param name="source">
        ///   The source stream to copy from.</param>
        /// <param name="target">
        ///   The destination stream to copy to.</param>
        private static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }// end:CopyStream()


    }// end:class 

}
