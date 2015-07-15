using System;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using Catel.IoC;
using Catel;
using Catel.Logging;
using System.Globalization;
using Catel.Data;

namespace Metaseed.MetaShell.Services
{
    using ViewModels;
    using Infrastructure;
    //using Modules.PropertyGrid.ViewModels;
    using Metaseed.Collections.Generic;
    //using Metaseed.MetaShell.Framework.Shell.Views;
    public class ShellService : ObservableObject, IShellService
    {
        private static object _lock = new object();
        static ILog Log = LogManager.GetCurrentClassLogger();
        static bool _created = false;
        public const string HomeRibbonTabName = "RibbonTabHome";
        private readonly IRibbonService _ribbonService;
        public ShellService(IRibbonService ribbonServce)
        {
            _ribbonService = ribbonServce;
            System.Diagnostics.Debug.Assert(_created == false);
            _created = true;
            _tools = new ObservableCollection<IToolViewModel>();
            BindingOperations.EnableCollectionSynchronization(_tools, _lock);
            _documents = new ObservableCollection<IDocumentViewModel>();
            //Creates the lock object somewhere
            BindingOperations.EnableCollectionSynchronization(_documents, _lock);
            _documentsUnopen = new ObservableCollection<DocumentUnopenBase>();
            BindingOperations.EnableCollectionSynchronization(_documentsUnopen, _lock);
            CurrentCulture = string.IsNullOrEmpty(Metaseed.ShellBase.Properties.Settings.Default.Culture) ? CultureInfo.CurrentUICulture : new CultureInfo(Metaseed.ShellBase.Properties.Settings.Default.Culture);
            CurrentTheme = string.IsNullOrEmpty(Metaseed.ShellBase.Properties.Settings.Default.Theme) ? AppTheme.Office2013 : (AppTheme)Enum.Parse(typeof(AppTheme), Metaseed.ShellBase.Properties.Settings.Default.Theme);
        }

