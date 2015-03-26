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
    public class CheckListBox : CheckListBoxBase
    {
        #region ctors

        static CheckListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckListBox), new FrameworkPropertyMetadata((typeof(CheckListBox))));
        }

        public CheckListBox() : base(false)
        {
        }
        #endregion
    }
}


