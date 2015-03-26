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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Collections.ObjectModel;
namespace WpfDemoApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<DemoEntity> listEntities = new List<DemoEntity>();

        public Window1()
        {
            Cultures = new ObservableCollection<CultureInfo> { new CultureInfo("en"), new CultureInfo("zh-CHS") };
            InitializeComponent();
            

            this.listEntities.Add(new DemoEntity {  PropName    = "Helene", 
                                                    PropType    = DemoEnumType.Normal,
                                                    PropFeature = DemoEnumFeature.Second|DemoEnumFeature.Third, 
                                                    PropAction  = 1,
                                                    PropArray   = null});

            this.listEntities.Add(new DemoEntity {  PropName    = "Walter", 
                                                    PropType    = DemoEnumType.Special,
                                                    PropFeature = DemoEnumFeature.First|DemoEnumFeature.Forth, 
                                                    PropAction  = 0, 
                                                    PropArray   = new bool[]{true, false, true, false} });

            this.listEntities.Add(new DemoEntity {  PropName    = "Sandra", 
                                                    PropType    = DemoEnumType.Exclusive, 
                                                    PropFeature = DemoEnumFeature.None, 
                                                    PropAction  = 2, 
                                                    PropArray   = null });

            this.listBoxEntity.ItemsSource   = this.listEntities;
            this.listBoxEntity.SelectedIndex = 0;
            
        }
        public ObservableCollection<CultureInfo> Cultures { get; set; }
        CultureInfo _CurrentCulture;
        public CultureInfo CurrentCulture
        {
            get
            { return _CurrentCulture; }
            set {
                _CurrentCulture = value;
            }
        }
        private void listBoxEntity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DataContext = (DemoEntity)this.listBoxEntity.SelectedItem;
        }
    }
}
