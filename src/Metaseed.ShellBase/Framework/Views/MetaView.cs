
namespace Metaseed.MetaShell.Views
{
    using Catel.Windows.Controls;
    using ViewModels;

    public abstract class MetaView : UserControl
    {
        #region Constructors
        public MetaView(LayoutContentViewModel viewModel)
            : base(viewModel)
        {
            if (AppEnvironment.IsInDesignMode)
            {
                return;
            }
            this.Loaded += MetaView_Loaded;
        }

        void MetaView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.ViewModel is MetaViewModel)
            {
                var metaViewModel = this.ViewModel as MetaViewModel;
                metaViewModel.OnViewLoaded(this);
            }

        }
        #endregion
    }
}