using System.Collections.Generic;
using Catel.MVVM;
using Catel.Data;
using Catel.Messaging;

namespace Metaseed.Modules.Browser.ViewModels
{
    using MetaShell.ViewModels;
    using ComponentModel;

    [LocalizedDisplayName("Metaseed.MetaShell:BrowserResource:BrowserDocument")]
    public class BrowserDocumentViewModel : DocumentViewModel
    {
        private readonly List<string> _previousPages = new List<string>();
        private readonly List<string> _nextPages = new List<string>();
        IMessageMediator _MessageMediator;
        public BrowserDocumentViewModel(IMessageMediator messageMediator)
        {
            _MessageMediator = messageMediator;
            Url = "http://www.metaseed.com";
            GoBack = new Command(OnGoBackExecute, OnGoBackCanExecute);
            GoForward = new Command(OnGoForwardExecute, OnGoForwardCanExecute);
            Browse = new Command(OnBrowseExecute, OnBrowseCanExecute);   
        }
        protected override string GetLocalizedTitle()
        {
            return Metaseed.MetaShell.Framework.Browser.Resources.BrowserResource.BrowserDocument;
        }
        #region Properties
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets the recent sites.
        /// </summary>
        /// <value>
        /// The recent sites.
        /// </value>
        public string[] RecentSites { get { return new[] { "Metaseed" }; } }

        #region SelectedSite property

        /// <summary>
        /// Gets or sets the SelectedSite value.
        /// </summary>
        public string SelectedSite
        {
            get { return GetValue<string>(SelectedSiteProperty); }
            set { SetValue(SelectedSiteProperty, value); }
        }

        /// <summary>
        /// SelectedSite property data.
        /// </summary>
        public static readonly PropertyData SelectedSiteProperty = RegisterProperty("SelectedSite", typeof(string), null, OnSelectedSiteChanged);

        /// <summary>
        /// Called when the SelectedSite value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AdvancedPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedSiteChanged(object sender, AdvancedPropertyChangedEventArgs e)
        {
            var _this = ((BrowserDocumentViewModel)sender);

            switch (_this.SelectedSite)
            {
                case "Metaseed":
                    _this.Url = "http://www.metaseed.com";
                    break;
                default:
                    return;
            }

            _this.OnBrowseExecute();
        }
        #endregion

        #endregion

        #region Commands
        /// <summary>
        /// Gets the GoBack command.
        /// </summary>
        public Command GoBack { get; private set; }

        /// <summary>
        /// Method to check whether the GoBack command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnGoBackCanExecute()
        {
            return _previousPages.Count > 0;
        }

        /// <summary>
        /// Method to invoke when the GoBack command is executed.
        /// </summary>
        private void OnGoBackExecute()
        {
            // TODO: Handle command logic here
        }

        /// <summary>
        /// Gets the GoForward command.
        /// </summary>
        public Command GoForward { get; private set; }

        /// <summary>
        /// Method to check whether the GoForward command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnGoForwardCanExecute()
        {
            return _nextPages.Count > 0;
        }

        /// <summary>
        /// Method to invoke when the GoForward command is executed.
        /// </summary>
        private void OnGoForwardExecute()
        {
            // TODO: Handle command logic here
        }

        /// <summary>
        /// Gets the Browse command.
        /// </summary>
        public Command Browse { get; private set; }

        /// <summary>
        /// Method to check whether the Browse command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnBrowseCanExecute()
        {
            return !string.IsNullOrWhiteSpace(Url);
        }

        /// <summary>
        /// Method to invoke when the Browse command is executed.
        /// </summary>
        private void OnBrowseExecute()
        {
            var url = Url;
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }

            _MessageMediator.SendMessage(url, typeof(BrowserDocumentViewModel).Name);
            //Title = string.Format("Browser: {0}", url);
        }

        
        #endregion

    }
}
