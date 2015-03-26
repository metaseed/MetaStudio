using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfDemoApp
{
    public class DemoEntity
    {
        public string           PropName    { get; set; }
        public DemoEnumType     PropType    { get; set; }
        public DemoEnumFeature  PropFeature { get; set; }
        public int              PropAction  { get; set; }
        public bool[]           PropArray   { get; set; }
    }
}
