using System;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using System.Windows;
using Metaseed.Windows.Interop;
namespace Metaseed.MetaShell.Services
{
    //http://www.cnblogs.com/netatomy/archive/2008/01/06/1028188.html
    //http://www.cnblogs.com/gossip/archive/2010/03/23/1692639.html
    //http://help.163.com/09/1223/14/5R7P3QI100753VB8.html
    //http://blogs.msdn.com/b/mariya/archive/2006/06/15/633007.aspx
    //http://answers.unity3d.com/questions/18952/system-net-mail-smtpclient-send
    //http://www.cnitblog.com/hj627/articles/61893.html
    /// <summary>Sends error logs as emails</summary>
    public class EmailLogger : LoggerImplementation
    {
        /// <summary>Logs the specified error.</summary>
        /// <param name="error">The error to log.</param>
        public override void LogError(string error)
        {
            // check all properties have been set
            if (string.IsNullOrEmpty(EmailFrom))
                throw new ArgumentException("EmailFrom has not been set");
            if (string.IsNullOrEmpty(EmailTo))
                throw new ArgumentException("EmailTo has not been set");
            if (string.IsNullOrEmpty(EmailServer))
                throw new ArgumentException("EmailServer has not been set");

            #region 163
            //EmailFrom = "songjz179@163.com";
            //EmailTo = "songjz179@163.com";
            //EmailServer = "smtp.163.com";
            //string userName="songjz179";
            //string psw="songjz!&(";
            //int portNum=25;
            //bool enableSSL=false;
            #endregion 
            #region sina
            EmailFrom = "feellikerunning@sina.com";
            EmailTo = "songjz179@163.com";
            EmailServer = "smtp.sina.com";
            string userName = "feellikerunning";
            string psw = "zhonglin";
            int portNum = 25;
            bool enableSSL = false;
            #endregion
            #region 126
            //EmailFrom = "echosong2008@126.com";
            //EmailTo = "songjz179@163.com";
            //EmailServer = "smtp.126.com";
            //string userName = "echosong2008";
            //string psw = "zhonglin";
            //int portNum = 25;
            //bool enableSSL = false;
            #endregion
            #region gmail
            //EmailFrom = "goody2e@gmail.com";
            //EmailTo = "songjz179@163.com";
            //EmailServer = "smtp.gmail.com";
            //string userName = "goody2e";
            //string psw = "gd&#*()%";
            //int portNum = 465;
            //bool enableSSL = true;
            #endregion

            MailMessage message = new MailMessage(EmailFrom, EmailTo, "CANOpenStudio Unhandled Exception Report", error);
            // message.To.Add()
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\screen_Error.jpg";
            bool again = false;
            if (System.IO.File.Exists(filename))
            {
                try
                {
                    message.Attachments.Add(new Attachment(filename));
                }
                catch (Exception)
                {
                    again = true;
                }
            }
            if (again)
            {

                filename = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\Desktop_screen_Error.jpg";
                BitmapSource bs = CaptureScreenshot.Capture(new Rect(0, 0, SystemParameters.MaximizedPrimaryScreenWidth, SystemParameters.MaximizedPrimaryScreenHeight));
                FileStream stream1 = new FileStream(filename, FileMode.Create);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = 80;
                encoder.Frames.Add(BitmapFrame.Create(bs));
                //encoder.Save(stream1);
                message.Attachments.Add(new Attachment(stream1, "Desktop_screen_Error.jpg"));
            }



            message.IsBodyHtml = false;
            message.Priority = MailPriority.High;
            
            //http://help.163.com/10/1111/15/6L7HMASV00753VB8.html
            //smtp.163.com 安全类型：SSL 端口号：465 / 994 ;若安全类型选择“无”，则需将端口号修改为 25。
            SmtpClient client = new SmtpClient(EmailServer);// 如 smtp.163.com, smtp.gmail.com 
            client.Port = portNum;// // Gmail 使用 465 和 587 端口 
            client.EnableSsl = enableSSL; // 如果使用GMail，则需要设置为true 
            // Add credentials if the SMTP server requires them.
            client.Credentials = new NetworkCredential(userName, psw);// CredentialCache.DefaultNetworkCredentials; //new NetworkCredential("用户名", "密码"); //



            client.SendCompleted += new SendCompletedEventHandler(SendMailCompleted);
            try
            {
                client.SendAsync(message, "错误报告");

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Unknown Error：\n" + "Please Send The Files:'BugReport.log,screen_Error.jpg' On The Desktop To songjz163@163.com, To Help To Improve The Function Of The Application，\nThank You Very Much!\n\n\n\n" + e.Message.ToString());
            }
            
        }
 
        void SendMailCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //if (ExceptionLogger.lwm!=null)
            //{
            //    ExceptionLogger.lwm.Close();
            //}
            string m = e.UserState.ToString();
            if (e.Cancelled) // 邮件被取消 
            {
                System.Windows.MessageBox.Show(m + "email 被取消。");
            }
            if (e.Error != null)
            {
                System.Windows.MessageBox.Show("错误：\n" + "请将桌面上的'BugReport.log和screen_Error.jpg'发送给songjz163@163.com,以帮助完善软件的功能，\n谢谢您的支持!\n\n\n\n" + e.Error.ToString());
            }
            else
            {
                //System.Windows.MessageBox.Show("We Are Sorry To Show This Message To You,\nWe Will Solve The Problem As Soon As Possible, Thanks! \n");
            }
            ExceptionHandler.emailLoggerEvent.Set();
        }
        private string emailServer = "smtp.163.com";
        /// <summary>
        /// Specifies the email server that the exception information email will be sent via
        /// </summary>
        public string EmailServer
        {
            get
            {
                return emailServer;
            }
            set
            {
                emailServer = value;
            }
        }

        private string emailFrom = "songjz179@163.com";
        /// <summary>
        /// Specifies the email address that the exception information will be sent from
        /// </summary>
        public string EmailFrom
        {
            get
            {
                return emailFrom;
            }
            set
            {
                emailFrom = value;
            }
        }

        private string emailTo = "songjz179@163.com";
        /// <summary>
        /// Specifies the email address that the exception information will be sent to
        /// </summary>
        public string EmailTo
        {
            get
            {
                return emailTo;
            }
            set
            {
                emailTo = value;
            }
        }
    }
}
