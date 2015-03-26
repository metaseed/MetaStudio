using System.Collections.Generic;
using System.IO;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using Metaseed.Windows.Interop;
namespace Metaseed.MetaShell.Services
{
    /// <summary>
    /// This logger will log to a text file called 'BugReport.txt'
    /// </summary>
    public class TextFileLogger : LoggerImplementation
    {
        public static Boolean IsTruncateTheFile = true;
        /// <summary>Logs the specified error.</summary>
        /// <param name="error">The error to log.</param>
        public override void LogError(string error)
        {
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//Path.GetDirectoryName(Application.ExecutablePath);
            string filename_S = filename + "\\screen_Error.jpg";
            filename+= "\\BugReport.txt";

            List<string> data = new List<string>();

            lock (this)
            {
                BitmapSource bs = CaptureScreenshot.Capture(new Rect(0, 0, SystemParameters.MaximizedPrimaryScreenWidth, SystemParameters.MaximizedPrimaryScreenHeight));
                FileStream stream1 = new FileStream(filename_S, FileMode.Create);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                //encoder.FlipHorizontal = true;
                //encoder.FlipVertical = false;
                encoder.QualityLevel = 80;//The quality level of the JPEG image. The value range is 1 (lowest quality) to 100 (highest quality) inclusive. 

                //encoder.Rotation = Rotation.Rotate90;
                encoder.Frames.Add(BitmapFrame.Create(bs));
                encoder.Save(stream1);
                stream1.Close();
                if (IsTruncateTheFile)
                {
                    if (File.Exists(filename))
                    {
                        using (StreamReader reader = new StreamReader(filename))
                        {
                            string line = null;
                            do
                            {
                                line = reader.ReadLine();
                                data.Add(line);
                            }
                            while (line != null);
                        }
                    }

                    // truncate the file if it's too long
                    int writeStart = 0;
                    if (data.Count > 1000)
                        writeStart = data.Count - 1000;

                    using (StreamWriter stream = new StreamWriter(filename, false))
                    {
                        for (int i = writeStart; i < data.Count; i++)
                        {
                            stream.WriteLine(data[i]);
                        }
                        stream.Write(error);
                    }
                }
                else
                {
                    using (StreamWriter stream = new StreamWriter(filename, false))
                    {

                        stream.Write(error);
                    }
                }

            }
            ExceptionHandler.textLoggerEvent.Set();
        }
    }
}
