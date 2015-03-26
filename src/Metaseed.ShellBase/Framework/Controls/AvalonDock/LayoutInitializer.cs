using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Xceed.Wpf.AvalonDock.Layout;

using Metaseed.MetaShell.Services;
namespace Metaseed.MetaShell.Controls
{
    using ViewModels;
    public class LayoutInitializer : ILayoutUpdateStrategy
    {
        private enum InsertPosition
        {
            Start,
            End
        }
        private static LayoutAnchorablePane CreateAnchorablePane(LayoutRoot layout, Orientation orientation,
            string paneName, InsertPosition position)
        {
            var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == orientation);

            //if (parent == null)
            //{
            //    switch (orientation)
            //    {
            //        case Orientation.Horizontal:
            //            var hpl = new LayoutPanel() { Orientation = Orientation.Horizontal };
            //            System.Diagnostics.Debug.Assert(layout.RootPanel.Orientation == Orientation.Vertical);
            //            layout.RootPanel.InsertChildAt(0, hpl);
            //            break;
            //        case Orientation.Vertical:
            //            //var hp = layout.Descendents().OfType<LayoutPanel>().FirstOrDefault(d => d.Orientation == Orientation.Horizontal);
            //            //if (hp == null)
            //            //{
            //            var hp = new LayoutPanel() { Orientation = Orientation.Horizontal };
            //            hp.InsertChildAt(0, new LayoutDocumentPaneGroup(new LayoutDocumentPane()));
            //            //}
            //            layout.RootPanel = new LayoutPanel(hp) { Orientation = Orientation.Vertical };
            //            parent = layout.RootPanel;
            //            break;
            //        default:
            //            break;
            //    }
            //}
            var toolsPane = new LayoutAnchorablePane { Name = paneName };
            if (position == InsertPosition.Start)
            {
                parent.InsertChildAt(0, toolsPane);
            }
            else
            {
                parent.Children.Add(toolsPane);
            }
            return toolsPane;
        }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            //AD wants to add the anchorable into destinationContainer, just for test provide a new anchorable pane 
            //if the pane is floating let the manager go ahead

            //var destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer != null && destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;

            var toolViewModel = anchorableToShow.Content as IToolViewModel;
            if (toolViewModel != null)
            {
                var preferredLocation = toolViewModel.PreferredLocation;
                string paneName = preferredLocation.ToString();
                var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == paneName);
                if (toolsPane == null)
                {
                    switch (preferredLocation)
                    {
                        case ToolPaneLocation.Left:
                            toolsPane = CreateAnchorablePane(layout, Orientation.Horizontal, paneName, InsertPosition.Start);
                            break;
                        case ToolPaneLocation.Right:
                            toolsPane = CreateAnchorablePane(layout, Orientation.Horizontal, paneName, InsertPosition.End);
                            break;
                        case ToolPaneLocation.Bottom:
                            toolsPane = CreateAnchorablePane(layout, Orientation.Vertical, paneName, InsertPosition.End);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }
            return false;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
            // If this is the first anchorable added to this pane, then use the preferred size.
            var toolViewModel = anchorableShown.Content as IToolViewModel;
            if (toolViewModel != null && toolViewModel.IsVisible == true)
            {
                var anchorablePane = anchorableShown.Parent as LayoutAnchorablePane;
                if (anchorablePane != null && anchorablePane.ChildrenCount == 1)
                {
                    switch (toolViewModel.PreferredLocation)
                    {
                        case ToolPaneLocation.Left:
                        case ToolPaneLocation.Right:
                            anchorablePane.DockWidth = new GridLength(toolViewModel.PreferredWidth, GridUnitType.Pixel);
                            break;
                        case ToolPaneLocation.Bottom:
                            anchorablePane.DockHeight = new GridLength(toolViewModel.PreferredHeight, GridUnitType.Pixel);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {

        }
    }
}