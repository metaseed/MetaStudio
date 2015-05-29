using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents.Serialization;
using Catel.ExceptionHandling;

namespace Metaseed.MVVM.Commands
{
    public partial class RibbonButtonUIData : RibbonUIData
    {
        
        override public string ValidateAndSerialize()
        {
            if (this.UiPosition== null) throw new ArgumentNullException("UiPosition");
            if (this.UiPosition.GroupBox == null) throw new ArgumentNullException("GroupBox");
            if (string.IsNullOrEmpty(this.UiPosition.GroupBox.Name)) throw new Exception("GroupBox.Name should not be null or empty");
            return Serialize();
        }

        public override string ToString()
        {
            return this.ValidateAndSerialize();
        }
    }
}
