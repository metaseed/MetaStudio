using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Catel.IoC;
using Catel.Messaging;
namespace Metaseed.Modules.Browser.Views
{
    using MetaShell.Views;
    using ViewModels;

    public partial class BrowserDocumentView : DocumentBaseView
    {
        public BrowserDocumentView(BrowserDocumentViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            var messageMediator = Catel.IoC.ServiceLocator.Default.ResolveType<IMessageMediator>();
            messageMediator.Register<string>(this, OnBrowse, typeof(BrowserDocumentViewModel).Name);
             var vm = ViewModel as BrowserDocumentViewModel;
             OnBrowse(vm.Url);
        }
        protected override void OnViewModelChanged()
        {
            var vm = ViewModel as BrowserDocumentViewModel;
            if (vm != null)
            {
                if (!string.IsNullOrWhiteSpace(vm.Url))
                {
                    OnBrowse(vm.Url);
                }
            }
        }
        private void OnBrowse(string url)
        {
            webBrowser.Navigate(url);
        }
    }
}
