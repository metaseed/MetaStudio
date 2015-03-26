using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Catel;
using Catel.Runtime.Serialization;
using Catel.Runtime.Serialization.Xml;
using Catel.Logging;
using Catel.IoC;
using Catel.Data;
using System.Windows.Media;

namespace Metaseed.MetaShell.ViewModels
{
    using Services;
    using Metaseed.Data;
    using Infrastructure;

    //http://blogs.msdn.com/b/dphill/archive/2011/01/23/closable-tabbed-views-in-prism.aspx
    public partial class LayoutContentViewModel : MetaViewModel, ILayoutContentViewModel
    {
        protected static readonly ILog Log = LogManager.GetCurrentClassLogger();

        IShellService _shellService;
        public IShellService ShellService
        {
            get { return _shellService ?? (_shellService = this.GetDependencyResolver().Resolve<IShellService>()); }
        }
        //IEventAggregator _EventAggregator;
        public LayoutContentViewModel()
        {
            ID = Guid.NewGuid();
            _nameText = this.GetType().Name;
            //_EventAggregator = ServiceLocator.ResolveType<IEventAggregator>();
            // ShellService.CurrentCultureChanged += _ShellService_CurrentCultureChanged;
            CurrentCultureChangedEvent.Register(this, CurrentCultureChangedEventHandler);
        }
        protected readonly DataDirtyManager DataDirtyManager = new DataDirtyManager();
        virtual protected void OnIsDataDirtyChanged(bool value)
        {

        }

        /// <summary>
        /// saved in avlondock panel layout, used to load and creat the content from package
        /// </summary>
        public virtual string ContentId { get; set; }

        public bool IsDataDirty
        {
            get { return DataDirtyManager.IsDataDirty; }
            set
            {
                if (!value)
                {
                    DataDirtyManager.ClearIsDataDirty();
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false);//only set false to clear
                }
            }
        }


        protected override async Task Close()
        {
            await base.Close();
            CurrentCultureChangedEvent.Unregister(this, CurrentCultureChangedEventHandler);
        }
        public string PackgeContentType { get { return Metaseed.ShellBase.Properties.Resources.Open; } }

        void CurrentCultureChangedEventHandler(CurrentCultureChangedEvent currentCultureChangedEvent)
        {
            //var cultures= currentCultureChangedEvent.Data;
            //var cultureOld = cultures.Item1;
            //var cultureNew = cultures.Item2;
            this.RaisePropertyChanged(() => Title);
            OnCurrentCultureChanged(currentCultureChangedEvent);
            RaisePropertyChanged("PackgeContentType");
        }
        virtual protected void OnCurrentCultureChanged(CurrentCultureChangedEvent currentCultureChangedEvent)
        {

        }

        public virtual ICommand CloseCommand { get { return null; } }
        protected virtual string GetLocalizedTitle()
        {
            return String.Empty;
        }

        protected bool isOnlyCharOrDigital(string str)
        {
            return str.All(char.IsLetterOrDigit);
        }

        public override string Title
        {
            get
            {
                string title = GetLocalizedTitle();
                //System.Diagnostics.Debug.Assert(isOnlyCharOrDigital(title));//please just input charactor or digital number!
                if (string.IsNullOrEmpty(title))
                {
                    title = NameText ?? string.Empty;
                }
                return title;
            }
            protected set
            {
                throw new System.Exception("Should Not Set Title.Please Override GetLocalizedTitle Methord.");
            }
        }
        string _nameText;
        /// <summary>
        /// part of the title as the kind id, when the GetLocalizedTitle() return null or empty
        /// </summary>
        public string NameText
        {
            get { return _nameText; }
            set
            {
                _nameText = value;
                RaisePropertyChanged(() => NameText);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
            }
        }

        public virtual string Description
        {
            get { return Title; }
        }

        /// <summary>
        /// id of the content.
        /// </summary>
        public Guid ID
        {
            get { return GetValue<Guid>(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        /// <summary>
        /// Register the ID property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IDProperty = RegisterProperty("ID", typeof(Guid), null);

        public virtual ImageSource IconSource
        {
            get { return null; }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
        bool _canClose = true;
        public bool CanClose
        {
            get { return _canClose; }
            set
            {
                if (_canClose != value)
                {
                    _canClose = value;
                    RaisePropertyChanged(() => CanClose);
                }
            }
        }
        bool _canFloat = true;
        public bool CanFloat
        {
            get { return _canFloat; }
            set
            {
                if (_canFloat != value)
                {
                    _canFloat = value;
                    RaisePropertyChanged(() => CanFloat);
                }
            }
        }
        //public ContentControl RegionContentControl { get; set; }

        //private bool _IsManageable=true;
        ///// <summary>
        ///// displayed in the management manu
        ///// </summary>
        //public bool IsManageable
        //{
        //    get { return _IsManageable; }
        //    set
        //    {
        //        if (value != _IsManageable)
        //        {
        //            _IsManageable = value;
        //            RaisePropertyChanged(() => this.IsManageable);
        //        }
        //    }
        //}




        public virtual void SaveState(Stream stream)
        {
            IXmlSerializer xmlSerializer = SerializationFactory.GetXmlSerializer();
            xmlSerializer.Serialize(this, stream);
            Log.Debug("Saved object {0}", this.ContentId);
            base.ClearIsDirtyOnAllChilds();
            this.IsDataDirty = false;
        }

        /// <summary>
        /// Notifies that the value for <see cref="P:Microsoft.Practices.Prism.IActiveAware.IsActive" /> property has changed.
        /// </summary>
        public event EventHandler IsActiveChanged;

        private bool _isActive;

        virtual protected void OnIsActiveChanged()
        {
            
        }
        /// <summary>
        /// Gets or sets a value indicating whether the object is active.
        /// </summary>
        /// <value><see langword="true" /> if the object is active; otherwise <see langword="false" />.</value>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (value == _isActive)
                    return;
                _isActive = value;
                OnIsActiveChanged();
                RaisePropertyChanged(() => this.IsActive);
                IsActiveChanged.SafeInvoke(this);
            }
        }
    }
}