using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace WpfCheckListBox
{
    public class RadioComboBox : CheckComboBoxBase
    {
        #region ctors

        static RadioComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioComboBox), new FrameworkPropertyMetadata((typeof(RadioComboBox))));
        }

        public RadioComboBox(): base(true)
        {
        }
        #endregion
    }
}