        IMessager _messager;
        public IMessager Messager
        {
            get { return _messager ?? (_messager = ServiceLocator.Default.ResolveType<IMessager>()); }
        }
        CultureInfo _currentCulture;
        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                if (!value.Equals(_currentCulture))
                {
                    var currentCultureOld = _currentCulture;
                    _currentCulture = value;
                    WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = value;
                    //InterestedAppSettings.Culture = value.Name;
                    this.RaisePropertyChanged(() => this.CurrentCulture);
                    Metaseed.ShellBase.Properties.Settings.Default.Culture = value.Name;
                    Metaseed.ShellBase.Properties.Settings.Default.Save();
                    //CurrentCultureChanged.SafeInvoke(this, new AdvancedPropertyChangedEventArgs(this, "CurrentCulture", currentCultureOld, _CurrentCulture));
                    CurrentCultureChangedEvent.SendWith(new Tuple<CultureInfo, CultureInfo>(currentCultureOld, _currentCulture));
                }
            }
        }

        IDocumentViewModel _activeDocument;
        //public event PropertyChangedEventHandler ActiveDocumentChanged;
        /// <summary>
        /// every time the document is added to Documents collection the activeDocument is changed, but it's not shown, means the ui items on it are not loaded.
        /// </summary>
        public IDocumentViewModel ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                if (_activeDocument != value)
                {
                    var activeDocumentOld = _activeDocument;
                    _activeDocument = value;
                    if (activeDocumentOld != null)
                    {
                        var documentViewModel = activeDocumentOld as DocumentBaseViewModel;
                        if (documentViewModel != null)
                            documentViewModel.OnActiveDocumentChanged(activeDocumentOld, _activeDocument);
                    }
                    if (_activeDocument != null)
                    {
                        var documentViewModel = _activeDocument as DocumentBaseViewModel;
                        if (documentViewModel != null)
                            documentViewModel.OnActiveDocumentChanged(activeDocumentOld, _activeDocument);
                        //_ActiveDocument.IsActive = true;
                    }

                    this.RaisePropertyChanged(() => this.ActiveDocument);
                    //ActiveDocumentChanged.SafeInvoke(this, new AdvancedPropertyChangedEventArgs(this, "ActiveDocument", activeDocumentOld, _ActiveDocument));
                    ActiveDocumentChangedEvent.SendWith(new Tuple<IDocumentViewModel, IDocumentViewModel>(activeDocumentOld, _activeDocument));
                }
            }
        }

        private readonly ObservableCollection<IToolViewModel> _tools;
        public ObservableCollection<IToolViewModel> Tools
        {
            get { return _tools; }

        }

        private readonly ObservableCollection<IDocumentViewModel> _documents;
        public ObservableCollection<IDocumentViewModel> Documents
        {
            get { return _documents; }

        }

        public IRibbonService Ribbon { get { return _ribbonService; } }

        readonly ObservableCollection<DocumentUnopenBase> _documentsUnopen;
        /// <summary>
        /// contains docs killed(has been saved) and not killed(may be saved)
        /// </summary>
        public ObservableCollection<DocumentUnopenBase> DocumentsUnopen
        {
            get { return _documentsUnopen; }
        }
        //public event PropertyChangedEventHandler ActiveDocumentChanged;

        //public event PropertyChangedEventHandler CurrentThemeChanged;
        //public event PropertyChangedEventHandler CurrentCultureChanged;
        #region DocumentsAndTools

        protected bool IsClearing = false;
        /// <summary>
        /// just clear the view: close the documents. called before open another file
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            IsClearing = true;
            while (Documents.Count > 0)
            {
                var result = CloseDocument(Documents[0]);
                if (!result)
                {
                    IsClearing = false;
                    return false;
                }
            }
            while (Tools.Count > 0)
            {
                CloseTool(Tools[0]);
            }
            DocumentsUnopen.Clear();
            IsClearing = false;
            return true;
        }

        #endregion
        #region Documents
        public void OpenDocument(IDocumentViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);
            var docViewModel = (viewModel as DocumentBaseViewModel);
            docViewModel.OnBeforeOpen();
            //see comments up.not using Collection.Contains because it using Equals to compare
            if (!Documents.Contains_CompareByReference<IDocumentViewModel>(viewModel))
            {
                Documents.Add(viewModel);
            }
            docViewModel.IsAliveClosed = false;
            viewModel.IsSelected = true;
        }

        public void ActivateDocument(IDocumentViewModel documentViewModel)
        {
            ActiveDocument = documentViewModel;
        }
        void CloseTool(IToolViewModel toolViewModel)
        {
            toolViewModel.CloseViewModel(true);
            Tools.Remove(toolViewModel);
        }
        //public event Action<IDocumentViewModel> DocumentClosed;
        public bool CloseDocument(IDocumentViewModel documentViewModel)
        {
            if ((!documentViewModel.CanClose) && !IsClearing)
            {
                this.Messager.MessageBox.ShowInformation(Metaseed.ShellBase.Properties.Resources.ClosingThisDocumentIsNotAlowed);
                return false;
            }

            if (documentViewModel.IsDataDirty)
            {
                documentViewModel.SaveViewModel();
                Log.Info(documentViewModel.NameText + "-" + Metaseed.ShellBase.Properties.Resources.Saved);
            }
            if (!documentViewModel.KeepAliveWhenClose)
            {
                documentViewModel.CloseViewModel(true);
                var viewModel = documentViewModel as DocumentBaseViewModel;
                if (viewModel != null)
                    viewModel.IsAliveClosed = false;
            }
            else
            {
                var viewModel = documentViewModel as DocumentBaseViewModel;
                if (viewModel != null)
                    viewModel.IsAliveClosed = true;
            }
            if (Application.Current == null || Application.Current.Dispatcher == null) return false;
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { Documents.Remove(documentViewModel); }),
                    DispatcherPriority.Normal);
            }
            else
            {
                Documents.Remove(documentViewModel);
            }

            if (!IsClearing && (documentViewModel.ContentId != null))
            {
                var docUnopen = new DocumentUnopenBase() { ContentId = documentViewModel.ContentId, NameText = documentViewModel.Title, Description = documentViewModel.Description, KeepAliveWhenClose = documentViewModel.KeepAliveWhenClose };
                if (documentViewModel.KeepAliveWhenClose)
                {
                    docUnopen.DocClosedButAlive = documentViewModel;
                }
                DocumentsUnopen.Add(docUnopen);
                var model = documentViewModel as DocumentBaseViewModel;
                if (model != null) model.OnAfterClose();
            }

            DocumentClosedEvent.SendWith(documentViewModel);
            return true;
        }
        public bool CouldDeleteDocument(IDocumentViewModel documentViewModel)
        {
            if (documentViewModel != null)
            {
                if (documentViewModel.IsDeletable == false)
                {
                    return false;
                }
            }
            return true;
        }
        public void DeleteDocument(ILayoutContentViewModel document)
        {
            if (document is IDocumentViewModel)
            {
                var d = document as IDocumentViewModel;
                if (!this.CloseDocument(d))
                {
                    return;
                }
            }
            foreach (var unopenDoc in this.DocumentsUnopen)
            {
                if (unopenDoc.ContentId == document.ContentId)
                {
                    if (unopenDoc.KeepAliveWhenClose)
                    {
                        unopenDoc.DocClosedButAlive.CloseViewModel(true);
                    }
                    this.DocumentsUnopen.Remove(unopenDoc);
                    break;
                }
            }

        }
        #endregion Documents

        /// <summary>
        /// to Hide the Tool just set the viewModel.IsVisible = false;
        /// </summary>
        /// <param name="toolViewModel"></param>
        public void ShowTool(IToolViewModel toolViewModel)
        {
            Argument.IsNotNull("viewModel", toolViewModel);

            if (!Tools.Contains_CompareByReference(toolViewModel))//not using Contains
            {
                Tools.Add(toolViewModel);
            }
            toolViewModel.IsVisible = true;
            toolViewModel.IsSelected = true;
        }
        public void HideTool(IToolViewModel toolViewModel)
        {
            Argument.IsNotNull("toolViewModel", toolViewModel);
            toolViewModel.IsVisible = false;

        }
        internal IToolViewModel GetTool(Guid key)
        {
            foreach (var tool in Tools)
            {
                if (tool.ID.Equals(key))
                {
                    return tool;
                }
            }
            return null;
        }
        public void CloseApp()
        {
            Application.Current.MainWindow.Close();
        }

        private AppTheme currentTheme;
        public AppTheme CurrentTheme
        {
            get { return currentTheme; }
            set
            {
                if (!value.Equals(currentTheme))
                {
                    var currentThemeOld = currentTheme;

                    ChangeTheme(value);
                    this.RaisePropertyChanged(() => this.CurrentTheme);
                    Metaseed.ShellBase.Properties.Settings.Default.Theme = value.ToString();
                    Metaseed.ShellBase.Properties.Settings.Default.Save();
                    CurrentThemeChangedEvent.SendWith(new Tuple<AppTheme, AppTheme>(currentThemeOld, currentTheme));
                }
            }
        }
        //private ResourceDictionary _currentTheme;
        //public ResourceDictionary CurrentTheme
        //{
        //    get { return _currentTheme; }
        //    set
        //    {
        //        if (_currentTheme == value)
        //            return;

        //        if (_currentTheme != null)
        //            Application.Current.Resources.MergedDictionaries.Remove(_currentTheme);
        //        var currentThremeOld = _currentTheme;
        //        _currentTheme = value;

        //        if (_currentTheme != null)
        //            Application.Current.Resources.MergedDictionaries.Add(_currentTheme);
        //        this.RaisePropertyChanged(() => this.CurrentTheme);
        //        //if (CurrentThemeChanged != null)
        //        //    CurrentThemeChanged(this, new AdvancedPropertyChangedEventArgs(this, "CurrentTheme", currentThremeOld, _CurrentTheme));
        //        CurrentThemeChangedEvent.SendWith(new Tuple<ResourceDictionary, ResourceDictionary>(currentThremeOld, _currentTheme));
        //    }
        //}
        void ChangeTheme(AppTheme theme)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                var owner = Application.Current.MainWindow;//Window.GetWindow(this);
                if (owner != null)
                {
                    owner.Resources.BeginInit();

                    if (owner.Resources.MergedDictionaries.Count > 0)
                    {
                        if (owner.Resources.MergedDictionaries[0].Source.OriginalString.StartsWith("pack://application:,,,/Fluent;component/Themes/"))
                        {
                            owner.Resources.MergedDictionaries.RemoveAt(0);
                        }

                    }
                    switch (theme)
                    {
                        case AppTheme.Office2010Black:
                            owner.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml") });
                            break;
                        case AppTheme.Office2010Blue:
                            owner.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Blue.xaml") });
                            break;
                        case AppTheme.Office2010Silver:
                            owner.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml") });
                            break;
                        case AppTheme.Office2013:
                            //owner.Resources.MergedDictionaries.Insert(0,new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml") });
                            break;
                    }
                    owner.Resources.EndInit();
                }

                if (this.currentTheme != theme)
                {
                    Application.Current.Resources.BeginInit();
                    if (Application.Current.Resources.MergedDictionaries.Count > 0)
                    {
                        if (Application.Current.Resources.MergedDictionaries[0].Source.OriginalString.StartsWith("pack://application:,,,/Fluent;component/Themes/"))
                        {
                            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                        }
                    }
                    switch (theme)
                    {
                        case AppTheme.Office2010Black:
                        case AppTheme.Office2010Blue:
                        case AppTheme.Office2010Silver:
                            Application.Current.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Generic.xaml") });
                            break;
                        case AppTheme.Office2013:
                            Application.Current.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml") });
                            break;
                    }

                    this.currentTheme = theme;
                    Application.Current.Resources.EndInit();

                    if (owner != null)
                    {
                        owner.Style = null;
                        owner.Style = owner.FindResource("RibbonWindowStyle") as Style;
                        owner.Style = null;
                    }
                }
            }));

        }
    }
}
