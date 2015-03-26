using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Metaseed.Windows.Interop
{
    //http://www.csharpdeveloping.net/CodeSnippets.mvc/Detail/how_to_truncate_file_path
    //http://www.dreamincode.net/code/snippet3281.htm
   public class TruncateFilePath
    {
        /*
         string path = @"C:\WINDOWS\Microsoft.NET\Framework\v3.5\Microsoft .NET Framework 3.5 SP1\test.dll";
 Console.WriteLine(path);

 StringBuilder sb = new StringBuilder();
 int length = 40;

 // truncate path
 PathCompactPathEx(sb, path, length, 0);

 Console.WriteLine(sb);
         */

      [DllImport("shlwapi.dll")]
       //[DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
       static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, Int32 cchMax, Int32 dwFlags);

       public static string TruncatePath(string path, Int32 length)
      {
          StringBuilder sb = new StringBuilder();
          PathCompactPathEx(sb, path, length, 0);
          return sb.ToString();
      }
        ///// <summary>
        ///// method for truncating a path with elipses
        ///// </summary>
        ///// <param name="path">the path we wish to have truncated</param>
        ///// <param name="font">the font we're working with</param>
        ///// <returns></returns>
        //public static string TruncatePath(ref string path, Font font)
        //{
        //    //first trim the incoming string
        //    string truncatedPath = path.Trim();

        //    //next we need the height & width of the incoming string
        //    Bitmap bmp = new Bitmap(1, 1);

        //    //create a graphics from the blank Bitmap
        //    Graphics gfx = Graphics.FromImage(bmp);

        //    //now with our Graphics object we can measure the
        //    //height & width of the string
        //    SizeF sizeF = gfx.MeasureString(path, font);
        //    float height = sizeF.Height;
        //    float width = sizeF.Width;

        //    //now we will replace the center of the path with elipses
        //    TextRenderer.MeasureText(truncatedPath, font, new Size(Convert.ToInt32(width), Convert.ToInt32(height)),
        //                                             TextFormatFlags.ModifyString | TextFormatFlags.PathEllipsis);

        //    gfx.Dispose();
        //    bmp.Dispose();

        //    //return the modified path
        //    return truncatedPath;
        //}
    }
}
