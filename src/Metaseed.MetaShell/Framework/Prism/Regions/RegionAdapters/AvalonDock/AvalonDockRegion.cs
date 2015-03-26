using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Regions.Behaviors;
using System.ComponentModel;
using Microsoft.Practices.ServiceLocation;
namespace Metaseed.MetaShell.Prism.Regions
{
    using Views;
    public class AvalonDockRegion : DependencyObject
    {
        #region Name

        /// <summary>
        /// Name Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(AvalonDockRegion),
                new FrameworkPropertyMetadata((string)null,
                    new PropertyChangedCallback(OnRegionNameChanged)));

        /// <summary>
        /// Gets the Name property.  This dependency property 
        /// indicates the region name of the layout item.
        /// </summary>
        public static string GetRegionName(DependencyObject d)
        {
            return (string)d.GetValue(RegionNameProperty);
        }

        /// <summary>
        /// Sets the Name property.  This dependency property 
        /// indicates the region name of the layout item.
        /// </summary>
        public static void SetRegionName(DependencyObject d, string value)
        {
            d.SetValue(RegionNameProperty, value);
        }

        /// <summary>
        /// Handles changes to the Name property.
        /// </summary>
        private static void OnRegionNameChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            CreateRegionWithRegionAdapter(s, (string)e.NewValue);
        }

        #endregion


        static void CreateRegionWithRegionAdapter(DependencyObject element, string regionName)
        {
            if (element == null) 
                throw new ArgumentNullException("element");

            //If I'm in design mode the main window is not set
            if (Application.Current == null ||
                Application.Current.MainWindow == null)
                return;
            var shellView = ServiceLocator.Current.GetInstance<ShellView>();
            if (shellView==null)
            {
                throw new ArgumentNullException("AvalonDockRegion.shellView");
            }
            shellView.Loaded += (sender, e) =>
            {
                //try
                {
                    if (ServiceLocator.Current == null)
                        return;

                    // Build the region
                    var mappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
                    if (mappings == null)
                        return;
                    IRegionAdapter regionAdapter = mappings.GetMapping(element.GetType());
                    if (regionAdapter == null)
                        return;

                    regionAdapter.Initialize(element, regionName);
                }
                //catch (Exception ex)
                //{
                //    throw new RegionCreationException(string.Format("Unable to create region {0}", regionName), ex);
                //}
            };


        }

        static void shellView_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
