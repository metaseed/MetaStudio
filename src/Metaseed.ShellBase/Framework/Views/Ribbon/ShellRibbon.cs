using System.ComponentModel;
using System.Runtime.CompilerServices;
using Fluent;
using Metaseed.MetaShell.Annotations;
using Metaseed.MetaShell.ViewModels;

namespace Metaseed.MetaShell.Views
{
    public class ShellRibbon : Ribbon, INotifyPropertyChanged
    {
        ShellViewModel _shellViewModel;
        protected ShellViewModel ShellViewModel
        {
            get { return _shellViewModel ?? (_shellViewModel = (ShellViewModel) this.DataContext); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsBackstageOpen 
        {
            get
            {
                var backstage = this.Menu as Backstage;
                if (backstage != null) 
                    return backstage.IsOpen;
                return false;
            }
        }
    }
}
