using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WpfCheckListBox
{
    public interface ICheckCtrlBase
    {
        object  CheckedValue { get; set; }
        void    SetDisplay   (string displayText);
    }
}
