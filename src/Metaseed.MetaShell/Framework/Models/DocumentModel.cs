using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catel.Data;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Metaseed.ComponentModel;
namespace Metaseed.MetaShell.Models
{
    [Serializable]
    public class DocumentModel : MetaModel
    {
        /// <summary>
        /// InstanceTitle.
        /// </summary>
        public string InstanceTitle
        {
            get { return GetValue<string>(InstanceTitleProperty); }
            set { SetValue(InstanceTitleProperty, value); }
        }
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)//only enabled when App.cs Catel.Data.ModelBase.SuspendValidationForAllModels=false; or enable the validation per model, viewmodel
        {
            //if (string.IsNullOrWhiteSpace(InstanceTitle))
            //{
            //    validationResults.Add(FieldValidationResult.CreateError(InstanceTitleProperty, "Name of room is required"));
            //}
        }
        /// <summary>
        /// Register the InstanceTitle property so it is known in the class.
        /// </summary>
        public static readonly PropertyData InstanceTitleProperty = RegisterProperty("InstanceTitle", typeof(string), "test", InstanceTitlePropertyChanged);
        static void InstanceTitlePropertyChanged(object sender,AdvancedPropertyChangedEventArgs e){

        }
    }
}
