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
    public class CheckComboBox : CheckComboBoxBase
    {
        #region ctors

        static CheckComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckComboBox), new FrameworkPropertyMetadata((typeof(CheckComboBox))));
        }

        public CheckComboBox(): base(false)
        {
        }
        #endregion
    }
}


