using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Catel.IoC;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using System.Reflection;
using System.Linq;
using System.ComponentModel;
using Catel.Messaging;
//using Microsoft.Practices.Prism.Events;
namespace Metaseed.Modules.PropertyGrid.ViewModels
{
    using MetaShell.ViewModels;
    using MetaShell.Services;
    using MetaShell.Properties;
    using Services;
    using ComponentModel;
    using MetaShell.Infrastructure;


    public class PropertyGridViewModel : ToolViewModel, IPropertyGridService
    {
        MetaPropertyGrid _PropertyGrid;
        internal MetaPropertyGrid PropertyGrid
        {
            get { return _PropertyGrid; }
            set
            {
                _PropertyGrid = value;
                SetPropertyEditor(typeof(SolidColorBrush), typeof(SolidColorBrushEditor));

                PropertyGridServiceAvailableEvent.SendWith(this);
                //_EventAggregator.GetEvent<PropertyGridServiceAvailableEvent>().Publish(this);
                //test
                //SetPropertyEditor(typeof(Codeplex.Dashboarding.ColorPointCollection), typeof(Metaseed.Gauges.ColorPointCollectionEditor));
                //AddEditorDefinition(Metaseed.Gauges.ColorRangeEditor.EditorDefinition);
            }
        }
        public static Guid ToolID = new Guid("6063911E-4C41-4850-9B04-7E325F6B46DE");
        static PropertyGridViewModel()
        {
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary { Source = new Uri(@"pack://application:,,,/Metaseed.MetaShell;component/InternalModules/PropertyGrid/Editors/PropertyGridEditorResourceDictionary.xaml", UriKind.RelativeOrAbsolute) });
        }
        // IEventAggregator _EventAggregator;
        //static int created = 0;
        public static bool Registed;// to walk around this https://catelproject.atlassian.net/browse/CTL-204
        public PropertyGridViewModel()
        {
            foreach (var tool in ShellService.Tools)
            {
                if (tool.ID.Equals(ToolID))
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
            //todo 20150127 see https://catelproject.atlassian.net/browse/CTL-204
            //created++;
            this.ID = ToolID;
            //_EventAggregator = ServiceLocator.ResolveType<IEventAggregator>();
            //_EventAggregator.GetEvent<>().Subscribe(PackageBeforeOpenEventEventHandler, ThreadOption.PublisherThread);

            //var singletonPropertyGridService = ServiceLocator.ResolveTypeAndReturnNullIfNotRegistered<IPropertyGridService>() as PropertyGridViewModel;
            //if (singletonPropertyGridService!=null)
            //{
            //    ShellService.ActiveDocumentChanged -= singletonPropertyGridService.ShellService_ActiveDocumentChanged;
            //}
            //ShellService.ActiveDocumentChanged += this.ShellService_ActiveDocumentChanged;
            if (!Registed)//if (!ServiceLocator.IsTypeRegistered<IPropertyGridService>())
            {
                //if (openPackage == false)//second time call constructor when deserialize from package
                //{
                //PackageBeforeOpenEvent.Register(this, PackageBeforeOpenEventEventHandler);
               // ActiveDocumentChangedEvent.Register(this, ActiveDocumentChangedEventHandler);
                Catel.IoC.ServiceLocator.Default.RegisterInstance<IPropertyGridService>(this);
                Registed = true;
                // }
                // else
                //{
                //openPackage = false;//first time
                //}
            }

        }
        protected override async Task Close()
        {
            base.Close();
            //Catel.IoC.ServiceLocator.Default.RemoveAllInstances(typeof(IPropertyGridService));//original is just remove type
            Catel.IoC.ServiceLocator.Default.RemoveType(typeof(IPropertyGridService));//remove instances and the type 20150127 catel 4.0 not have RemoveAllInstances
            //hasU = true;
            Registed = false;
           // PackageBeforeOpenEvent.Unregister(this, PackageBeforeOpenEventEventHandler);
        }
        public override void Show(object objectWithContext)
        {
            base.Show(objectWithContext);
            this.SelectedObject = objectWithContext;
        }
        //void ActiveDocumentChangedEventHandler(ActiveDocumentChangedEvent activeDocumentChangedEvent)
        //{
        //    this.SelectedObject = ShellService.ActiveDocument;
        //}
       // bool hasU = false;
        //static bool openPackage = false;//this line should exist to walk around this issue: https://catelproject.atlassian.net/browse/CTL-203 Note!!!solved
        //void PackageBeforeOpenEventEventHandler(PackageBeforeOpenEvent packageBeforeOpenEvent)
        //{
        //    //openPackage = true;
        //    //ActiveDocumentChangedEvent.Unregister(this, ActiveDocumentChangedEventHandler);
            
        //}

        protected override string GetLocalizedTitle()
        {
            return Resources.Properties;
        }
        public override ToolPaneLocation PreferredLocation
        {
            get { return ToolPaneLocation.Right; }
        }
        public override double PreferredWidth
        {
            get { return 260; }
        }

        public override ImageSource IconSource
        {
            get
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("pack://application:,,,/Metaseed.MetaShell;component/InternalModules/PropertyGrid/Resources/Icons/Properties.png");
                bi.EndInit();
                return bi;
            }
        }

        private object _selectedObject;
        public object SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                //fixed! now catel has added the custom attribute to properties
                //https://catelproject.atlassian.net/browse/CTL-186
                //if (value is Catel.MVVM.ViewModelBase)
                //{
                //    string[]  propertiesToShow = GetBrowsableProperties(value);
                //    _selectedObject = new _(value, propertiesToShow);
                //}
                //else
                if (object.ReferenceEquals(_selectedObject,value))
                {
                    return;
                }
                {
                    _selectedObject = value;
                }
                Show(value);
                RaisePropertyChanged(() => SelectedObject);
            }
        }

        private bool _AutoGenerateProperties;

        public bool AutoGenerateProperties
        {
            get { return _AutoGenerateProperties; }
            set
            {
                if (value == _AutoGenerateProperties)
                    return;

                _AutoGenerateProperties = value;
                RaisePropertyChanged(() => this.AutoGenerateProperties);
            }
        }
        public void AddEditorDefinition(Xceed.Wpf.Toolkit.PropertyGrid.EditorTemplateDefinition editorDefinition)
        {
            Catel.Argument.IsNotNull("PropertyGrid", PropertyGrid);
            PropertyGrid.AddEditorTemplateDefinition(editorDefinition);
        }
        public void SetPropertyEditor(Type propertyType, Type iTypeEditor)
        {
            Catel.Argument.IsNotNull("PropertyGrid", PropertyGrid);
            PropertyGrid.SetPropertyEditor(propertyType, iTypeEditor);
        }
        public void SetPropertyEditor(string propertyName, Type iTypeEditor)
        {
            Catel.Argument.IsNotNull("PropertyGrid", PropertyGrid);
            PropertyGrid.SetPropertyEditor(propertyName, iTypeEditor);
        }
    }
}