using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Metaseed.Controls
{
    /// <summary>
    /// Interaction logic for RadioListBox.xaml
    /// </summary>
    public partial class RadioListBox : ListBox
    {
        public RadioListBox()
        {
            InitializeComponent();

            SelectionMode = SelectionMode.Single;
        }

        public new SelectionMode SelectionMode
        {
            get 
            { 
                return base.SelectionMode; 
            }
            private set
            {
                base.SelectionMode = value;
            }
        }

        public bool IsTransparent
        {
            set
            {
                if (value.Equals(true))
                {
                    Border border = this.Template.FindName("theBorder", this) as Border;
                    border.BorderThickness = new Thickness(0.0);
                    border.Background = System.Windows.Media.Brushes.Transparent;
                }
                else
                {
                   Border border = this.Template.FindName("theBorder", this) as Border;
                   border.BorderBrush = this.BorderBrush;
                   border.BorderThickness = this.BorderThickness;
                   border.Background = this.Background;
                }
            }
        }

        private void ItemRadioClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem sel = (e.Source as RadioButton).TemplatedParent as ListBoxItem;
            int newIndex = this.ItemContainerGenerator.IndexFromContainer(sel); ;
            this.SelectedIndex = newIndex;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            CheckRadioButtons(e.RemovedItems, false);
            CheckRadioButtons(e.AddedItems, true);
        }

        private void CheckRadioButtons(System.Collections.IList radioButtons, bool isChecked)
        {
            foreach (object item in radioButtons)
            {
                ListBoxItem lbi = this.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;

                if (lbi != null)
                {
                    RadioButton radio = lbi.Template.FindName("radio", lbi) as RadioButton;
                    if (radio != null)
                        radio.IsChecked = isChecked;
                }
            }
        }
    }
}
