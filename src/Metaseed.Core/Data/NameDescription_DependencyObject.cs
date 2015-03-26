using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace Metaseed.Data
{
    [Serializable]
    public class NameDescription_DependencyObject : DependencyObject, INameDescription
    {
        public string NameText
        {
            get { return (string)GetValue(NameTextProperty); }
            set { SetValue(NameTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameTextProperty =
            DependencyProperty.Register("NameText", typeof(string), typeof(NameDescription_DependencyObject), new PropertyMetadata(null));



        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(NameDescription_DependencyObject), new PropertyMetadata(null));
    }
}
