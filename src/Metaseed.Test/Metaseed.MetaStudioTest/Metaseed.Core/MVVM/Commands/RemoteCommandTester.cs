using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
        public void TestRemoteCommandManager_RepeatRegistCommand()
        {
            var remoteCommandManagerService = new RemoteCommandManagerService();
            remoteCommandManagerService.Start();

            var remoteCommandManager= new RemoteCommandManager();
            try
            {
                remoteCommandManager.Open();

                Debug.WriteLine("\n register"); 
                remoteCommandManager.Register("id","text","icon");
                remoteCommandManager.Register("id", "text", "icon");

                remoteCommandManager.Close();
                Assert.IsTrue(false);
            }
            catch (TimeoutException e)
            {
                Debug.WriteLine("The service operation timed out. " + e.Message);
                remoteCommandManager.Abort();
            }
            // Catch the contractually specified SOAP fault raised here as an exception. 
            catch (FaultException<ValidationFault> e)
            {
                Debug.WriteLine("Message: {0}, Description: {1}", e.Detail.Message, e.Detail.Description);
                remoteCommandManager.Abort();
                Assert.IsTrue(true);
            }
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF 
            // services when ServiceDebugBehavior.IncludeExceptionDetailInFaults  
            // is set to true or when un-typed FaultExceptions raised.
            catch (FaultException e)
            {
                Debug.WriteLine(e.Message);
                remoteCommandManager.Abort();
            }
            // Standard communication fault handler. 
            catch (CommunicationException e)
            {
                Debug.WriteLine("There was a communication problem. " + e.Message + e.StackTrace);
                remoteCommandManager.Abort();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                remoteCommandManager.Abort();
            }
        }
        [TestMethod]
        public void TestRemoteCommandManager_()
        {

        }
    }

    
       
}
