using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents.Serialization;
using Metaseed.Windows.Markup;

namespace Metaseed.MVVM.Commands
{
    public partial class RibbonButtonUIData : RibbonUIData
    {
        
        override public string ValidateAndSerialize()
        {
            if (this.UiPosition== null) throw new ArgumentNullException("UiPosition");
            if (this.UiPosition.GroupBox == null) throw new ArgumentNullException("GroupBox");
            if (string.IsNullOrEmpty(this.UiPosition.GroupBox.Name)) throw new Exception("GroupBox.Name should not be null or empty");
            if (!NameValidationHelper.IsValidIdentifierName(this.UiPosition.GroupBox.Name)) throw new Exception("GroupBox.Name is not a valid name");
            if(!string.IsNullOrEmpty(this.UiPosition.RibbonTabGroup.Name))
            {
                if (!NameValidationHelper.IsValidIdentifierName(this.UiPosition.RibbonTabGroup.Name)) throw new Exception("RibbonTabGroup.Name is not a valid name");
            }
            if (!string.IsNullOrEmpty(this.UiPosition.RibbonTab.Name))
            {
                if (!NameValidationHelper.IsValidIdentifierName(this.UiPosition.RibbonTab.Name)) throw new Exception("RibbonTab.Name is not a valid name");
            }
            return Serialize();
        }

        public override string ToString()
        {
            return this.ValidateAndSerialize();
        }
    }
}
