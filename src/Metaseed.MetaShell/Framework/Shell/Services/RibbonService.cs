using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Catel;
using Catel.Logging;
using Catel.MVVM.Services;
using Fluent;


namespace Metaseed.MetaShell.Services
{
    using Views;
    public class ObjectWithContexualRibbon
    {
        /// <summary>
        /// could be ribbon tab tabgroup or item
        /// </summary>
        List<FrameworkElement> ContexualRibbonObjects;
    }

    public class RibbonService : MetaService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IDispatcherService _dispatcherService;
        private readonly ShellRibbon _ribbon;
        public RibbonService(IDispatcherService dispatcherService, ShellRibbon ribbon)
        {
            Argument.IsNotNull("dispatcherService", dispatcherService);
            Argument.IsNotNull("ribbon", ribbon);
            _dispatcherService = dispatcherService;
            _ribbon = ribbon;
            _ribbon.Loaded += OnRibbonLoaded;
            ScreenTip.HelpPressed += OnScreenTipHelpPressed;
        }
        /// <summary>
        /// Handles F1 pressed on ScreenTip with help capability
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        static void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
        {
            // Show help according the given help topic
            // (here just show help topic as string)
            MessageBox.Show(e.HelpTopic.ToString());
        }
        /// <summary>
        /// Called when the ribbon is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        /// <remarks>
        /// This is a workaround. If this code is not used, you will get issues with showing the second contextual tab group.
        /// </remarks>
        private void OnRibbonLoaded(object sender, RoutedEventArgs e)
        {
            _ribbon.Loaded -= OnRibbonLoaded;

            foreach (var group in _ribbon.ContextualGroups)
            {
                group.Visibility = Visibility.Visible;
            }

            _dispatcherService.BeginInvoke(() =>
            {
                foreach (var group in _ribbon.ContextualGroups)
                {
                    group.Visibility = Visibility.Hidden;
                }

                //ActivateTabForCurrentlySelectedDocumentView();
            });
        }

    }
}
