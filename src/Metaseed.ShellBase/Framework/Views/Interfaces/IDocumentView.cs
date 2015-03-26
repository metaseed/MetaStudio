
namespace Metaseed.MetaShell.Views
{
    using Catel.Windows.Controls;

    /// <summary>
    /// Interface defining a document view.
    /// </summary>
    public interface IDocumentView : ILayoutContentView
    {
        #region Methods
        /// <summary>
        /// Closes the document.
        /// </summary>
        void Close();
        #endregion
    }
}