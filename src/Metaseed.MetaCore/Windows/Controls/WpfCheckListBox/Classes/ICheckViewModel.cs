using System;
using System.Collections.Generic;
using System.Text;

namespace WpfCheckListBox
{
    public interface ICheckViewModel
    {
        List<CheckItem>     CheckItems      { get; set; }
        Type                BoundType       { get; }
        bool                IsRadioMode     { get; set; }
        ICheckCtrlBase      HostParent      { get; set; }

        void                InitDiscovery   ();
        void                ApplyValue      (object newValue);
    }
}
