using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metaseed.MVVM.Commands.Ribbon;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Metaseed.MetaStudioTest.Metaseed.Core.MVVM.Commands
{
    [TestClass]
    public class RemoteCommand_Tester
    {
        [TestMethod]
       public void Test()
        {
            var uidata =
                new RibbonRemoteCommandUIData() { Header = "text", IconURI = "/☯ModuleDemo;component/Resources/Images/ModuleDemoLogo.png", IsCheckable = false, IsChecked = false }
                    .Serialize();
            var s=RibbonRemoteCommandUIData.Deserialize(uidata);
        }
    }
}
