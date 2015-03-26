/* System.Web.dll
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Metaseed.Services
{
  /// <summary>Logs errors via a HTTP POST to a webpage</summary>
  public class WebsiteLogger : LoggerImplementation
  {
    /// <summary>Logs the specified error.</summary>
    /// <param name="error">The error to log.</param>
    public override void LogError(string error)
    {
      if (string.IsNullOrEmpty(url))
        throw new ArgumentException("Url has not been set");
      if (string.IsNullOrEmpty(queryString))
        throw new ArgumentException("QueryString has not been set");

      Uri uri = new Uri(url);
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
      httpWebRequest.Method = "POST";
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";

      Encoding encoding = Encoding.Default;

      string parameters = string.Format(queryString, HttpUtility.UrlEncode(error));

      // get length of request (may well be a better way to do this)
      MemoryStream memStream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter(memStream, encoding);
      streamWriter.Write(parameters);
      streamWriter.Flush();
      httpWebRequest.ContentLength = memStream.Length;
      streamWriter.Close();

      Stream stream = httpWebRequest.GetRequestStream();
      streamWriter = new StreamWriter(stream, encoding);
      streamWriter.Write(parameters);
      streamWriter.Close();

      using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
      using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
      {
        streamReader.ReadToEnd();
      }
    }

    private string url;
    /// <summary>
    /// Gets or sets the URL that will be used when posting an error to a website.
    /// </summary>
    public string Url
    {
      get
      {
        return url;
      }
      set
      {
        url = value;
      }
    }

    private string queryString;
    /// <summary>
    /// Gets or sets the format of the query string that will be used when posting an error to a website. 
    /// e.g error={0}
    /// </summary>
    public string QueryString
    {
      get
      {
        return queryString;
      }
      set
      {
        queryString = value;
      }
    }
  }
}
*/