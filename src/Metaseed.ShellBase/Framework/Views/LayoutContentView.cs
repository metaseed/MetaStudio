
namespace Metaseed.MetaShell.Views
{
    using Catel.Windows.Controls;
    using ViewModels;

    public abstract class LayoutContentView : MetaView, IDocumentView
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutContentView"/> class. 
        /// </summary>
        public LayoutContentView(LayoutContentViewModel viewModel):base(viewModel)
        {
            CloseViewModelOnUnloaded = false;
        }
        #endregion

        #region IDocumentView Members
        /// <summary>
        /// Closes the document.
        /// </summary>
        public void Close()
        {
            ViewModel.CloseViewModel(null);
        }
        #endregion

    }
}