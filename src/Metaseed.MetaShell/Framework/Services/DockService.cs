using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock;
using Catel;
using Catel.IoC;
using Catel.Windows.Controls;
using Catel.Logging;
using Microsoft.Practices.Prism.Regions;
namespace Metaseed.MetaShell.Services
{
    using Controls;
    using Views;
    public class DockService : MetaService
    {
        #region static members

        static DockingManager DockingManager;
        static LayoutDocumentPane LayoutDocumentPane;
        static IRegionManager RegionManager;
        public DockService()
        {
            DockingManager = ServiceLocator.Default.ResolveType<DockingManager>();
            //DockingManager.DocumentClosed += OnDockingManagerDocumentClosed;
            LayoutDocumentPane = ServiceLocator.Default.ResolveType<LayoutDocumentPane>();
            RegionManager = ServiceLocator.Default.ResolveType<IRegionManager>();
        }
        #region Properties

        #endregion
        #region Methods
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The found document or <c>null</c> if no document was found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType" /> is <c>null</c>.</exception>
        public static LayoutDocument FindDocument(Type viewType, object tag = null)
        {
            Argument.IsNotNull("viewType", viewType);

            return (from document in LayoutDocumentPane.Children where document is LayoutDocument && document.Content.GetType() == viewType && TagHelper.AreTagsEqual(tag, ((IView)document.Content).Tag) select document).Cast<LayoutDocument>().FirstOrDefault();
        }
        
        /// <summary>
        /// Activates the document in the docking manager, which makes it the active document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="document" /> is <c>null</c>.</exception>
        public static void ActivateDocument(LayoutDocument document)
        {
            Argument.IsNotNull("document", document);

            LayoutDocumentPane.SelectedContentIndex = LayoutDocumentPane.IndexOfChild(document);
        }

        /// <summary>
        /// Gets currently activated document in the docking manager.
        /// </summary>
        /// <returns>The active document.</returns>
        public static LayoutDocument GetActiveDocument()
        {
            return LayoutDocumentPane.Children[LayoutDocumentPane.SelectedContentIndex] as LayoutDocument;
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The created layout document.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        //public static LayoutDocument CreateDocument(IDocumentViewModel view, object tag = null)
        //{
        //    Argument.IsNotNull("view", view);

        //    var layoutDocument = WrapViewInLayoutDocument(view, tag);

        //    LayoutDocumentPane.Children.Add(layoutDocument);

        //    return layoutDocument;
        //}

        /// <summary>
        /// Wraps the view in a layout document.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>A wrapped layout document.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        //private static LayoutDocument WrapViewInLayoutDocument(IDocumentViewModel view, object tag = null)
        //{
            //return new LayoutDocumentBindable(view, tag);
        //}

        /// <summary>
        /// Called when the docking manager has just closed a document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DocumentClosedEventArgs" /> instance containing the event data.</param>
        //private static void OnDockingManagerDocumentClosed(object sender, DocumentClosedEventArgs e)
        //{
        //    var containerView = e.Document;
        //    var view = containerView.Content as IDocumentView;
        //    if (view != null)
        //    {
        //        view.CloseDocument();
        //    }

        //    // var region = RegionManager.Regions[(string)view.Tag];
        //    // region.Remove(sender);
        //}

        /// <summary>
        /// Closes the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="document" /> is <c>null</c>.</exception>
        public static void CloseDocument(LayoutDocument document)
        {
            Argument.IsNotNull(() => document);

            document.Close();
        }
        #endregion
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Determines whether the anchorable with the specified name is currently visible.
        /// </summary>
        /// <param name="name">
        /// The name of the anchorable.
        /// </param>
        /// <returns>
        /// <c>true</c> if the anchorable with the specified name is visible; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The anchorable with the specified name cannot be found.
        /// </exception>
        static public bool IsAnchorableVisible(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            var anchorable = FindAnchorable(name, true);
            return anchorable.IsVisible;
        }

        /// <summary>
        /// Shows the anchorable with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the anchorable.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The anchorable with the specified name cannot be found.
        /// </exception>
        static public void ShowAnchorable(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            var anchorable = FindAnchorable(name, true);
            anchorable.IsVisible = true;
        }

        /// <summary>
        /// Hides the anchorable with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the anchorable.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The anchorable with the specified name cannot be found.
        /// </exception>
        static public void HideAnchorable(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            var anchorable = FindAnchorable(name, true);
            anchorable.IsVisible = false;
        }
        /// <summary>
        /// Finds the anchorable with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the anchorable.
        /// </param>
        /// <param name="throwExceptionWhenNotFound">
        /// If set to <c>true</c>, this method will throw an <see cref="InvalidOperationException"/> when the anchorable cannot be found.
        /// </param>
        /// <returns>
        /// The <see cref="LayoutAnchorable"/> or <c>null</c> if the anchorable cannot be found.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> is <c>null</c> or whitespace.
        /// </exception>
        static private LayoutAnchorable FindAnchorable(string name, bool throwExceptionWhenNotFound = false)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            var visibleAnchorable = (from child in DockingManager.Layout.Children
                                     where child is LayoutAnchorable && TagHelper.AreTagsEqual(((LayoutAnchorable)child).ContentId, name)
                                     select (LayoutAnchorable)child).FirstOrDefault();
            if (visibleAnchorable != null)
            {
                return visibleAnchorable;
            }

            var invisibleAnchorable = (from child in DockingManager.Layout.Hidden
                                       where TagHelper.AreTagsEqual((child).ContentId, name)
                                       select child).FirstOrDefault();
            if (invisibleAnchorable != null)
            {
                return invisibleAnchorable;
            }

            if (throwExceptionWhenNotFound)
            {
                string error = string.Format("Anchorable with name '{0}' cannot be found", name);
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            return null;
        }
        #endregion static members
        #region object members
        #endregion object members
    }
}
