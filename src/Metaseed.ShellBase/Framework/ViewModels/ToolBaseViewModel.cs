using System.Windows.Input;
using Metaseed.MetaShell.Services;
using Catel.MVVM;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using Microsoft.Practices.Prism.Regions;
using Catel.Data;

namespace Metaseed.MetaShell.ViewModels
{
    using Metaseed.Views;
    using Infrastructure;
    public class ToolBaseViewModel : LayoutContentViewModel, IToolViewModel
    {
        /// <summary>
        /// override the title and preferredLocation properties
        /// </summary>
        public ToolBaseViewModel()
        {
            
        }

        public ToolBaseViewModel(ToolPaneLocation preferredLocation)
            : this()
        {
            _PreferredLocation = preferredLocation;
        }
        #region IToolViewModel
         void IContextUI.Initialize() {
            _HasInitialized = true;
        }
        bool _HasInitialized;
        public bool HasInitialized { get { return _HasInitialized; } }
        public virtual void Show(object objectWithContext)
        {
            ShellService.ShowTool(this);
        }
        public virtual void Hide(object objectWithContext)
        {
            ShellService.HideTool(this);
        }
        #endregion

        bool _canHide = true;
        public bool CanHide
        {
            get { return _canHide; }
            set
            {
                if (_canHide != value)
                {
                    _canHide = value;
                    RaisePropertyChanged(() => CanHide);
                }
            }
        }
        private ICommand _closeCommand;
        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new DelegateCommand(closeCommand/*() => IsVisible = false*/, () => true)); }
        }
        void closeCommand()
        {
            ShellService.HideTool(this);
        }
        /// <summary>
        /// override to change the default value
        /// </summary>
        public virtual ToolPaneLocation PreferredLocation
        {
            get { return _PreferredLocation; }
        }
        ToolPaneLocation _PreferredLocation = ToolPaneLocation.Left;

        public virtual double PreferredHeight
        {
            get { return 200; }
        }

        public virtual double PreferredWidth
        {
            get { return 200; }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                {
                    return;
                }
                _isVisible = value;
                RaisePropertyChanged(() => IsVisible);
            }
        }

    }
}