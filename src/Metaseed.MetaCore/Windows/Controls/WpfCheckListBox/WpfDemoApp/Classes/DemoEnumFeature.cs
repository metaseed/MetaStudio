using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfDemoApp
{
    [Flags]
    public enum DemoEnumFeature : ulong
    {
        None    = 0,
        First   = 1,
        Second  = 2,
        Third   = 4,
        Forth   = 8,
    }

}
