using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metaseed.MVVM.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Metaseed.MetaStudioTest.Metaseed.Core.MVVM.Commands
{
    [TestClass]
    public class RemoteCommand_Tester1
    {
        [TestMethod]
        public void RibbonButtonUIData_SerializeDeserializeTest()
        {
            var uidata = new RibbonButtonUIData()
                {
                    UiPosition = new RibbonUIPosition()
                    {
                        GroupBox = new RibbonUIPositionGroupBox() {Name = "aaa"},
                    },
                    IconURI = "pack://application:,,,/☯ModuleDemo;component/Resources/Images/ModuleDemoLogo.png",
                }.ValidateAndSerialize();
            var c=RibbonButtonUIData.Deserialize(uidata);
            Assert.IsNotNull(c);
        }
    }
}
