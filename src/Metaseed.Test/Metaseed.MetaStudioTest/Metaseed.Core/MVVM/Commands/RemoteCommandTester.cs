using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metaseed.MVVM.Commands;
using Metaseed.MVVM.Commands.Ribbon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Metaseed.MetaStudioTest.Metaseed.Core.MVVM.Commands
{
    using Metaseed.Core.MVVM;
    [TestClass]
    public class RemoteCommandTester
    {
        [TestMethod]
        public void TestRemoteCommandManager_Register_Excute_CommandPara()
        {
            var remoteCommandServiceServer = new FakeRemoteCommandServer();
            var serviceController = new RemoteCommandServiceController(remoteCommandServiceServer);
            serviceController.Start();
            var remoteCommandService = new RemoteCommandService();
            try
            {
                remoteCommandService.Open();
                Debug.WriteLine("\n register command");
                var clientCommand = new MyClientCommand(remoteCommandService, "id",
                    new CommandUIData() { Header = "text", IconURI = "icon", IsCheckable = false, IsChecked = false }.Serialize());
                remoteCommandService.Register(clientCommand);
                ((UI)(remoteCommandServiceServer.UIBuilder)).TestExecute();
                bool r=clientCommand.event_Exec.WaitOne(8000);
                Assert.IsTrue(r);
                clientCommand.RaiseCanExecuteChanged(null,null);
                var c=clientCommand.event_CanExec.WaitOne(8000);
                Assert.IsTrue(c);
                remoteCommandService.Close();
                
            }
            catch (TimeoutException e)
            {
                Debug.WriteLine("The service operation timed out. " + e.Message);
                remoteCommandService.Abort();
                Assert.IsTrue(false);
            }
            catch (FaultException<RemoteCommandFault> e)
            {
                Debug.WriteLine("Message: {0}, Description: {1}", e.Detail.Message, e.Detail.Description);
                remoteCommandService.Abort();
                Assert.IsTrue(false);
            }
            catch (FaultException e)
            {
                Debug.WriteLine(e.Message);
                remoteCommandService.Abort();
                Assert.IsTrue(false);
            }
            catch (CommunicationException e)
            {
                Debug.WriteLine("There was a communication problem. " + e.Message + e.StackTrace);
                remoteCommandService.Abort();
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + e.StackTrace);

                remoteCommandService.Abort();
                Assert.IsTrue(false);
            }
            finally
            {
                serviceController.Stop();
            }
        }
    }



}
