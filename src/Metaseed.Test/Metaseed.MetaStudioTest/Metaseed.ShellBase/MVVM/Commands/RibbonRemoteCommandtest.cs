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
    public class RibbonButtonRemoteCommand_Tester1
    {
        [TestMethod]
        public void RibbonButtonRemoteCommandSerialDeserial1()
        {
        //    var list=new RibbonCommandsList();
        //    RibbonCommandsList.Types.Add(typeof(MyClientCommand));
        //    list.CommandList.Add(new MyClientCommand(null, "id", RibbonUIType.Button.ToString(),
        //new RibbonButtonUIData()
        //{
        //    UiPosition = new RibbonUIPosition()
        //    {
        //        GroupBox = new RibbonUIPositionGroupBox() { Name = "aaa" },
        //    },
        //    IconURI = "pack://application:,,,/☯ModuleDemo;component/Resources/Images/ModuleDemoLogo.png",
        //}.ValidateAndSerialize()));

        //    list.CommandList.Add(new RemoteCommand(null, "id2", RibbonUIType.Button.ToString(),
        //new RibbonButtonUIData()
        //{
        //    UiPosition = new RibbonUIPosition()
        //    {
        //        GroupBox = new RibbonUIPositionGroupBox() { Name = "aaa" },
        //    },
        //    IconURI = "pack://application:,,,/☯ModuleDemo;component/Resources/Images/ModuleDemoLogo.png",
        //}.ValidateAndSerialize()));

        //   var st= list.Serialize();
        //    Assert.IsNotNull(st);
        }
    }
}
