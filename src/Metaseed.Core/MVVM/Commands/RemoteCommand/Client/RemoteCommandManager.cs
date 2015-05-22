using System.Collections.Generic;
using System.ComponentModel;

namespace Metaseed.MVVM.Commands
{
    internal class RemoteCommandManager
    {
        readonly RemoteCommandService _remoteCommandService;
        private readonly Dictionary<string, IRemoteCommand> _commands = new Dictionary<string, IRemoteCommand>();
        internal RemoteCommandManager(RemoteCommandService remoteCommandService)
        {
            _remoteCommandService = remoteCommandService;
        }
        internal RemoteCommand this[string commandID]
        {
            get
            {
                return (RemoteCommand)_commands[commandID];
            }
        }
        internal RemoteCommand Add(string commandID, string uiData)
        {
            var command = new RemoteCommand(_remoteCommandService, commandID, uiData);
            _commands.Add(commandID, command);
            return command;
        }

        internal void Add(IRemoteCommand command)
        {
            _commands.Add(command.ID, command);
        }

        internal void Remove(string commandID)
        {
            _commands.Remove(commandID);
        }

    }

}

