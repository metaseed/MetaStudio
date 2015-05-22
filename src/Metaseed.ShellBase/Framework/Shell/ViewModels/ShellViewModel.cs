using Catel.Logging;
using Catel.IoC;
using System.Reflection;
using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
namespace Metaseed.MetaShell.ViewModels
{
    using Services;
    using Infrastructure;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class ShellViewModel : MetaViewModel
    {
        static bool _created = false;
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            System.Diagnostics.Debug.Assert(_created == false);
            _created = true;
            //_EventAggregator = ServiceLocator.ResolveType<IEventAggregator>();
            // _EventAggregator.GetEvent<AppClosingEvent>().Subscribe(onAppClosingEvent, ThreadOption.UIThread);
            //ShellViewModel_PackageManagementPart();
            DocumentClosedEvent.Register(this, DocumentClosedEventHandler);
        }
        #endregion

        IShellService _iShellService;
        public IShellService ShellService
        {
            get
            {
                if (_iShellService != null) return _iShellService;
                _iShellService = this.GetDependencyResolver().Resolve<IShellService>();
                //this.SubscribeToWeakPropertyChangedEvent
                //_IShellService.DocumentClosed+=_IShellService_DocumentClosed;
                return _iShellService;
            }
        }

        void DocumentClosedEventHandler(DocumentClosedEvent documentClosedEvent)
        {
            IDocumentViewModel documentClosed = documentClosedEvent.Data;
        }

        #region Fields
        //IEventAggregator _EventAggregator;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Events

        #endregion

        #region Properties
        ListCollectionView _tools;
        public ListCollectionView Tools
        {
            get
            {
                if (_tools == null)
                {
                    _tools = new ListCollectionView(this.ShellService.Tools);//&& ((tool as ToolViewModel).IsVisible == false) 
                    _tools.SortDescriptions.Add(new SortDescription("NameText", ListSortDirection.Ascending));
                    _tools.Filter = (tool) => { return tool.GetType().IsSubclassOf(typeof(ToolBaseViewModel)); };
                }
                return _tools;
            }
        }

        readonly CultureInfo[] _cultures = new CultureInfo[] { new CultureInfo("en"), new CultureInfo("zh-hans") };
        public CultureInfo[] Cultures { get { return _cultures; } }

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return "MetaStudio - Ver: " + Assembly.GetEntryAssembly().GetName().Version; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        #endregion

        #region Commands
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        
       
        #endregion

        #region Methods

        
        #endregion
    }
}
