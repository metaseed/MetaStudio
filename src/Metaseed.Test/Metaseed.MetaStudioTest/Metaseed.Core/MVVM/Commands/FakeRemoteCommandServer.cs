using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metaseed.MetaStudioTest.Metaseed.Core.MVVM.Commands;
using Metaseed.MVVM.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Metaseed.MetaStudioTest.Metaseed.Core.MVVM.Commands
{


    class MyClientCommand : RemoteCommand
    {
        public MyClientCommand(IRemoteCommandService commandService, string id, CommandUIData uiData)
            : base(commandService, id, uiData)
        {
            UIData = uiData;
        }
        public override bool CanExecute(object parameter)
        {
            event_CanExec.Set();
            return false;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            //Assert.AreEqual(((int)parameter), 1);
            Assert.AreEqual(parameter, "string");
            event_Exec.Set();
        }
        internal AutoResetEvent event_Exec = new AutoResetEvent(false);
        internal AutoResetEvent event_CanExec = new AutoResetEvent(false);
    }
     class UI : IRemoteCommandUIBuilder
    {
        private RemoteCommandService_Server _remoteCommandService_Server;
        public UI(RemoteCommandService_Server remoteCommandService_Server)
        {
            _remoteCommandService_Server = remoteCommandService_Server;
        }

        public void GenerateUI(CompositeRemoteCommand command)
        {

            RegisterCanExecuteEvent();
        }

        void RegisterCanExecuteEvent()
        {
            var commands = _remoteCommandService_Server.CommandManager.Commands["id"];
            commands.CanExecuteChanged += delegate(object sender, EventArgs e)
            {
                var r = commands.CanExecute("canExecute");
                Assert.IsFalse(r);
            };
        }
        internal void TestExecute()
        {
            var commands = _remoteCommandService_Server.CommandManager.Commands["id"];
            //commands.Execute(1);
            commands.Execute("string");
        }


        public void RemoveUI(string commandID)
        {
            throw new NotImplementedException();
        }
    }
    internal class FakeRemoteCommandServer : RemoteCommandService_Server
    {
        public FakeRemoteCommandServer()
        {
            this.UIBuilder = new UI(this);
        }
    }
}
