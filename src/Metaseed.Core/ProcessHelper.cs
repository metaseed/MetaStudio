using System;
using System.Diagnostics;
namespace Metaseed
{
    /// <summary>
    /// Helper class for environment information.
    /// </summary>
    public static class ProcessHelper
    {
        private static readonly Lazy<bool> HostedByVisualStudio = new Lazy<bool>(new Func<bool>(IsProcessCurrentlyHostedByVisualStudio));
        private static readonly Lazy<bool> HostedByExpressionBlend = new Lazy<bool>(new Func<bool>(IsProcessCurrentlyHostedByExpressionBlend));
        /// <summary>
        /// Determines whether the process is hosted by visual studio.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByVisualStudio
        {
            get
            {
                return HostedByVisualStudio != null && HostedByVisualStudio.Value;
            }
        }
        /// <summary>
        /// Determines whether the process is hosted by expression blend.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByExpressionBlend
        {
            get
            {
                return HostedByExpressionBlend != null && HostedByExpressionBlend.Value;
            }
        }
        /// <summary>
        /// Determines whether the process is hosted by any tool, such as visual studio or blend.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by any tool, such as visual studio or blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByTool
        {
            get
            {
                return IsProcessHostedByVisualStudio || IsProcessHostedByExpressionBlend;
            }
        }
        /// <summary>
        /// Determines whether the process is hosted by visual studio.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the 
        /// <see cref="P:Metaseed.ProcessHelper.IsProcessHostedByVisualStudio" /> instead.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByVisualStudio()
        {
            return Process.GetCurrentProcess().IsHostedByVisualStudio();
        }
        /// <summary>
        /// Determines whether the process is hosted by expression blend.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the 
        /// <see cref="P:Metaseed.ProcessHelper.IsProcessHostedByExpressionBlend" /> instead.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByExpressionBlend()
        {
            return Process.GetCurrentProcess().IsHostedByExpressionBlend();
        }
        /// <summary>
        /// Determines whether the process is hosted by any tool, such as visual studio or blend.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the 
        /// <see cref="P:Metaseed.ProcessHelper.IsProcessHostedByTool" /> instead.
        /// </summary>
        /// <returns><c>true</c> if the current process is hosted by any tool; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByTool()
        {
            return Process.GetCurrentProcess().IsProcessCurrentlyHostedByTool();
        }

        public static bool IsHostedByExpressionBlend(this Process currentProcess)
        {
            return currentProcess.ProcessName.StartsWith("blend", StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsHostedByVisualStudio(this Process currentProcess)
        {
            return currentProcess.ProcessName.StartsWith("devenv", StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsProcessCurrentlyHostedByTool(this Process currentProcess)
        {
            return currentProcess.IsHostedByVisualStudio() || currentProcess.IsHostedByExpressionBlend();
        }
    }
}