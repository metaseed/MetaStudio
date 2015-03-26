using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catel.MVVM;

namespace Metaseed.MetaShell
{
    public class Authentication : IAuthenticationProvider
    {
        public bool CanCommandBeExecuted(ICatelCommand command, object commandParameter)
        {
            //https://catelproject.atlassian.net/wiki/display/CTL/Commands+authentication?src=search
            return true;
        }

        public bool HasAccessToUIElement(System.Windows.FrameworkElement element, object tag, object authenticationTag)
        {
            //https://catelproject.atlassian.net/wiki/display/CTL/Authentication?src=search
            return true;
        }
    }
}
