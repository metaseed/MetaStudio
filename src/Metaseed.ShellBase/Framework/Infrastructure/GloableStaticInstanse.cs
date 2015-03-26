using System.Diagnostics;
using System;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
namespace Metaseed.MetaShell.Infrastructure
{
    public enum AppStatus
    {
        Starting,
        StaringAndOpeningPackage,
        Running,
        OpeningPackage,
        SavingPackage,
        Closing
    }
    public class GloableStaticInstanse
    {
        public static AppStatus AppStatus;
        /// <summary>
        /// initialized in App static constructor
        /// </summary>
        public static Stopwatch StopWatch;
        /// <summary>
        /// using? Process.GetCurrentProcess().StartInfo.Arguments or Application.Current.Host.InitParams
        /// just the initial input file path, for current file path using ShellViewModel.CurrentCurrentPackagePath
        /// </summary>
        public static string StartupInputFilePathName = String.Empty;
    }
    //public class InterestedAppSettings
    //{
    //    public static StringCollection RecentFiles;
    //    public static string Culture;
    //}
}
