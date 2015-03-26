using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Deployment.Application;
using System.Windows;
using System.Diagnostics;
namespace Metaseed.MetaShell.Services
{
    public static class ExceptionMessage
    {
        private static TimeSpan GetSystemUpTime()
        {
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            return TimeSpan.FromSeconds(upTime.NextValue());
        }

        // use to get memory available
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        static private string GetExceptionTypeStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(GetExceptionTypeStack(e.InnerException));
                message.AppendLine("   " + e.GetType().ToString());
                return (message.ToString());
            }
            else
            {
                return "   " + e.GetType().ToString();
            }
        }

        static private string GetExceptionMessageStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(GetExceptionMessageStack(e.InnerException));
                message.AppendLine("   " + e.Message);
                return (message.ToString());
            }
            else
            {
                return "   " + e.Message;
            }
        }

        static private string GetExceptionCallStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(GetExceptionCallStack(e.InnerException));
                message.AppendLine("--- Next Call Stack:");
                message.AppendLine(e.StackTrace);
                return (message.ToString());
            }
            else
            {
                //// Get stack trace for the exception with source file information
                //var st = new StackTrace(e, true);
                //// Get the top stack frame
                //var frame = st.GetFrame(0);
                //// Get the line number from the stack frame
                //var line = frame.GetFileLineNumber();
                //StringBuilder sb = new StringBuilder("LineNumber:" + line); sb.Append(System.Environment.NewLine);
                //sb.Append(e.StackTrace);
                return e.StackTrace;//sb.ToString();
            }
        }
        static public StringBuilder BuildExceptionMessage(Exception exception)
        {
            StringBuilder error = new StringBuilder();

            //error.AppendLine("Application:       " + Application.ProductName);
            //error.AppendLine("Version:           " + Application.ProductVersion);
            //error.AppendLine("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            //error.AppendLine("Computer name:     " + SystemInformation.ComputerName);
            //error.AppendLine("User name:         " + SystemInformation.UserName);
            //error.AppendLine("OS:                " + Environment.OSVersion.ToString());
            //error.AppendLine("Culture:           " + CultureInfo.CurrentCulture.Name);
            //error.AppendLine("Resolution:        " + SystemInformation.PrimaryMonitorSize.ToString());
            //error.AppendLine("System up time:    " + GetSystemUpTime());
            //error.AppendLine("App up time:       " +
            //  (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();

            error.AppendLine("Application:       " + assembly.GetName().Name);
            //http://msdn.microsoft.com/en-us/library/system.deployment.application.applicationdeployment.aspx
            //if (ApplicationDeployment.IsNetworkDeployed)
            var myVersion = new System.Version("0.0.0.0");
            try
            {
                myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
            }
            //var version = Environment.OSVersion.Version;
            //if (version.Major > 5 || (version.Major == 5 && version.Minor >= 1 && string.Compare(Environment.OSVersion.ServicePack, "Service Pack 2",true)>=0))
            //{
            //    // this is XP SP2 or higher
            //}Environment.GetEnvironmentVariable("windir") 
            error.AppendLine("DeploymentVersion:           " + myVersion.ToString());
            error.AppendLine("MainAppVersion:           " + assembly.GetName().Version.ToString());
            error.AppendLine("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            //http://stackoverflow.com/questions/1233217/difference-between-systeminformation-computername-environment-machinename-and-n
            error.AppendLine("Computer name:     " + Environment.MachineName);
            error.AppendLine("User name:         " + Environment.UserName);
            error.AppendLine("OS:                " + Environment.OSVersion.ToString());
            error.AppendLine("Culture:           " + CultureInfo.CurrentCulture.Name);
            error.AppendLine(string.Format("Resolution:        (With:{0},Height:{1})", SystemParameters.MaximizedPrimaryScreenWidth, SystemParameters.MaximizedPrimaryScreenHeight));//MaximumWindowTrackHeight
            error.AppendLine("System up time:    " + GetSystemUpTime());
            error.AppendLine("App up time:       " +
              (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());

            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                error.AppendLine("Total memory:      " + memStatus.ullTotalPhys / (1024 * 1024) + "Mb");
                error.AppendLine("Available memory:  " + memStatus.ullAvailPhys / (1024 * 1024) + "Mb");
            }

            error.AppendLine("");

            error.AppendLine("Exception classes:   ");
            error.Append(GetExceptionTypeStack(exception));
            error.AppendLine("");
            error.AppendLine("Exception messages: ");
            error.Append(GetExceptionMessageStack(exception));

            error.AppendLine("");
            error.AppendLine("Stack Traces:");
            error.Append(GetExceptionCallStack(exception));
            error.AppendLine("");
            error.AppendLine("Loaded Modules:");
            Process thisProcess = Process.GetCurrentProcess();
            foreach (ProcessModule module in thisProcess.Modules)
            {
                error.AppendLine(module.FileName + " " + module.FileVersionInfo.FileVersion);
            }
            return error;
        }

       public static void ShowExceptionMessage(Exception exception) {
            WindowLogger wl = new WindowLogger();
            wl.LogError(BuildExceptionMessage(exception).ToString());
        }
    }
}
