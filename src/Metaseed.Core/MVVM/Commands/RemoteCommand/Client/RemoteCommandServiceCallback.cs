namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandServiceCallback : IRemoteCommandServiceCallback
    {
        internal RemoteCommandService RemoteCommandService;
        public void Excute(string commandID, object parameter)
        {
            RemoteCommandService.commandManager[commandID].Execute(parameter);
        }

        public bool CanExcute(string commandID, object param)
        {
            return RemoteCommandService.commandManager[commandID].CanExecute(param);
        }
    }
}
