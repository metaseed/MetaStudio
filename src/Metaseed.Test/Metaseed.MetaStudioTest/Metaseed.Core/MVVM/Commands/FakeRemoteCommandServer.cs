using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Metaseed.MVVM.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Metaseed.MetaStudioTest.Metaseed.Core.MVVM.Commands
{


    class MyCommand : RemoteCommand
    {
        public MyCommand(IRemoteCommandService commandService, string id, CommandUIData uiData)
            : base(commandService, id, uiData)
        {
            UIData = uiData;
        }
        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter);
        }

        public bool excuted = false;
        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            //Assert.AreEqual(((int)parameter), 1);
            Assert.AreEqual(parameter,"string");
            excuted = true;
        }

    }
    class UI : IRemoteCommandUIBuilder
    {
        private RemoteCommandService_Server _remoteCommandService_Server;
        public UI(RemoteCommandService_Server remoteCommandService_Server)
        {
            _remoteCommandService_Server = remoteCommandService_Server;
        }

        public void GenerateUI(RemoteCommandDelegate command)
        {
            var commands = _remoteCommandService_Server.CommandManager.GetCommands("id");
            foreach (var remoteCommandDelegate in commands)
            {
                //remoteCommandDelegate.Execute(1);
                remoteCommandDelegate.Execute( "string");
            }
        }
    }
    internal class FakeRemoteCommandServer:RemoteCommandService_Server
    {
        public FakeRemoteCommandServer()
        {
            this.UIBuilder=new UI(this);
        }
    }
}
