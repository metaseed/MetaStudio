﻿using System;
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
    using Metaseed.Core.MVVM;
    [TestClass]
    public class RemoteCommandTester
    {
        [TestMethod]
        public void TestRemoteCommandManager_Excute_CommandPara()
        {
            var remoteCommandServiceServer = new FakeRemoteCommandServer();
            var serviceController = new RemoteCommandServiceController(remoteCommandServiceServer);
            serviceController.Start();
            var remoteCommandService = new RemoteCommandService();
            try
            {
                remoteCommandService.Open();
                Debug.WriteLine("\n register command");
                var command = new MyCommand(remoteCommandService, "id",
                    new CommandUIData() { Text = "text", IconURL = "icon", IsCheckable = false, IsChecked = false });
                remoteCommandService.Register(command);

                Thread.Sleep(1500);
                remoteCommandService.Close();
                Assert.IsTrue(command.excuted);
            }
            catch (TimeoutException e)
            {
                Debug.WriteLine("The service operation timed out. " + e.Message);
                remoteCommandService.Abort();
                Assert.IsTrue(false);
            }
            catch (FaultException<ValidationFault> e)
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