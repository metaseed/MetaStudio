using System.Threading.Tasks;
using System.Windows.Input;
using Catel.Memento;
using Catel.IoC;
using Catel.MVVM;
using Microsoft.Practices.Prism.Commands;
using Catel.Data;
using System.Collections.Generic;
using Fluent;
using System;
using Catel.Runtime.Serialization;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;


namespace Metaseed.MetaShell.ViewModels
{
    using Infrastructure;
    using ViewModels;
    using Metaseed.Views;
    using ComponentModel;
    using Metaseed.Data;
    public class DocumentBaseViewModel : LayoutContentViewModel, IDocumentViewModel
    {
        public DocumentBaseViewModel()
        {
            InstanceTitleDataDirtyObj = new DataDirtyObject();
            this.DataDirtyManager.AddDataObject(InstanceTitleDataDirtyObj);
            this.DataDirtyManager.IsDataDirtyChangedEvent += DataDirtyManager_IsDataDirtyChangedEvent;
            CanFloat = true;//if not, auto enter dragging status when switch document by click the document header(the two documents's contextual tools in one tool pane hide and show), then when mouse moving the document become a floating window,and some exception occurs when drag drop
        }

        void DataDirtyManager_IsDataDirtyChangedEvent(object sender, DataDirtyEventArgs e)
        {
            RaisePropertyChanged(() => this.Title);
            RaisePropertyChanged("Description");
        }

        ///// <summary>
        ///// Gets or sets the property value.
        ///// </summary>
        //[Model]
        //public DocumentModel  ModelOfDocument
        //{
        //    get { return GetValue<DocumentModel >(ModelOfDocumentProperty); }
        //    private set { SetValue(ModelOfDocumentProperty, value); }
        //}

        /// <summary>
        /// Register the ModelOfDocument property so it is known in the class.
        /// </summary>
        // public static readonly PropertyData ModelOfDocumentProperty = RegisterProperty("ModelOfDocument", typeof(DocumentModel ));
        /// <summary>
        /// Title The User Could Change In PropertyGrid.
        /// </summary>
        [LocalizedDisplayName("Metaseed.MetaShell:Resources:InstanceTitle")]
        [LocalizedDescription("Metaseed.MetaShell:Resources:InstanceTitleDescription")]
        [LocalizedCategory("Metaseed.MetaShell:Resources:Config")]
        [PropertyOrder(1)]
        [Browsable(true)]
        //[ViewModelToModel("ModelOfDocument")]
        // [ExcludeFromSerialization] if use this the instancetile will always be the default value of model when reopen
        public string InstanceTitle
        {
            get { return GetValue<string>(InstanceTitleProperty); }
            set { SetValue(InstanceTitleProperty, value); }
        }
        /// <summary>
        /// Register the InstanceTitle property so it is known in the class.
        /// </summary>
        public static readonly PropertyData InstanceTitleProperty = RegisterProperty("InstanceTitle", typeof(string), string.Empty, (sender, e) => ((DocumentBaseViewModel)sender).OnInstanceTitleChanged());

        DataDirtyObject InstanceTitleDataDirtyObj;

        /// <summary>
        /// Called when the InstanceTitle property has changed.
        /// </summary>
        private void OnInstanceTitleChanged()
        {
            InstanceTitleDataDirtyObj.IsDataDirty = true;
        }


        protected override async Task Close()
        {
             await base.Close();
            //PackageBeforeOpenEvent.Unregister(this, new Action<PackageBeforeOpenEvent>(PackageBeforeOpenEventHandler));
        }
        
        /// <summary>
        /// called only when oldValue or newVlaue is me
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        internal protected virtual void OnActiveDocumentChanged(IDocumentViewModel oldValue, IDocumentViewModel newValue)
        {
            //using Object.ReferenceEquals to compare
            if (object.ReferenceEquals(newValue, this))
            {
                ContextualUI.Show(this);
            }
            else if (object.ReferenceEquals(oldValue, this))
            {
                ContextualUI.Hide(this);
            }
        }
        internal protected virtual void OnBeforeOpen()
        {
            ContextualUI.Clear();
        }
        internal protected virtual void OnAfterClose() { }
        protected override void OnIsActiveChanged()
        {
            base.OnIsActiveChanged();
        }

        readonly ContextUserInterface _contextualUi = new ContextUserInterface();
        public ContextUserInterface ContextualUI
        {
            get { return _contextualUi; }
        }




        private IMementoService _mementoService;
        public IMementoService MementoService
        {
            get { return _mementoService ?? (_mementoService = this.GetDependencyResolver().Resolve<IMementoService>()); }
        }

        private ICommand _closeCommand;
        public override ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new DelegateCommand(() => ShellService.CloseDocument(this), () => true));
            }
        }

        /// <summary>
        /// keep this alive when the close command executed
        /// false: when close, this view model is killed
        /// </summary>
        public bool KeepAliveWhenClose { get; set; }

        private bool _isAliveClosed = true;
        public bool IsAliveClosed
        {
            get
            {
                return (_isAliveClosed && KeepAliveWhenClose);
            }
            set
            {
                _isAliveClosed = value;
            }
        }

        /// <summary>
        /// used by PropertyGrid, if not binding error will occure.
        /// </summary>
        public string Name
        {
            get
            {
                return InstanceTitle;
            }
        }
        /// <summary>
        /// add instance title and dirty id to the title
        /// </summary>
        public override string Title
        {
            get
            {
                string title = base.Title;
                if (!string.IsNullOrEmpty(InstanceTitle))
                {
                    title = (title + "-" + InstanceTitle);
                }
                if (IsDataDirty)
                {
                    return title + "*";
                }
                else
                {
                    return title;
                }
            }
        }


        private bool _isDeletable = true;
        public bool IsDeletable
        {
            get { return _isDeletable; }
            set
            {
                if (value != _isDeletable)
                {
                    _isDeletable = value;
                    RaisePropertyChanged(() => this.IsDeletable);
                }
            }
        }


        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

        }


        /// <summary>
        /// Validates the field values of this object. Override this method to enable
        /// validation of field values.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);
            if (!isOnlyCharOrDigital(InstanceTitle))
            {
                validationResults.Add(FieldValidationResult.CreateError(InstanceTitleProperty, "The InstanceTitle Contains invalid charactor,please just input charactor or digital number!"));
            }
        }

    }

}