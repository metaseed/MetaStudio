using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Windows.Threading;

namespace Metaseed.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Simulate Application.DoEvents function of <see cref=" System.Windows.Forms.Application"/> class.
        /// http://msdn.microsoft.com/en-us/library/system.windows.forms.application.doevents.aspx
        /// </summary>
        [SecurityPermissionAttribute ( SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode )]
        public static void DoEvents ( )
        {
            DispatcherFrame frame = new DispatcherFrame ( );
            /*Dispatcher.CurrentDispatcher.*/
            MessageListener.Instance.Dispatcher.BeginInvoke ( DispatcherPriority.Background,
                new DispatcherOperationCallback ( ExitFrames ), frame );
            try
            {
                Dispatcher.PushFrame ( frame );
            }
            catch ( InvalidOperationException )
            {
            }
        }

        private static object ExitFrames ( object frame )
        {
            ( ( DispatcherFrame ) frame ).Continue = false;

            return null;
        }
    }
}
