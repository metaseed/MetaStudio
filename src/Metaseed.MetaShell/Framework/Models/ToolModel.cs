using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Catel.Data;
using Metaseed.ComponentModel;
namespace Metaseed.MetaShell.Models
{
    [Serializable]
    public class ToolModel : MetaModel
    {
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [Browsable(true)]
        [LocalizedDisplayName("Metaseed.MetaShell:Resources:InstanceTitle")]
        [LocalizedDescription("Metaseed.MetaShell:Resources:InstanceTitleDescription")]
        [LocalizedCategory("Metaseed.MetaShell:Resources:InstanceTitle")]
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string NameTitle
        {
            get { return GetValue<string>(NameTitleProperty); }
            set { SetValue(NameTitleProperty, value); }
        }

         /// <summary>
         /// Register the NameTitle property so it is known in the class.
         /// </summary>
         public static readonly PropertyData NameTitleProperty = RegisterProperty("NameTitle", typeof(string), string.Empty);
    }
}
