using System;
using System.Collections.Generic;
using System.IO;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Metaseed.MetaShell.Views
{
    using ViewModels;
    internal static class LayoutSaveLoadHelper
    {
        public static void SaveLayout(DockingManager manager, Stream stream)
        {
            var layoutSerializer = new XmlLayoutSerializer(manager);
            layoutSerializer.Serialize(stream);
        }

        public static void LoadLayout(DockingManager manager, Stream stream, Action<IDocumentViewModel> addDocumentCallback,
                                      Action<IToolViewModel> addToolCallback, Dictionary<string, ILayoutContentViewModel> items)
        {
            var layoutSerializer = new XmlLayoutSerializer(manager);

            layoutSerializer.LayoutSerializationCallback += (s, e) =>
                {
                    ILayoutContentViewModel item;
                    if (items.TryGetValue(e.Model.ContentId, out item))
                    {
                        e.Content = item;

                        var tool = item as IToolViewModel;
                        var anchorable = e.Model as LayoutAnchorable;

                        var document = item as IDocumentViewModel;
                        var layoutDocument = e.Model as LayoutDocument;

                        if (tool != null && anchorable != null)
                        {
                            addToolCallback(tool);
                            tool.IsVisible = anchorable.IsVisible;

                            if (anchorable.IsActive)
                                tool.IsActive=true;

                            tool.IsSelected = e.Model.IsSelected;

                            return;
                        }

                        if (document != null && layoutDocument != null)
                        {
                            addDocumentCallback(document);
                            document.IsSelected = layoutDocument.IsSelected;
                            return;
                        }
                    }

                    // Don't create any panels if something went wrong.
                    e.Cancel = true;
                };

            try
            {
                layoutSerializer.Deserialize(stream);
            }
            catch
            {
            }
        }
    }
}