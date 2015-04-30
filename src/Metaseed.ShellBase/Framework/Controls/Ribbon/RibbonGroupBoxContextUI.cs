using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluent;

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using Catel.Logging;
using System.Diagnostics;
using Catel.IoC;
namespace Metaseed.MetaShell.Controls
{
    using Metaseed.Views;
    using Services;
    public class RibbonGroupBoxContextUI : RibbonGroupBox, IContextUI, INotifyPropertyChanged
    {
        //static readonly ILog Log = LogManager.GetCurrentClassLogger();
        static protected Ribbon Ribbon;
        bool _HasInitialized = false;
        public bool HasInitialized { get { return _HasInitialized; } }
        string _RibbonTabName = null;
        public string RibbonTabName { get { return _RibbonTabName; } set { _RibbonTabName = value; } }
        public virtual void Initialize()
        {
            Debug.Assert(Name != null);
            Debug.Assert(RibbonTabName != null);
            var shellService = ServiceLocator.Default.ResolveType<IShellService>();
            shellService.Ribbon.AddRibbonGroupBox(this,RibbonTabName);
            this.Hide(null);
            _HasInitialized = true;
        }


        public virtual void Show(object c)
        {
            this.Visibility = Visibility.Visible;
            this.DataContext = c;
        }
        public virtual void Hide(object c)
        {
            this.Visibility = Visibility.Collapsed;
            this.DataContext = c;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
